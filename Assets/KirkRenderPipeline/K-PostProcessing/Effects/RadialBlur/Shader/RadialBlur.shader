Shader "Hidden/K-PostProcessing/RadialBlur" 
{
	HLSLINCLUDE

	#include "../../../Shaders/StdLib.hlsl"
	#include "../../../Shaders/XPostProcessing.hlsl"

	uniform float _BlurFactor;	//模糊强度（0-0.05）
	uniform float4 _BlurCenter; //模糊中心点xy值（0-1）屏幕空间
	uniform float _SAMPLE_COUNT = 6;
	#define SAMPLE_COUNT 20		//迭代次数

	half4 frag(VaryingsDefault i) : SV_Target
	{
		//模糊方向为模糊中点指向边缘（当前像素点），而越边缘该值越大，越模糊
		float2 dir = i.texcoord - _BlurCenter.xy;
		float4 outColor = 0;
		//采样SAMPLE_COUNT次
		for (int j = 0; j < _SAMPLE_COUNT; ++j)
		{
			//计算采样uv值：正常uv值+从中间向边缘逐渐增加的采样距离
			float2 uv = i.texcoord + _BlurFactor * dir * j;
			outColor += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
		}
		//取平均值
		outColor /= _SAMPLE_COUNT;
		return outColor;
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