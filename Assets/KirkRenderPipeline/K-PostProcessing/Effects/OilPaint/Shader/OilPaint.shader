Shader "Hidden/K-PostProcessing/OilPaint"
{
	SubShader
	{
		Cull Off ZWrite Off ZTest Always
		Pass
		{
			HLSLPROGRAM
			#pragma vertex VertDefault
			#pragma fragment Frag

			#include "../../../Shaders/StdLib.hlsl"
			#include "../../../Shaders/XPostProcessing.hlsl"

			int _Radius;
			float _ResolutionValue;
			int _Width;
			int _Height;

			float4 Frag(VaryingsDefault i) : SV_Target
			{
				float2 src_size = float2(_ResolutionValue / _Width,_ResolutionValue / _Height);
				float2 uv = i.texcoord;
				float n = (_Radius + 1) * (_Radius + 1);
				float3 m0 = 0.0;
				float3 m1 = 0.0;
				float3 s0 = 0.0;
				float3 s1 = 0.0;
				float3 color = 0.0;
				for (int j = -_Radius; j <= 0; ++j) {
					for (int k = -_Radius; k <= 0; ++k) {
						color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(k, j) * src_size).rgb;
						m0 += color;
						s0 += color * color;
					}
				}
				for (int j = 0; j <= _Radius; ++j) {
					for (int k = 0; k <= _Radius; ++k) {
						color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(k, j) * src_size).rgb;
						m1 += color;
						s1 += color * color;
					}
				}
				float4 finalColor = 0.0;
				float min_sigma2 = 100;
				m0 /= n;
				s0 = abs(s0 / n - m0 * m0);
				float sigma2 = s0.r + s0.g + s0.b;
				if (sigma2 < min_sigma2)
				{
					min_sigma2 = sigma2;
					finalColor = float4(m0, 1.0);
				}
				m1 /= n;
				s1 = abs(s1 / n - m1 * m1);
				sigma2 = s1.r + s1.g + s1.b;
				if (sigma2 < min_sigma2)
				{
					min_sigma2 = sigma2;
					finalColor = float4(m1, 1.0);
				}
				return finalColor;
			}

			ENDHLSL

		}
	}
}