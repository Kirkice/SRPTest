using UnityEngine;
using UnityEngine.Rendering;

public class Voxelizer : System.IDisposable {
    /// <summary>
    /// 私有变量
    /// </summary>
    #region Private Parames
    int _antiAliasing;
    int _resolution;
    Camera _camera;
    CommandBuffer _command;
    DrawingSettings _drawingSettings;
    FilteringSettings _filteringSettings;
    RenderTextureDescriptor _cameraDescriptor;
    ScriptableCullingParameters _cullingParameters;
    KirkGI _gi;
    #endregion

    /// <summary>
    /// Voxelizer
    /// </summary>
    /// <param name="gi"></param>
    #region Voxelizer
    public Voxelizer(KirkGI gi) {
    _gi = gi;

    _command = new CommandBuffer { name = "KirkGI.Voxelizer" };

    CreateCamera();
    CreateCameraDescriptor();
    CreateCameraSettings();
  }
    #endregion

    /// <summary>
    /// Dispose
    /// </summary>
    #region Dispose
    public void Dispose() {
#if UNITY_EDITOR
    GameObject.DestroyImmediate(_camera.gameObject);
#else
    GameObject.Destroy(_camera.gameObject);
#endif

    _command.Dispose();
  }
    #endregion

    /// <summary>
    /// Voxelize
    /// </summary>
    /// <param name="renderContext"></param>
    /// <param name="renderer"></param>
    #region Voxelize
    public void Voxelize(ScriptableRenderContext renderContext, GIRenderer renderer) {
    if (!_camera.TryGetCullingParameters(out _cullingParameters)) return;
  
    var cullingResults = renderContext.Cull(ref _cullingParameters);

    _gi.lights.Clear();

    foreach (var light in cullingResults.visibleLights) {
      if (KirkGI.supportedLightTypes.Contains(light.lightType) && light.finalColor.maxColorComponent > 0f) {
        _gi.lights.Add(new LightSource(light, _gi.worldToVoxel));
      }
    }

    UpdateCamera();

    _command.BeginSample(_command.name);

    _command.GetTemporaryRT(ShaderIDs.Dummy, _cameraDescriptor);
    _command.SetRenderTarget(ShaderIDs.Dummy, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.DontCare);

    _command.SetGlobalInt(ShaderIDs.Resolution, _resolution);
    _command.SetRandomWriteTarget(1, _gi.voxelBuffer, false);
    _command.SetViewProjectionMatrices(_camera.worldToCameraMatrix, _camera.projectionMatrix);

    _drawingSettings.perObjectData = renderer.RenderPipeline.PerObjectData;

    renderContext.ExecuteCommandBuffer(_command);
    renderContext.DrawRenderers(cullingResults, ref _drawingSettings, ref _filteringSettings);

    _command.Clear();

    _command.ClearRandomWriteTargets();
    _command.ReleaseTemporaryRT(ShaderIDs.Dummy);

    _command.EndSample(_command.name);

    renderContext.ExecuteCommandBuffer(_command);

    _command.Clear();
  }
    #endregion


    #region CameraSet
    void CreateCamera()
    {
        var gameObject = new GameObject("__" + _gi.name + "_VOXELIZER__") { hideFlags = HideFlags.HideAndDontSave };
        gameObject.SetActive(false);

        _camera = gameObject.AddComponent<Camera>();
        _camera.allowMSAA = true;
        _camera.aspect = 1f;
        _camera.orthographic = true;
    }

    void CreateCameraDescriptor()
    {
        _cameraDescriptor = new RenderTextureDescriptor()
        {
            colorFormat = RenderTextureFormat.R8,
            dimension = TextureDimension.Tex2D,
            memoryless = RenderTextureMemoryless.Color | RenderTextureMemoryless.Depth | RenderTextureMemoryless.MSAA,
            volumeDepth = 1,
            sRGB = false
        };
    }

    void CreateCameraSettings()
    {
        var sortingSettings = new SortingSettings(_camera) { criteria = SortingCriteria.OptimizeStateChanges };
        _drawingSettings = new DrawingSettings(ShaderTagIDs.Voxelization, sortingSettings);
        _filteringSettings = new FilteringSettings(RenderQueueRange.all);
    }

    void UpdateCamera()
    {
        if (_antiAliasing != (int)_gi.antiAliasing)
        {
            _antiAliasing = (int)_gi.antiAliasing;
            _cameraDescriptor.msaaSamples = _antiAliasing;
        }

        if (_resolution != (int)_gi.resolution)
        {
            _resolution = (int)_gi.resolution;
            _cameraDescriptor.height = _cameraDescriptor.width = _resolution;
        }

        _camera.farClipPlane = .5f * _gi.bound;
        _camera.nearClipPlane = -.5f * _gi.bound;
        _camera.orthographicSize = .5f * _gi.bound;
        _camera.transform.position = _gi.voxelSpaceCenter;
    }
    #endregion
}
