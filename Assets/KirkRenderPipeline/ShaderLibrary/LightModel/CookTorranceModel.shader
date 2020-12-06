Shader "Kirk-PendererPipeline/LightModel/CookTorranceModel"
{
    Properties
    {
        _Color("Base Color",Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _Roughness("Roughness",Range(0,1)) = 1
        _Fresnel("Fresnel",Range(0,1)) = 1
        _K("K",Range(0,1)) = 1
        _Environment ("Environment", Cube) = "white"
    }
    SubShader
    {
        Tags{ "Queue" = "Geometry" }
        Pass
        {
            Tags{ "LightMode" = "KirkLit" }
            HLSLPROGRAM
            #pragma vertex VERT_COOKTORRANCE
            #pragma fragment FRAGMENT_COOKTORRANCE
            #include "UnityCG.cginc"
            #include "CommonLighting.hlsl"
            #include "BaseLightModel.hlsl"
            ENDHLSL
        }
    }
}
