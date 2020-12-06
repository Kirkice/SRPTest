using UnityEngine;
using UnityEngine.Rendering;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(KirkGI))]
[AddComponentMenu("Rendering/KirkGI Mipmap Debug")]
public class KirkGIMipmapDebug : MonoBehaviour {
  [Range(1f, 9f)]
  public float mipmapLevel = 1f;
  [Tooltip("How big is a step when ray tracing through the voxel volume.")]
  public float rayTracingStep = .05f;
  public FilterMode filterMode = FilterMode.Point;

  Camera _camera;
  CommandBuffer _command;
  KirkGI _gi;

  void Awake() {
    _camera = GetComponent<Camera>();
    _gi = GetComponent<KirkGI>();
  }

  void OnEnable() {
    _command = new CommandBuffer { name = "KirkGI.Debug.Mipmap" };
    _camera.AddCommandBuffer(CameraEvent.AfterEverything, _command);
  }

  void OnDisable() {
    _camera.RemoveCommandBuffer(CameraEvent.AfterEverything, _command);
    _command.Dispose();
  }

  void OnPreRender() {
    if (!isActiveAndEnabled || !_gi.isActiveAndEnabled) return;

    _command.Clear();

    var transform = Matrix4x4.TRS(_gi.origin, Quaternion.identity, Vector3.one * _gi.bound);

    if (filterMode == FilterMode.Point) {
      _command.EnableShaderKeyword("RADIANCE_POINT_SAMPLER");
    } else {
      _command.DisableShaderKeyword("RADIANCE_POINT_SAMPLER");
    }

    _command.SetGlobalFloat(ShaderIDs.MipmapLevel, Mathf.Min(mipmapLevel, _gi.radiances.Length));
    _command.SetGlobalFloat(ShaderIDs.RayTracingStep, rayTracingStep);
    _command.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);
    _command.DrawProcedural(transform, VisualizationShader.material, (int)VisualizationShader.Pass.Mipmap, MeshTopology.Quads, 24, 1);
  }
}
