Shader "Kirk-PendererPipeline/LightModel/LambertModel"
{
    Properties
    {
        _Color("Main Color",color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags{ "Queue" = "Geometry" }
        Pass
        {
			Tags{ "LightMode" = "KirkLit" }
			HLSLPROGRAM
			#pragma vertex VERT_LAMBERT
			#pragma fragment FRAGMENT_LAMBERT
            #include "UnityCG.cginc"
            #include "CommonLighting.hlsl"
            #include "BaseLightModel.hlsl"
			ENDHLSL
        }
    }
}
