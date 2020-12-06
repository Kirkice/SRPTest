#include "Common.cginc"
#include "BRDFs.cginc"
#include "BTDFs.cginc"
#include "UnityStandardBRDF.cginc" 

//***************** GetLitMaterialGIDirect
half3 GetLitMaterialGIDirect(half MAX_DIRECTIONAL_LIGHTS_COUNT,half3 V,half3 N,half4 _DirectionArr[4],half4 _ColorArr[4])
{
    half3 dLight = 0;
    for (int n = 0; n < MAX_DIRECTIONAL_LIGHTS_COUNT; n++)
    {	
        half blender = 0.5;//used to blend BSDF and BRDF

        if (blender < 1 - _SpecTrans)
        {
            half3 L = normalize(_DirectionArr[n].xyz);
            half3 H = normalize(V + L);

            half3 NdotL = abs(dot(N, L));
            half3 NdotH = abs(dot(N, H));
            half3 VdotH = abs(dot(V, H));
            half3 NdotV = abs(dot(N, V));

            half diffuseRatio = 0.5 * (1.0 - _Metallic);
            half specularRoatio = 1 - diffuseRatio;

            half3 F0 = half3(0.08, 0.08, 0.08);
            F0 = lerp(F0 * _Specular, _Color, _Metallic);

            half NDF = DistributionGGX(N, H, _Roughness);
            half G = GeometrySmith(N, V, L, _Roughness);
            half3 F = FresnelSchlick(max(dot(H, V), 0.0), F0);

            half3 specularBrdf = SpecularBRDF(NDF, G, F, V, L, N);
            half speccualrPdf = ImportanceSampleGGX_PDF(NDF, NdotH, VdotH);

            half3 kS = F;
            half3 kD = 1.0 - kS;
            kD *= 1.0 - _Metallic;

            half3 diffuseBrdf = DiffuseBRDF(_Color);
            half diffusePdf = CosinSamplingPDF(NdotL);
            
            half3 totalBrdf = (diffuseBrdf * kD + specularBrdf) * NdotL;
            half totalPdf = diffuseRatio * diffusePdf + specularRoatio * speccualrPdf;

            half3 energy = saturate(totalBrdf + totalPdf);
            dLight += energy * _ColorArr[n].rgb;
        }
        else
        {
            bool fromOutside = dot(normalize(-V), N) < 0;
            half3 Normal = fromOutside ? N : -N;
            half3 bias = Normal * 0.001f;
            half etai = 1;
            half etat = 1.55;
            
            half3 H = ImportanceSampleGGX(float2(rand(), rand()), Normal, V, _Roughness);
            
            half3 F0 = half3(0.08, 0.08, 0.08);
            F0 = F0 * _Specular;

            half3 F = FresnelSchlick(max(dot(H, V), 0.0), F0);
            half kr = Calculatefresnel(-V, Normal, 1.55);
            half specularRoatio = kr;
            half refractionRatio = 1 - kr;

            half3 L = normalize(_DirectionArr[n].xyz);

            half NdotL = abs(dot(Normal, L));
            half NdotV = abs(dot(Normal, V));
            
            half NdotH = abs(dot(Normal, H));
            half VdotH = abs(dot(V, H));
            half LdotH = abs(dot(L, H));
            half NDF = DistributionGGX(Normal, H, _Roughness);
            half G = GeometrySmith(Normal, V, L, _Roughness);

            half3 specularBrdf = SpecularBRDF(NDF, G, F, V, L, Normal);
            half speccualrPdf = ImportanceSampleGGX_PDF(NDF, NdotH, VdotH);

            half etaOut = etat;
            half etaIn = etai;

            half3 refractionBtdf = RefractionBTDF(NDF, G, F, V, L, Normal, H, etaIn, etaOut);
            half refractionPdf = ImportanceSampleGGX_PDF(NDF, NdotH, VdotH);

            half3 totalBrdf = (specularBrdf + refractionBtdf * _TransColor) * NdotL;
            half totalPdf = specularRoatio * speccualrPdf + refractionRatio * refractionPdf;

            half3 energy = saturate(totalBrdf / totalPdf);
            dLight += energy * _ColorArr[n].rgb;
        }
    }
    return dLight * _AOFactor;
}

//***************** GetLitMaterialGIInDirect
half3 GetLitMaterialGIInDirect(half3 V,half3 N)
{
    half3 NdotV = abs(dot(N, V));

    half3 F0 = half3(0.08, 0.08, 0.08);
    F0 = lerp(F0 * _Specular, _Color, _Metallic);
    half roughness = _Roughness * _Roughness;
    half3 ambient_contrib = ShadeSH9(float4(N, 1));
    float3 ambient = 0.03 * _Color;
    float3 iblDiffuse = max(half3(0, 0, 0), ambient.rgb + ambient_contrib);
    float3 Flast = fresnelSchlickRoughness(max(NdotV, 0.0), F0, roughness);
    float kdLast = (1 - Flast) * (1 - _Metallic);
    float3 iblDiffuseResult = iblDiffuse * kdLast * _Color;

    //间接光 镜面反射
    float mip_roughness = _Roughness * (1.7 - 0.7 * _Roughness);
    float3 reflectVec = reflect(-V, N);
    half mip = mip_roughness * UNITY_SPECCUBE_LOD_STEPS;
    half4 rgbm = UNITY_SAMPLE_TEXCUBE_LOD(_CubeMap, reflectVec, mip);
    float3 iblSpecular = DecodeHDR(rgbm, _CubeMap_HDR);
    float2 envBDRF = tex2D(_LUT, float2(lerp(0, 0.99, NdotV.x), lerp(0, 0.99, roughness))).rg; // LUT采样
    float3 iblSpecularResult = iblSpecular * (Flast * envBDRF.r + envBDRF.g);
    float3 IndirectResult = iblDiffuseResult + iblSpecularResult;
    return IndirectResult * _AOFactor;
}