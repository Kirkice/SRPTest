Shader "Hidden/K-PostProcessing/Mosaic" 
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

			half4 _Params;
			#define _PixelSize _Params.x
			#define _PixelRatio _Params.y
			#define _PixelScaleX _Params.z
			#define _PixelScaleY _Params.w	

			float2 GetMosaicUV( half2 uv)
			{
				float pixelScale = 1.0 / _PixelSize;
				float2 uv0 = half2(pixelScale * _PixelScaleX * floor(uv.x / (pixelScale *_PixelScaleX)), (pixelScale * _PixelRatio *_PixelScaleY) * floor(uv.y / (pixelScale *_PixelRatio * _PixelScaleY)));
				return uv0;
			}

			float4 Frag(VaryingsDefault i) : SV_Target
			{
				float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, GetMosaicUV(i.texcoord));
				return color;
			}

			ENDHLSL

		}
	}
}    