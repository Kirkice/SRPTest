Shader "Hidden/K-PostProcessing/Voronoi" 
{
	HLSLINCLUDE
	
	#include "../../../Shaders/StdLib.hlsl"
	#include "../../../Shaders/XPostProcessing.hlsl"
    #define HASHSCALE3 float3(.1031, .1030, .0973)
	half4 _BlurOffset;
    half _Tilling;
	TEXTURE2D_SAMPLER2D(_Source, sampler_Source);
	float4 _Source_TexelSize;
	
	struct v2f
	{
		float4 pos: POSITION;
		float2 uv: TEXCOORD0;	
		float4 uv01: TEXCOORD1;
		float4 uv23: TEXCOORD2;
		float4 uv45: TEXCOORD3;
	};
	
    float2 hash22(float2 p)
    {
        float3 p3 = frac(float3(p.xyx) * HASHSCALE3);
        p3 += dot(p3, p3.yzx+19.19);
        return frac((p3.xx+p3.yz)*p3.zy);
    }

    float wnoise(float2 p,float time) 
    {
        float2 n = floor(p);
        float2 f = frac(p);
        float md = 5.0;
        float2 m = float2(0.,0.);
        for (int i = -1;i<=1;i++) 
        {
            for (int j = -1;j<=1;j++) 
            {
                float2 g = float2(i, j);
                float2 o = hash22(n+g);
                o = 0.5+0.5*sin(time+6.28*o);
                float2 r = g + o - f;
                float d = dot(r, r);
                if (d < md) 
                {
                    md = d;
                    m = n+g+o;
                } 
            } 
        }
        return md;
    }

	v2f VertGaussianBlur(AttributesDefault v)
	{
		v2f o;
		o.pos = float4(v.vertex.xy, 0, 1);
		
		o.uv.xy = TransformTriangleVertexToUV(o.pos.xy);
		
		#if UNITY_UV_STARTS_AT_TOP
			o.uv = o.uv * float2(1.0, -1.0) + float2(0.0, 1.0);
		#endif
		o.uv = TransformStereoScreenSpaceTex(o.uv, 1.0);
		
		o.uv01 = o.uv.xyxy + _BlurOffset.xyxy * float4(1, 1, -1, -1);
		o.uv23 = o.uv.xyxy + _BlurOffset.xyxy * float4(1, 1, -1, -1) * 2.0;
		o.uv45 = o.uv.xyxy + _BlurOffset.xyxy * float4(1, 1, -1, -1) * 6.0;
		
		return o;
	}
	
	float4 FragGaussianBlur(v2f i): SV_Target
	{
		half4 color = float4(0, 0, 0, 0);
		color += 0.40 * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
		color += 0.15 * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv01.xy);
		color += 0.15 * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv01.zw);
		color += 0.10 * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv23.xy);
		color += 0.10 * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv23.zw);
		color += 0.05 * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv45.xy);
		color += 0.05 * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv45.zw);
		return color;
	}
	
	
	float4 FragCombine(VaryingsDefault i): SV_Target
	{
        float2 uv = _Tilling * i.texcoordStereo;
        float time = _Time.y;
        float val = wnoise(uv,time);
		float4 sourceColor = SAMPLE_TEXTURE2D(_Source, sampler_Source, i.texcoordStereo);
		float4 blurColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoordStereo);
		float4 finalRGBA = lerp(blurColor, sourceColor * 2, val);
		return finalRGBA;
	}
	
	
	ENDHLSL
	
	SubShader
	{
		Cull Off ZWrite Off ZTest Always
		
		Pass
		{
			HLSLPROGRAM
			#pragma vertex VertGaussianBlur
			#pragma fragment FragGaussianBlur
			ENDHLSL
		}
		
		Pass
		{
			HLSLPROGRAM
			#pragma vertex VertDefault
			#pragma fragment FragCombine
			ENDHLSL
		}
	}
}    