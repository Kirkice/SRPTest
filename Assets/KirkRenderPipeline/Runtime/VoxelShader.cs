using UnityEngine;
using UnityEngine.Rendering;

public class VoxelShader : System.IDisposable 
{
    /// <summary>
    /// 变量声明
    /// </summary>
    #region Parames
    public ComputeShader compute
    {
        get
        {
            if (_compute == null) _compute = (ComputeShader)Resources.Load("GI/Compute/VoxelShader");

            return _compute;
        }
    }

    const string sampleCleanup = "Cleanup";
    const string sampleComputeAggregate = "Compute.Aggregate";
    const string sampleComputeClear = "Compute.Clear";
    const string sampleComputeRender = "Compute.Render";
    const string sampleSetup = "Setup";

    int _kernelAggregate;
    int _kernelClear;
    int _kernelRender;
    CommandBuffer _command;
    ComputeBuffer _arguments;
    ComputeBuffer _lightSources;
    ComputeShader _compute;
    NumThreads _threadsAggregate;
    NumThreads _threadsClear;
    NumThreads _threadsTrace;
    RenderTextureDescriptor _descriptor;
    KirkGI _gi;
    #endregion

    /// <summary>
    /// 体素参数设置
    /// </summary>
    /// <param name="gi"></param>
    #region VoxelShader
    public VoxelShader(KirkGI gi) {
    _gi = gi;

    _command = new CommandBuffer { name = "KirkGI.VoxelShader" };

    _arguments = new ComputeBuffer(3, sizeof(int), ComputeBufferType.IndirectArguments);
    _arguments.SetData(new int[] { 1, 1, 1 });
    _lightSources = new ComputeBuffer(64, LightSource.size);

    _kernelAggregate = KirkRenderPipeline.isD3D11Supported ? 0 : 1;
    _kernelClear = compute.FindKernel("CSClear");
    _kernelRender = compute.FindKernel("CSRender");

    _threadsAggregate = new NumThreads(compute, _kernelAggregate);
    _threadsClear = new NumThreads(compute, _kernelClear);
    _threadsTrace = new NumThreads(compute, _kernelRender);

    _descriptor = new RenderTextureDescriptor() {
      colorFormat = RenderTextureFormat.RInt,
      dimension = TextureDimension.Tex3D,
      enableRandomWrite = true,
      msaaSamples = 1,
      sRGB = false
    };
  }
    #endregion

    #region Dispose
    public void Dispose()
    {
        _arguments.Dispose();
        _command.Dispose();
        _lightSources.Dispose();
    }
    #endregion

    /// <summary>
    /// 渲染
    /// </summary>
    /// <param name="renderContext"></param>
    #region Render
    public void Render(ScriptableRenderContext renderContext)
    {
        Setup();
        ComputeClear();
        ComputeRender();
        ComputeAggregate();
        Cleanup();

        renderContext.ExecuteCommandBuffer(_command);
        _command.Clear();
    }
    #endregion

    #region Cleanup
    void Cleanup()
    {
        _command.BeginSample(sampleCleanup);

        _command.ReleaseTemporaryRT(ShaderIDs.RadianceBA);
        _command.ReleaseTemporaryRT(ShaderIDs.RadianceRG);
        _command.ReleaseTemporaryRT(ShaderIDs.RadianceCount);

        _command.EndSample(sampleCleanup);
    }
    #endregion

    /// <summary>
    /// ComputeShader设置
    /// </summary>
    #region Compute
    void ComputeAggregate()
    {
        _command.BeginSample(sampleComputeAggregate);

        _command.SetComputeTextureParam(compute, _kernelAggregate, ShaderIDs.RadianceBA, ShaderIDs.RadianceBA);
        _command.SetComputeTextureParam(compute, _kernelAggregate, ShaderIDs.RadianceRG, ShaderIDs.RadianceRG);
        _command.SetComputeTextureParam(compute, _kernelAggregate, ShaderIDs.RadianceCount, ShaderIDs.RadianceCount);
        _command.SetComputeTextureParam(compute, _kernelAggregate, ShaderIDs.Target, _gi.radiances[0]);
        _command.DispatchCompute(compute, _kernelAggregate,
          Mathf.CeilToInt((float)_gi.resolution / _threadsAggregate.x),
          Mathf.CeilToInt((float)_gi.resolution / _threadsAggregate.y),
          Mathf.CeilToInt((float)_gi.resolution / _threadsAggregate.z)
        );

        _command.EndSample(sampleComputeAggregate);
    }

    void ComputeClear()
    {
        _command.BeginSample(sampleComputeClear);

        _command.SetComputeTextureParam(compute, _kernelClear, ShaderIDs.RadianceBA, ShaderIDs.RadianceBA);
        _command.SetComputeTextureParam(compute, _kernelClear, ShaderIDs.RadianceRG, ShaderIDs.RadianceRG);
        _command.SetComputeTextureParam(compute, _kernelClear, ShaderIDs.RadianceCount, ShaderIDs.RadianceCount);
        _command.DispatchCompute(compute, _kernelClear,
          Mathf.CeilToInt((float)_gi.resolution / _threadsClear.x),
          Mathf.CeilToInt((float)_gi.resolution / _threadsClear.y),
          Mathf.CeilToInt((float)_gi.resolution / _threadsClear.z)
        );

        _command.EndSample(sampleComputeClear);
    }

    void ComputeRender()
    {
        _command.BeginSample(sampleComputeRender);

        _lightSources.SetData(_gi.lights);

        _command.SetComputeIntParam(compute, ShaderIDs.Resolution, (int)_gi.resolution);
        _command.SetComputeIntParam(compute, ShaderIDs.LightCount, _gi.lights.Count);
        _command.SetComputeBufferParam(compute, _kernelRender, ShaderIDs.LightSources, _lightSources);
        _command.SetComputeBufferParam(compute, _kernelRender, ShaderIDs.VoxelBuffer, _gi.voxelBuffer);
        _command.SetComputeMatrixParam(compute, ShaderIDs.VoxelToWorld, _gi.voxelToWorld);
        _command.SetComputeMatrixParam(compute, ShaderIDs.WorldToVoxel, _gi.worldToVoxel);
        _command.SetComputeTextureParam(compute, _kernelRender, ShaderIDs.RadianceBA, ShaderIDs.RadianceBA);
        _command.SetComputeTextureParam(compute, _kernelRender, ShaderIDs.RadianceRG, ShaderIDs.RadianceRG);
        _command.SetComputeTextureParam(compute, _kernelRender, ShaderIDs.RadianceCount, ShaderIDs.RadianceCount);

        for (var i = 0; i < 9; i++)
        {
            _command.SetComputeTextureParam(compute, _kernelRender, ShaderIDs.Radiance[i], _gi.radiances[Mathf.Min(i, _gi.radiances.Length - 1)]);
        }

        _command.CopyCounterValue(_gi.voxelBuffer, _arguments, 0);
        _gi.parameterizer.Parameterize(_command, _arguments, _threadsTrace);
        _command.DispatchCompute(compute, _kernelRender, _arguments, 0);

        _command.EndSample(sampleComputeRender);
    }
    #endregion

    /// <summary>
    /// SetUp
    /// </summary>
    #region SetUp
    void Setup()
    {
        _command.BeginSample(sampleSetup);

        UpdateNumThreads();
        _descriptor.height = _descriptor.width = _descriptor.volumeDepth = (int)_gi.resolution;
        _command.GetTemporaryRT(ShaderIDs.RadianceCount, _descriptor);
        _command.GetTemporaryRT(ShaderIDs.RadianceBA, _descriptor);
        _command.GetTemporaryRT(ShaderIDs.RadianceRG, _descriptor);

        _command.EndSample(sampleSetup);
    }
    #endregion

    #region UpdateNumThreads
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    void UpdateNumThreads()
    {
        _threadsAggregate = new NumThreads(compute, _kernelAggregate);
        _threadsClear = new NumThreads(compute, _kernelClear);
        _threadsTrace = new NumThreads(compute, _kernelRender);
    }
    #endregion
}
