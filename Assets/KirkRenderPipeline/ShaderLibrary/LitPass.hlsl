#ifndef GI_INCLUDE
#define GI_INCLUDE

#include "Lighting.hlsl"

struct VertexInput_LIT
{
    float4 vertex   : POSITION;
    float4 uv       : TEXCOORD0;
    float3 normal   : NORMAL;
};

struct VertexOutput_LIT
{
    float4 position   : SV_POSITION;
    float2 uv         : TEXCOORD0;
    float3 normal     : NORMAL;
    float3 posWS      : TEXCOORD1;
};

VertexOutput_LIT VERT_LIT (VertexInput_LIT v) 
{
    VertexOutput_LIT o = (VertexOutput_LIT)0;  
    o.position = UnityObjectToClipPos(v.vertex);
    o.normal = UnityObjectToWorldNormal(v.normal);
    o.posWS = mul(unity_ObjectToWorld,v.vertex);
    o.uv = v.uv;
    return o;
}

half4 FRAGMENT_LIT(VertexOutput_LIT i, half facing : VFACE) : SV_TARGET
{
    ///////////////////
    //Direction
    SetMaterialTextureFactor(i.uv);
    half3 N = normalize(GetNormal(i.uv) + i.normal);
    half3 V = normalize(_WorldSpaceCameraPos.xyz - i.posWS.xyz);
    half3 DirectGI = GetLitMaterialGIDirect(MAX_DIRECTIONAL_LIGHTS,V,N,_kLightDirectionArr,_kLightColorArr);
    half3 InDirectGI = GetLitMaterialGIInDirect(V,N);
    return half4(DirectGI + InDirectGI,1);
}

#endif