Shader "Hidden/K-PostProcessing/HSV" 
{
	HLSLINCLUDE

	#include "../../../Shaders/StdLib.hlsl"
	#include "../../../Shaders/XPostProcessing.hlsl"

    uniform float _H;
    uniform float _S;
    uniform float _V;

	half4 frag(VaryingsDefault i) : SV_Target
	{
		half4 renderTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
        float3 rgb = renderTex.rgb;
        float4 k = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
        float4 p = lerp(float4(rgb.bg, k.wz), float4(rgb.gb, k.xy), step(rgb.b, rgb.g));
        // 比较r和max(b,g)
        float4 q = lerp(float4(p.xyw, rgb.r), float4(rgb.r, p.yzx), step(p.x, rgb.r));
        float d = q.x - min(q.w, q.y);
        float e = 1.0e-10;
        float3 hsv = float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);

        hsv.x = hsv.x + _H;
        hsv.y = hsv.y + _S;
        hsv.z = hsv.z + _V;
        rgb = saturate(3.0*abs(1.0-2.0*frac(hsv.x+float3(0.0,-1.0/3.0,1.0/3.0)))-1); //明度和饱和度为1时的颜色
        rgb = (lerp(float3(1,1,1),rgb,hsv.y)*hsv.z); // hsv
		return half4(rgb, renderTex.a);
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