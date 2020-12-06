Shader "Hidden/K-PostProcessing/Bloom" 
{
	SubShader
	{
        // 0: Prefilter
        Pass
        {
            ZTest Always Cull Off ZWrite Off
            HLSLPROGRAM
            #pragma multi_compile _ UNITY_COLORSPACE_GAMMA
            #include "Bloom.cginc"
            #pragma vertex VertDefault
            #pragma fragment frag_prefilter
            #pragma target 3.0
            ENDHLSL
        }
        // 1: Prefilter with anti-flicker
        Pass
        {
            ZTest Always Cull Off ZWrite Off
            HLSLPROGRAM
            #define ANTI_FLICKER 1
            #pragma multi_compile _ UNITY_COLORSPACE_GAMMA
            #include "Bloom.cginc"
            #pragma vertex VertDefault
            #pragma fragment frag_prefilter
            #pragma target 3.0
            ENDHLSL
        }
        // 2: First level downsampler
        Pass
        {
            ZTest Always Cull Off ZWrite Off
            HLSLPROGRAM
            #include "Bloom.cginc"
            #pragma vertex VertDefault
            #pragma fragment frag_downsample1
            #pragma target 3.0
            ENDHLSL
        }
        // 3: First level downsampler with anti-flicker
        Pass
        {
            ZTest Always Cull Off ZWrite Off
            HLSLPROGRAM
            #define ANTI_FLICKER 1
            #include "Bloom.cginc"
            #pragma vertex VertDefault
            #pragma fragment frag_downsample1
            #pragma target 3.0
            ENDHLSL
        }
        // 4: Second level downsampler
        Pass
        {
            ZTest Always Cull Off ZWrite Off
            HLSLPROGRAM
            #include "Bloom.cginc"
            #pragma vertex VertDefault
            #pragma fragment frag_downsample2
            #pragma target 3.0
            ENDHLSL
        }
        // 5: Upsampler
        Pass
        {
            ZTest Always Cull Off ZWrite Off
            HLSLPROGRAM
            #include "Bloom.cginc"
            #pragma vertex vert_multitex
            #pragma fragment frag_upsample
            #pragma target 3.0
            ENDHLSL
        }
        // 6: High quality upsampler
        Pass
        {
            ZTest Always Cull Off ZWrite Off
            HLSLPROGRAM
            #define HIGH_QUALITY 1
            #include "Bloom.cginc"
            #pragma vertex vert_multitex
            #pragma fragment frag_upsample
            #pragma target 3.0
            ENDHLSL
        }
        // 7: Combiner
        Pass
        {
            ZTest Always Cull Off ZWrite Off
            HLSLPROGRAM
            #pragma multi_compile _ UNITY_COLORSPACE_GAMMA
            #include "Bloom.cginc"
            #pragma vertex vert_multitex
            #pragma fragment frag_upsample_final
            #pragma target 3.0
            ENDHLSL
        }
        // 8: High quality combiner
        Pass
        {
            ZTest Always Cull Off ZWrite Off
            HLSLPROGRAM
            #define HIGH_QUALITY 1
            #pragma multi_compile _ UNITY_COLORSPACE_GAMMA
            #include "Bloom.cginc"
            #pragma vertex vert_multitex
            #pragma fragment frag_upsample_final
            #pragma target 3.0
            ENDHLSL
        }
	}
}