Shader "Hidden/K-PostProcessing/Explosure"
{
	HLSLINCLUDE

	#include "../../../Shaders/StdLib.hlsl"
	#include "../../../Shaders/XPostProcessing.hlsl"

	uniform float _Explosure;

	half4 frag(VaryingsDefault i) : SV_Target
	{
		float3 mainColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord).rgb * pow(2, _Explosure / 2);
		return float4(mainColor, 1);
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