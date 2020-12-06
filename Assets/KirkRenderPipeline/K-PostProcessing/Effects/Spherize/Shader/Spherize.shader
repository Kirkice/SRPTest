Shader "Hidden/K-PostProcessing/Spherize"
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

			float _Spherify;

			float4 Frag(VaryingsDefault i) : SV_Target
			{
				float2 centered_uv = i.texcoord * 2.0 - 1.0;
				float z = sqrt(1.0 - saturate(dot(centered_uv.xy, centered_uv.xy)));
				float2 spherified_uv = centered_uv / (z + 1.0);
				float2 uv = spherified_uv * 0.5 + 0.5;

				uv = lerp(i.texcoord, uv, _Spherify);
				float4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
				half sqrDist = dot(centered_uv.xy, centered_uv.xy);
				half mask = 1.0 - sqrDist;
				mask = saturate(mask / fwidth(mask));
				col.a *= mask;
				return col;
			}
			ENDHLSL
		}
	}
}