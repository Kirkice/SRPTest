Shader "Kirk-PendererPipeline/LightModel/PhongModel"
{
    Properties
    {
        _Color("Main Color",color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _SpecularPow("SpecularPow",Range(10,100)) = 3
    }
    SubShader
    {
        Tags{ "Queue" = "Geometry" }
        Pass
        {
			Tags{ "LightMode" = "KirkLit" }
			HLSLPROGRAM
			#pragma vertex VERT_PHONG
			#pragma fragment FRAGMENT_PHONG
            #include "UnityCG.cginc"
            #include "CommonLighting.hlsl"
            #include "BaseLightModel.hlsl"
			ENDHLSL
        }
    }
}
