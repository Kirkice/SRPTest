Shader "Hidden/K-PostProcessing/TurnGray" 
{
	HLSLINCLUDE

	#include "../../../Shaders/StdLib.hlsl"
	#include "../../../Shaders/XPostProcessing.hlsl"

    uniform float _Strength;

	half4 frag(VaryingsDefault i) : SV_Target
	{
		half4 renderTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
        half GrayRGB = (renderTex.r + renderTex.g + renderTex.b) / 3;
        half4 finalRGBA = half4(lerp(renderTex.r,GrayRGB,_Strength),lerp(renderTex.g,GrayRGB,_Strength),lerp(renderTex.b,GrayRGB,_Strength),renderTex.a);
        return finalRGBA;
	}
	ENDHLSL

	SubShader
	{
		ZTest Always Cull Off ZWrite Off
		Fog{ Mode off }
		Pass
		{
			HLSLPROGRAM
			#pragma vertex VertDefault
			#pragma fragment frag
			ENDHLSL
		}
	}
}    