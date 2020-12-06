Shader "Hidden/K-PostProcessing/Relief"
{
	HLSLINCLUDE

	#include "../../../Shaders/StdLib.hlsl"
	#include "../../../Shaders/XPostProcessing.hlsl"

	uniform float4 _SetColor;

	half4 frag(VaryingsDefault i) : SV_Target
	{
		float3 m_n = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord).rgb;
		float3 m_l = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord + float2(1 / _ScreenParams.x, 1 / _ScreenParams.y)).rgb;
		float3 diff = abs(m_n - m_l);
		float max = diff.r > diff.g ? diff.r : diff.g;
		max = max > diff.b ? max : diff.b;
		float gray = clamp(max + 0.2, 0, 1);
		gray = gray * gray;
		float4 c;
		c = float4(gray.xxx, 1) * _SetColor;
		return c;
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