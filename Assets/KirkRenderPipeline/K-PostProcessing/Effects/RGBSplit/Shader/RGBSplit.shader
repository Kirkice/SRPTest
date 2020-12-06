Shader "Hidden/K-PostProcessing/RGBSplit" 
{
	HLSLINCLUDE

	#include "../../../Shaders/StdLib.hlsl"
	#include "../../../Shaders/XPostProcessing.hlsl"

    uniform float _Scale;

	half4 frag(VaryingsDefault i) : SV_Target
	{
        _Scale = _Scale + 1;
        float2 newTextureCoordinate = float2((_Scale - 1.0) * 0.5 + i.texcoord.x / _Scale ,(_Scale - 1.0) *0.5 + i.texcoord.y /_Scale);
        float4 textureColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
        float4 shiftColor1 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, newTextureCoordinate + float2(-0.05 * (_Scale - 1.0), - 0.05 *(_Scale - 1.0)));
        float4 shiftColor2 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, newTextureCoordinate + float2(-0.1 * (_Scale - 1.0), - 0.1 *(_Scale - 1.0)));
        float3 blendFirstColor = fixed3(textureColor.r , textureColor.g, shiftColor1.b);
        float3 blend3DColor = fixed3(shiftColor2.r, blendFirstColor.g, blendFirstColor.b);
		return half4(blend3DColor,1);
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