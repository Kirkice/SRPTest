Shader "Hidden/K-PostProcessing/EdgeDetectionRobert"
{
	HLSLINCLUDE
	
	#include "../../../Shaders/StdLib.hlsl"
	#include "../../../Shaders/XPostProcessing.hlsl"
	
	
	half4 _Params;
	half4 _BackgroundColor;
	half4 _OutLineColor;

	#define _EdgeWidth _Params.x
	#define _EdgeNeonFade _Params.y
	#define _BackgroundFade _Params.w
	
	
	float3 sobel(float stepx, float stepy, float2 center)
	{
		float3 topLeft = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, center + float2(-stepx, stepy)).rgb;
		float3 bottomLeft = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, center + float2(-stepx, -stepy)).rgb;
		float3 topRight = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, center + float2(stepx, stepy)).rgb;
		float3 bottomRight = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, center + float2(stepx, -stepy)).rgb;
		float3 Gx = -1.0 * topLeft + 1.0 * bottomRight;
		float3 Gy = -1.0 * topRight + 1.0 * bottomLeft;
		float3 sobelGradient = sqrt((Gx * Gx) + (Gy * Gy));
		return sobelGradient;
	}
	
	
	half4 Frag(VaryingsDefault i): SV_Target
	{
		half4 sceneColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
		float3 sobelGradient = sobel(_EdgeWidth / _ScreenParams.x, _EdgeWidth / _ScreenParams.y, i.texcoord);
		float outLineArea = step(0.1,sobelGradient.r);
		half3 backgroundColor = lerp(_BackgroundColor.rgb * sceneColor.rgb, sceneColor.rgb, _BackgroundFade) * (1 - outLineArea);
		float3 edgeColor = lerp(sceneColor.rgb, sobelGradient.rgb * _OutLineColor, _EdgeNeonFade) * outLineArea;
		return half4(edgeColor + backgroundColor,1);
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


