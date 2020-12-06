#ifndef VXGI_BRDFS_GENERAL
  #define VXGI_BRDFS_GENERAL

  #include "Assets/KirkRenderPipeline/ShaderLibrary/BRDFs/Diffuse.cginc"
  #include "Assets/KirkRenderPipeline/ShaderLibrary/BRDFs/Specular.cginc"
  #include "Assets/KirkRenderPipeline/ShaderLibrary/Structs/LightingData.cginc"

  float3 GeneralBRDF(LightingData data)
  {
    return DiffuseBRDF(data) + SpecularBRDF(data);
  }
#endif
