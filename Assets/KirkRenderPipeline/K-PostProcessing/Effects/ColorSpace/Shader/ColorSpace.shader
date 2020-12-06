Shader "Hidden/K-PostProcessing/ColorSpace"
{
	HLSLINCLUDE

	#include "../../../Shaders/StdLib.hlsl"
	#include "../../../Shaders/XPostProcessing.hlsl"

	uniform float _Power;

	half4 frag(VaryingsDefault i) : SV_Target
	{
		half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
		color.r = pow(color.r, _Power);
		color.g = pow(color.g, _Power);
		color.b = pow(color.b, _Power);
		return color;
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