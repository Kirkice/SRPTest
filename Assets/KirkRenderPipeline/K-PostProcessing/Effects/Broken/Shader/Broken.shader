Shader "Hidden/K-PostProcessing/Broken" 
{
	HLSLINCLUDE

	#include "../../../Shaders/StdLib.hlsl"
	#include "../../../Shaders/XPostProcessing.hlsl"

	TEXTURE2D_SAMPLER2D(_BrokenNormalMap, sampler_BrokenNormalMap);
	float4 _BrokenNormalMap_TexelSize;
	float _BrokenScale;
    
    float3 UnpackNormalmapRGorAG(float4 packednormal)
    {
        // This do the trick
        packednormal.x *= packednormal.w;
    
        float3 normal;
        normal.xy = packednormal.xy * 2 - 1;
        normal.z = sqrt(1 - saturate(dot(normal.xy, normal.xy)));
        return normal;
    }
    
    float3 UnpackNormal(float4 packednormal)
    {
    #if defined(UNITY_NO_DXT5nm)
        return packednormal.xyz * 2 - 1;
    #else
        return UnpackNormalmapRGorAG(packednormal);
    #endif
    }



	half4 Frag(VaryingsDefault i) : SV_Target
	{
		float4 packedNormal = SAMPLE_TEXTURE2D(_BrokenNormalMap, sampler_BrokenNormalMap, i.texcoord);
		float3 tangentNormal = UnpackNormal(packedNormal);
		tangentNormal.xy *= _BrokenScale;
		float2 offset = tangentNormal.xy;
		float3 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord + offset);
		return float4(col,1); 
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


