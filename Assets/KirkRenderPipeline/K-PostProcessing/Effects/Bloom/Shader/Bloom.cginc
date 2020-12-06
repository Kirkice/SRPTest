#include "../../../Shaders/StdLib.hlsl"
#include "../../../Shaders/XPostProcessing.hlsl"
#define USE_RGBM defined(SHADER_API_MOBILE)

TEXTURE2D_SAMPLER2D(_BaseTex,sampler_BaseTex);
float4 _BaseTex_TexelSize;

uniform float _PrefilterOffs;
uniform half _Threshold;
uniform half3 _Curve;
uniform float _SampleScale;
uniform half _Intensity;

// Brightness function
half Brightness(half3 c)
{
    return max(max(c.r, c.g), c.b);
}

// 3-tap median filter
half3 Median(half3 a, half3 b, half3 c)
{
    return a + b + c - min(min(a, b), c) - max(max(a, b), c);
}

// RGBM encoding/decoding
half4 EncodeHDR(float3 rgb)
{
#if USE_RGBM
    rgb *= 1.0 / 8;
    float m = max(max(rgb.r, rgb.g), max(rgb.b, 1e-6));
    m = ceil(m * 255) / 255;
    return half4(rgb / m, m);
#else
    return half4(rgb, 0);
#endif
}

float3 DecodeHDR(half4 rgba)
{
#if USE_RGBM
    return rgba.rgb * rgba.a * 8;
#else
    return rgba.rgb;
#endif
}

// Downsample with a 4x4 box filter
half3 DownsampleFilter(float2 uv)
{
    float4 d = _MainTex_TexelSize.xyxy * float4(-1, -1, +1, +1);

    half3 s;
    s  = DecodeHDR(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + d.xy));
    s += DecodeHDR(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + d.zy));
    s += DecodeHDR(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + d.xw));
    s += DecodeHDR(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + d.zw));

    return s * (1.0 / 4);
}

// Downsample with a 4x4 box filter + anti-flicker filter
half3 DownsampleAntiFlickerFilter(float2 uv)
{
    float4 d = _MainTex_TexelSize.xyxy * float4(-1, -1, +1, +1);

    half3 s1 = DecodeHDR(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + d.xy));
    half3 s2 = DecodeHDR(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + d.zy));
    half3 s3 = DecodeHDR(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + d.xw));
    half3 s4 = DecodeHDR(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + d.zw));

    // Karis's luma weighted average (using brightness instead of luma)
    half s1w = 1 / (Brightness(s1) + 1);
    half s2w = 1 / (Brightness(s2) + 1);
    half s3w = 1 / (Brightness(s3) + 1);
    half s4w = 1 / (Brightness(s4) + 1);
    half one_div_wsum = 1 / (s1w + s2w + s3w + s4w);

    return (s1 * s1w + s2 * s2w + s3 * s3w + s4 * s4w) * one_div_wsum;
}

half3 UpsampleFilter(float2 uv)
{
#if HIGH_QUALITY
    // 9-tap bilinear upsampler (tent filter)
    float4 d = _MainTex_TexelSize.xyxy * float4(1, 1, -1, 0) * _SampleScale;

    half3 s;
    s  = DecodeHDR(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + d.xy));
    s += DecodeHDR(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + d.wy)) * 2;
    s += DecodeHDR(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + d.zy));

    s += DecodeHDR(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + d.zw)) * 2;
    s += DecodeHDR(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv       )) * 4;
    s += DecodeHDR(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + d.xw)) * 2;

    s += DecodeHDR(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + d.zy));
    s += DecodeHDR(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + d.wy)) * 2;
    s += DecodeHDR(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + d.xy));

    return s * (1.0 / 16);
#else
    // 4-tap bilinear upsampler
    float4 d = _MainTex_TexelSize.xyxy * float4(-1, -1, +1, +1) * (_SampleScale * 0.5);

    half3 s;
    s  = DecodeHDR(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + d.xy));
    s += DecodeHDR(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + d.zy));
    s += DecodeHDR(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + d.xw));
    s += DecodeHDR(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + d.zw));

    return s * (1.0 / 4);
#endif
}

//
// Vertex shader
//
struct appdata
{
    float3 vertex : POSITION;
};

struct v2f_multitex
{
    float4 vertex : SV_POSITION;
    float2 uvMain : TEXCOORD0;
    float2 uvBase : TEXCOORD1;
};

v2f_multitex vert_multitex(appdata v)
{
    v2f_multitex o;
	half index = v.vertex.z;
	v.vertex.z = 0.1;
    o.vertex = float4(v.vertex, 1.0);
    o.uvMain = TransformTriangleVertexToUV(v.vertex.xy);
	o.uvBase = TransformTriangleVertexToUV(v.vertex.xy);
#if UNITY_UV_STARTS_AT_TOP
    o.uvMain = o.uvMain * float2(1.0, -1.0) + float2(0.0, 1.0);
    o.uvBase = o.uvBase * float2(1.0, -1.0) + float2(0.0, 1.0);
#endif
    return o;
}

//
// fragment shader
//

half4 frag_prefilter(VaryingsDefault i) : SV_Target
{
    float2 uv = i.texcoord + _MainTex_TexelSize.xy * _PrefilterOffs;

#if ANTI_FLICKER
    float3 d = _MainTex_TexelSize.xyx * float3(1, 1, 0);
    half4 s0 = SafeHDR(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv));
    half3 s1 = SafeHDR(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv - d.xz).rgb);
    half3 s2 = SafeHDR(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + d.xz).rgb);
    half3 s3 = SafeHDR(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv - d.zy).rgb);
    half3 s4 = SafeHDR(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + d.zy).rgb);
    half3 m = Median(Median(s0.rgb, s1, s2), s3, s4);
#else
    half4 s0 = SafeHDR(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv));
    half3 m = s0.rgb;
#endif

#if UNITY_COLORSPACE_GAMMA
    m = GammaToLinearSpace(m);
#endif
    // Pixel brightness
    half br = Brightness(m);

    // Under-threshold part: quadratic curve
    half rq = clamp(br - _Curve.x, 0, _Curve.y);
    rq = _Curve.z * rq * rq;

    // Combine and apply the brightness response curve.
    m *= max(rq, br - _Threshold) / max(br, 1e-5);

    return EncodeHDR(m);
}

half4 frag_downsample1(VaryingsDefault i) : SV_Target
{
#if ANTI_FLICKER
    return EncodeHDR(DownsampleAntiFlickerFilter(i.texcoord));
#else
    return EncodeHDR(DownsampleFilter(i.texcoord));
#endif
}

half4 frag_downsample2(VaryingsDefault i) : SV_Target
{
    return EncodeHDR(DownsampleFilter(i.texcoord));
}

half4 frag_upsample(v2f_multitex i) : SV_Target
{
    half3 base = DecodeHDR(SAMPLE_TEXTURE2D(_BaseTex, sampler_BaseTex, i.uvBase));
    half3 blur = UpsampleFilter(i.uvMain);
    return EncodeHDR(base + blur);
}

half4 frag_upsample_final(v2f_multitex i) : SV_Target
{
    half4 base = SAMPLE_TEXTURE2D(_BaseTex, sampler_BaseTex, i.uvBase);
    half3 blur = UpsampleFilter(i.uvMain);
#if UNITY_COLORSPACE_GAMMA
    base.rgb = GammaToLinearSpace(base.rgb);
#endif
    half3 cout = base.rgb + blur * _Intensity;
#if UNITY_COLORSPACE_GAMMA
    cout = LinearToGammaSpace(cout);
#endif
    return half4(cout, base.a);
}
