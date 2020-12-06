Shader "Hidden/K-PostProcessing/BrightnessAndContrast" 
{
	HLSLINCLUDE

	#include "../../../Shaders/StdLib.hlsl"
	#include "../../../Shaders/XPostProcessing.hlsl"

    uniform float _Brightness;
    uniform float _Contrast;

	half4 frag(VaryingsDefault i) : SV_Target
	{
		half4 renderTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
		half3 finalColor = renderTex * _Brightness;
		half3 avgColor = half3(0.5, 0.5, 0.5);
		finalColor = lerp(avgColor, finalColor, _Contrast);
		return half4(finalColor, renderTex.a);
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