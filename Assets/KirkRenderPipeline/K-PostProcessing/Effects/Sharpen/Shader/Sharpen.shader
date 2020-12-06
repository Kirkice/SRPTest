﻿Shader "Hidden/K-PostProcessing/Sharpen"
{
	HLSLINCLUDE

	#include "../../../Shaders/StdLib.hlsl"
	#include "../../../Shaders/XPostProcessing.hlsl"

	uniform half _Strength; 
	uniform half _Threshold;

	half4 Frag(VaryingsDefault i) : SV_Target
	{

		half2 pixelSize = float2(1 / _ScreenParams.x, 1 / _ScreenParams.y);
		half2 halfPixelSize = pixelSize * 0.5;

		half4 blur = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord + half2(halfPixelSize.x, -pixelSize.y));
		blur += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord + half2(-pixelSize.x, -halfPixelSize.y));
		blur += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord + half2(pixelSize.x, halfPixelSize.y));
		blur += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord + half2(-halfPixelSize.x, pixelSize.y));
		blur *= 0.25;

		half4 sceneColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
		half4 lumaStrength = half4(0.222, 0.707, 0.071, 0.0) * _Strength;
		half4 sharp = sceneColor - blur;

		sceneColor += clamp(dot(sharp, lumaStrength), -_Threshold, _Threshold);

		return sceneColor;
	}

	ENDHLSL

	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			HLSLPROGRAM

			#pragma vertex VertDefault
			#pragma fragment Frag

			ENDHLSL

		}
	}
}