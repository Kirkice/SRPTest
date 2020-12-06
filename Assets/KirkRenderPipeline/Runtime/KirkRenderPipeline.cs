using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Rendering;

public class KirkRenderPipeline : RenderPipeline {
    /// <summary>
    /// 公有成员
    /// </summary>
    #region Public Parames
    public static bool isD3D11Supported => _D3D11DeviceType.Contains(SystemInfo.graphicsDeviceType);

    public PerObjectData PerObjectData { get; }

    static readonly ReadOnlyCollection<GraphicsDeviceType> _D3D11DeviceType = new ReadOnlyCollection<GraphicsDeviceType>(new[] {
    GraphicsDeviceType.Direct3D11,
    GraphicsDeviceType.Direct3D12,
    GraphicsDeviceType.XboxOne,
    GraphicsDeviceType.XboxOneD3D12
  });
    #endregion

    /// <summary>
    /// 私有成员
    /// </summary>
    #region Private Parames
    CommandBuffer _command;
    FilteringSettings _filteringSettings;
    ScriptableCullingParameters _cullingParameters;
    GIRenderer _renderer;

    private CommandBuffer LitcommandBuffer;
    const int maxDirectionalLights = 4;                                                                                                 //平行光
    Vector4[] DLightColors = new Vector4[maxDirectionalLights];
    Vector4[] DLightDirections = new Vector4[maxDirectionalLights];

    const int maxPointLights = 4;                                                                                                       //点光源
    Vector4[] PLightColors = new Vector4[maxPointLights];
    Vector4[] PLightPos = new Vector4[maxPointLights];

    const int maxSpotLights = 4;                                                                                                        //聚光灯
    Vector4[] SLightColors = new Vector4[maxSpotLights];
    Vector4[] SLightDirections = new Vector4[maxSpotLights];
    Vector4[] SLightPos = new Vector4[maxSpotLights];
    #endregion

    /// <summary>
    /// 相机回调
    /// </summary>
    /// <param name="camera"></param>
    /// <param name="message"></param>
    /// <param name="callback"></param>
    #region TriggerCameraCallback
    public static void TriggerCameraCallback(Camera camera, string message, Camera.CameraCallback callback)
    {
        camera.SendMessage(message, SendMessageOptions.DontRequireReceiver);
        if (callback != null) callback(camera);
    }
    #endregion

    /// <summary>
    /// KirkRenderPipeline构造函数
    /// </summary>
    /// <param name="asset"></param>
    #region KirkRenderPipeline
    public KirkRenderPipeline(KirkRenderPipelineAsset asset) {
    _renderer = new GIRenderer(this);
    _command = new CommandBuffer() { name = "KirkGI.RenderPipeline" };
    _filteringSettings = FilteringSettings.defaultValue;

    PerObjectData = asset.perObjectData;
    Shader.globalRenderPipeline = "KirkGI";
    GraphicsSettings.lightsUseLinearIntensity = true;
    GraphicsSettings.useScriptableRenderPipelineBatching = asset.SRPBatching;
  }
    #endregion

    /// <summary>
    /// 释放
    /// </summary>
    /// <param name="disposing"></param>
    #region Dispose
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        _command.Dispose();

        Shader.globalRenderPipeline = string.Empty;
    }
    #endregion

    /// <summary>
    /// 渲染
    /// </summary>
    /// <param name="renderContext"></param>
    /// <param name="cameras"></param>
    #region Render
    protected override void Render(ScriptableRenderContext renderContext, Camera[] cameras) {
    var mainCamera = Camera.main;

    BeginFrameRendering(renderContext, cameras);

    foreach (var camera in cameras) {
      Camera.SetupCurrent(camera);
      BeginCameraRendering(renderContext, camera);

      if (camera.cameraType == CameraType.Game) {
        if (camera.TryGetComponent<KirkGI>(out var gi) && gi.isActiveAndEnabled) {
          gi.Render(renderContext, _renderer);
        } else {
          RenderFallback(renderContext, camera);
        }
      } else {
#if UNITY_EDITOR
        if (camera.cameraType == CameraType.SceneView) {
          ScriptableRenderContext.EmitWorldGeometryForSceneView(camera);
        }

        if (mainCamera != null && mainCamera.TryGetComponent<KirkGI>(out var gi) && gi.isActiveAndEnabled) {
          gi.Render(renderContext, camera, _renderer);
        } else {
          RenderFallback(renderContext, camera);
        }
#else
        RenderFallback(renderContext, camera);
#endif
      }

      EndCameraRendering(renderContext, camera);
      renderContext.Submit();
    }

    EndFrameRendering(renderContext, cameras);
    renderContext.Submit();
  }
    #endregion

    /// <summary>
    /// 渲染回调
    /// </summary>
    /// <param name="renderContext"></param>
    /// <param name="camera"></param>
    #region RenderFallback
    void RenderFallback(ScriptableRenderContext renderContext, Camera camera)
    {
        ClearDirectionLight();
        if (LitcommandBuffer == null) LitcommandBuffer = new CommandBuffer() { name = "Kirk Render Pipeline Test" };                          //渲染开始后，创建CommandBuffer;

        var _DLightPos = Shader.PropertyToID("_kLightDirectionArr");
        var _DLightColor = Shader.PropertyToID("_kLightColorArr");

        var _PLightPos = Shader.PropertyToID("_kPLightPosArr");
        var _PLightColor = Shader.PropertyToID("_kPLightColorArr");

        var _SLightPos = Shader.PropertyToID("_SLightPosArr");
        var _SLightColor = Shader.PropertyToID("_SLightColorArr");
        var _SLightDir = Shader.PropertyToID("_SLightDirArr");

        renderContext.SetupCameraProperties(camera);                                                                                      //设置渲染相关相机参数,包含相机的各个矩阵和剪裁平面等
        LitcommandBuffer.ClearRenderTarget(true, true, Color.gray);                                                                    //清理myCommandBuffer，设置渲染目标的颜色为灰色。
                                                                                                                                    //裁剪
        ScriptableCullingParameters cullingParameters = new ScriptableCullingParameters();                                          //自定义一个剪裁参数，cullParam类里有很多可以设置的东西。
        camera.TryGetCullingParameters(out cullingParameters);                                                                      //直接使用相机默认剪裁参数
        cullingParameters.isOrthographic = false;                                                                                   //非正交相机
        CullingResults cullingResults = renderContext.Cull(ref cullingParameters);                                                        //获取剪裁之后的全部结果(其中不仅有渲染物体，还有相关的其他渲染要素)
                                                                                                                                    //灯光
        var lights = cullingResults.visibleLights;                                                                                  //在剪裁结果中获取灯光并进行参数获取
        LitcommandBuffer.name = "Light Render";
        int dLightIndex = 0;
        int pLightIndex = 0;
        int sLightIndex = 0;
        foreach (var light in lights)
        {
            if (light.lightType != LightType.Directional)                                                                           //判断灯光类型
            {
                if (light.lightType != LightType.Point)
                {
                    if (light.lightType == LightType.Spot)                                                                           //聚光灯
                    {
                        SLightColors[sLightIndex] = light.finalColor;
                        SLightColors[sLightIndex].w = light.range;                                                                  //将聚光灯的距离设置塞到颜色的A通道
                        Vector4 lightpos = light.localToWorldMatrix.GetColumn(2);                                                   //矩阵第三列为朝向，第四列为位置
                        SLightDirections[sLightIndex] = -lightpos;
                        float outerRad = Mathf.Deg2Rad * 0.5f * light.spotAngle;                                                    //外角弧度-unity中设置的角度为外角全角，我们之取半角进行计算
                        float outerCos = Mathf.Cos(outerRad);                                                                       //外角弧度cos值和tan值
                        float outerTan = Mathf.Tan(outerRad);
                        float innerRad = Mathf.Atan(((46f / 64f) * outerTan));                                                      //内角弧度计算-设定内角tan值为外角tan值的46/64
                        float innerCos = Mathf.Cos(innerRad);                                                                       //内角弧度cos值
                        SLightPos[sLightIndex] = light.localToWorldMatrix.GetColumn(3);
                        SLightDirections[sLightIndex].w = outerCos;                                                                 //角度计算用的cos(ro)与cos(ri) - cos(ro)分别存入方向与位置的w分量
                        SLightPos[sLightIndex].w = innerCos - outerCos;

                        sLightIndex++;
                    }
                    else                                                                                                            //区域灯
                    {

                    }
                }
                else                                                                                                                //点光源
                {
                    if (pLightIndex < maxPointLights)
                    {
                        PLightColors[pLightIndex] = light.finalColor;                                                               //将点光源的距离设置塞到颜色的A通道
                        PLightColors[pLightIndex].w = light.range;                                                                  //矩阵第4列为位置
                        PLightPos[pLightIndex] = light.localToWorldMatrix.GetColumn(3);
                        pLightIndex++;
                    }
                }
            }
            else                                                                                                                    //平行光
            {
                if (dLightIndex < maxDirectionalLights)
                {
                    Vector4 lightpos = light.localToWorldMatrix.GetColumn(2);                                                        //获取灯光参数,平行光朝向即为灯光Z轴方向。矩阵第一到三列分别为xyz轴项，第四列为位置。
                    DLightColors[dLightIndex] = light.finalColor;
                    DLightDirections[dLightIndex] = -lightpos;
                    DLightDirections[dLightIndex].w = 0;
                    dLightIndex++;
                }
            }
        }
        LitcommandBuffer.SetGlobalVectorArray(_DLightColor, DLightColors);
        LitcommandBuffer.SetGlobalVectorArray(_DLightPos, DLightDirections);

        LitcommandBuffer.SetGlobalVectorArray(_PLightColor, PLightColors);
        LitcommandBuffer.SetGlobalVectorArray(_PLightPos, PLightPos);

        LitcommandBuffer.SetGlobalVectorArray(_SLightPos, SLightPos);
        LitcommandBuffer.SetGlobalVectorArray(_SLightColor, SLightColors);
        LitcommandBuffer.SetGlobalVectorArray(_SLightDir, SLightDirections);

        renderContext.ExecuteCommandBuffer(LitcommandBuffer);                                                                                 //执行CommandBuffer中的指令
        LitcommandBuffer.Clear();
        //渲染设置
        SortingSettings sortingSettings = new SortingSettings(camera) { criteria = SortingCriteria.CommonOpaque };                   //渲染时，会牵扯到渲染排序，所以先要进行一个相机的排序设置，这里Unity内置了一些默认的排序可以调用
        DrawingSettings drawingSettings = new DrawingSettings(new ShaderTagId("KirkLit"), sortingSettings);                   //这边进行渲染的相关设置，需要指定渲染的shader的光照模式
                                                                                                                                     //过滤                                                                                                                       
        FilteringSettings filteringSettings = new FilteringSettings(RenderQueueRange.opaque, -1);                                    //这边是指定渲染的种类(对应shader中的Rendertype)和相关Layer的设置(-1表示全部layer)

        renderContext.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);                                           //绘制物体
        renderContext.DrawSkybox(camera);                                                                                                  //绘制天空球
        renderContext.Submit();
        //TriggerCameraCallback(camera, "OnPreRender", Camera.onPreRender);

        //if (!camera.TryGetCullingParameters(out _cullingParameters)) return;

        //TriggerCameraCallback(camera, "OnPreCull", Camera.onPreCull);

        //var cullingResults = renderContext.Cull(ref _cullingParameters);
        //var drawingSettings = new DrawingSettings { perObjectData = PerObjectData };
        //drawingSettings.SetShaderPassName(0, ShaderTagIDs.ForwardBase);
        //drawingSettings.SetShaderPassName(1, ShaderTagIDs.PrepassBase);
        //drawingSettings.SetShaderPassName(2, ShaderTagIDs.Always);
        //drawingSettings.SetShaderPassName(3, ShaderTagIDs.Vertex);
        //drawingSettings.SetShaderPassName(4, ShaderTagIDs.VertexLMRGBM);
        //drawingSettings.SetShaderPassName(5, ShaderTagIDs.VertexLM);

        //renderContext.SetupCameraProperties(camera);

        //_command.ClearRenderTarget(
        //  (camera.clearFlags & CameraClearFlags.Depth) != 0,
        //  camera.clearFlags == CameraClearFlags.Color,
        //  camera.backgroundColor
        //);
        //renderContext.ExecuteCommandBuffer(_command);
        //_command.Clear();

        //renderContext.DrawRenderers(cullingResults, ref drawingSettings, ref _filteringSettings);
        //if (camera.clearFlags == CameraClearFlags.Skybox) renderContext.DrawSkybox(camera);
        //renderContext.InvokeOnRenderObjectCallback();

        //TriggerCameraCallback(camera, "OnPostRender", Camera.onPostRender);
    }
    #endregion

    /// <summary>
    /// 清理灯光
    /// </summary>
    #region ClearDirectionLight
    private void ClearDirectionLight()
    {
        for (int i = 0; i < maxDirectionalLights; i++)
        {
            DLightDirections[i] = new Vector4(0, 0, 0, 0);
            DLightColors[i] = new Vector4(0, 0, 0, 0);
        }
    }
    #endregion
}
