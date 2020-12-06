/******************************************************************/
/******************************************************************/
/*************			Lambert  Model     			***************/
/******************************************************************/
/******************************************************************/
struct VertexInput_LAMBERT
{
    float4 position : POSITION;
    float2 uv       : TEXCOORD0;
    float3 normal   : NORMAL;
};

struct VertexOutput_LAMBERT
{
    float4 position   : SV_POSITION;
    float2 uv         : TEXCOORD0;
    float3 normal     : NORMAL;
    float3 posWS      : TEXCOORD1;
};

VertexOutput_LAMBERT VERT_LAMBERT (VertexInput_LAMBERT v) 
{
    VertexOutput_LAMBERT o = (VertexOutput_LAMBERT)0;  
    o.position = UnityObjectToClipPos(v.position);
    o.normal = UnityObjectToWorldNormal(v.normal);
    o.posWS = mul(unity_ObjectToWorld,v.position);
    o.uv = v.uv;
    return o;
}

half4 FRAGMENT_LAMBERT(VertexOutput_LAMBERT i, half facing : VFACE) : SV_TARGET
{
    half3 viewDirection = _WorldSpaceCameraPos.xyz - i.posWS;
    half3 DirectionLightColor = GetDirectionLightLambert(MAX_DIRECTIONAL_LIGHTS,normalize(viewDirection),i.normal,_kLightDirectionArr,_kLightColorArr);
    half3 PointLightColor = GetPointLightLambert(MAX_POINT_LIGHTS,i.normal,normalize(viewDirection),i.posWS,_kPLightPosArr,_kPLightColorArr);
    half3 SpointLightColor = GetSpointLightLambert(MAX_SPOT_LIGHTS,i.normal,normalize(viewDirection),i.posWS,_SLightPosArr,_SLightColorArr,_SLightDirArr);
    half4 finalRGBA =  half4(_Color.rgb,1.0) * tex2D(_MainTex, i.uv); 
    finalRGBA.rgb *= DirectionLightColor;
    finalRGBA.rgb = PointLightColor + SpointLightColor + finalRGBA.rgb;
    return finalRGBA;
}

/******************************************************************/
/******************************************************************/
/*************		   Half  Lambert  Model         ***************/
/******************************************************************/
/******************************************************************/
struct VertexInput_HalfLAMBERT
{
    float4 position : POSITION;
    float2 uv       : TEXCOORD0;
    float3 normal   : NORMAL;
};

struct VertexOutput_HalfLAMBERT
{
    float4 position   : SV_POSITION;
    float2 uv         : TEXCOORD0;
    float3 normal     : NORMAL;
    float3 posWS      : TEXCOORD1;
};

VertexOutput_HalfLAMBERT VERT_HALFLAMBERT (VertexInput_HalfLAMBERT v) 
{
    VertexOutput_HalfLAMBERT o = (VertexOutput_HalfLAMBERT)0;  
    o.position = UnityObjectToClipPos(v.position);
    o.normal = UnityObjectToWorldNormal(v.normal);
    o.posWS = mul(unity_ObjectToWorld,v.position);
    o.uv = v.uv;
    return o;
}

half4 FRAGMENT_HALFLAMBERT(VertexOutput_HalfLAMBERT i, half facing : VFACE) : SV_TARGET
{
    half3 viewDirection = _WorldSpaceCameraPos.xyz - i.posWS;
    half3 DirectionLightColor = GetDirectionLightHalfLambert(MAX_DIRECTIONAL_LIGHTS,normalize(viewDirection),i.normal,_kLightDirectionArr,_kLightColorArr);
    half3 PointLightColor = GetPointLightHalfLambert(MAX_POINT_LIGHTS,i.normal,normalize(viewDirection),i.posWS,_kPLightPosArr,_kPLightColorArr);
    half3 SpointLightColor = GetSpointLightHalfLambert(MAX_SPOT_LIGHTS,i.normal,normalize(viewDirection),i.posWS,_SLightPosArr,_SLightColorArr,_SLightDirArr);
    half4 finalRGBA =  half4(_Color.rgb,1.0) * tex2D(_MainTex, i.uv); 
    finalRGBA.rgb *= DirectionLightColor;
    //finalRGBA.rgb = PointLightColor + SpointLightColor + finalRGBA.rgb;
    return finalRGBA;
}

/******************************************************************/
/******************************************************************/
/*************		         Phong  Model           ***************/
/******************************************************************/
/******************************************************************/
struct VertexInput_PHONG
{
    float4 position : POSITION;
    float2 uv       : TEXCOORD0;
    float3 normal   : NORMAL;
};

struct VertexOutput_PHONG
{
    float4 position   : SV_POSITION;
    float2 uv         : TEXCOORD0;
    float3 normal     : NORMAL;
    float3 posWS      : TEXCOORD1;
};

VertexOutput_PHONG VERT_PHONG (VertexInput_PHONG v) 
{
    VertexOutput_PHONG o = (VertexOutput_PHONG)0;  
    o.position = UnityObjectToClipPos(v.position);
    o.normal = UnityObjectToWorldNormal(v.normal);
    o.posWS = mul(unity_ObjectToWorld,v.position);
    o.uv = v.uv;
    return o;
}

half4 FRAGMENT_PHONG(VertexOutput_PHONG i, half facing : VFACE) : SV_TARGET
{
    half3 viewDirection = _WorldSpaceCameraPos.xyz - i.posWS;
    half3 DirectionLightColor = GetDirectionLightPhong(MAX_DIRECTIONAL_LIGHTS,normalize(viewDirection),i.normal,_kLightDirectionArr,_kLightColorArr);
    half3 PointLightColor = GetPointLightPhong(MAX_POINT_LIGHTS,i.normal,normalize(viewDirection),i.posWS,_kPLightPosArr,_kPLightColorArr);
    half3 SpointLightColor = GetSpointLightPhong(MAX_SPOT_LIGHTS,i.normal,normalize(viewDirection),i.posWS,_SLightPosArr,_SLightColorArr,_SLightDirArr);
    half4 finalRGBA =  half4(_Color.rgb,1.0) * tex2D(_MainTex, i.uv); 
    finalRGBA.rgb *= DirectionLightColor;
    //finalRGBA.rgb = PointLightColor + SpointLightColor + finalRGBA.rgb;
    return finalRGBA;
}

/******************************************************************/
/******************************************************************/
/*************	 	    Blinn   Phong  Model        ***************/
/******************************************************************/
/******************************************************************/
struct VertexInput_BLINNPHONG
{
    float4 position : POSITION;
    float2 uv       : TEXCOORD0;
    float3 normal   : NORMAL;
};

struct VertexOutput_BLINNPHONG
{
    float4 position   : SV_POSITION;
    float2 uv         : TEXCOORD0;
    float3 normal     : NORMAL;
    float3 posWS      : TEXCOORD1;
};

VertexOutput_BLINNPHONG VERT_BLINNPHONG (VertexInput_BLINNPHONG v) 
{
    VertexOutput_BLINNPHONG o = (VertexOutput_BLINNPHONG)0;  
    o.position = UnityObjectToClipPos(v.position);
    o.normal = UnityObjectToWorldNormal(v.normal);
    o.posWS = mul(unity_ObjectToWorld,v.position);
    o.uv = v.uv;
    return o;
}

half4 FRAGMENT_BLINNPHONG(VertexOutput_BLINNPHONG i, half facing : VFACE) : SV_TARGET
{
    half3 viewDirection = _WorldSpaceCameraPos.xyz - i.posWS;
    half3 DirectionLightColor = GetDirectionLightBlinnPhong(MAX_DIRECTIONAL_LIGHTS,normalize(viewDirection),i.normal,_kLightDirectionArr,_kLightColorArr);
    half3 PointLightColor = GetPointLightBlinnPhong(MAX_POINT_LIGHTS,i.normal,normalize(viewDirection),i.posWS,_kPLightPosArr,_kPLightColorArr);
    half3 SpointLightColor = GetSpointLightBlinnPhong(MAX_SPOT_LIGHTS,i.normal,normalize(viewDirection),i.posWS,_SLightPosArr,_SLightColorArr,_SLightDirArr);
    half4 finalRGBA =  half4(_Color.rgb,1.0) * tex2D(_MainTex, i.uv); 
    finalRGBA.rgb *= DirectionLightColor;
    //finalRGBA.rgb = PointLightColor + SpointLightColor + finalRGBA.rgb;
    return finalRGBA;
}

/******************************************************************/
/******************************************************************/
/*************	 	   Cook  Torrance  Model        ***************/
/******************************************************************/
/******************************************************************/
struct VertexInput_COOKTORRANCE
{
    float4 position : POSITION;
    float2 uv       : TEXCOORD0;
    float3 normal   : NORMAL;
};

struct VertexOutput_COOKTORRANCE
{
    float4 position   : SV_POSITION;
    float2 uv         : TEXCOORD0;
    float3 normal     : NORMAL;
    float3 posWS      : TEXCOORD1;
};

VertexOutput_COOKTORRANCE VERT_COOKTORRANCE (VertexInput_COOKTORRANCE v)
{
    VertexOutput_COOKTORRANCE o;
    o.position = mul(UNITY_MATRIX_MVP, v.position);
    o.posWS = mul(unity_ObjectToWorld,v.position);
    o.uv = v.uv;
    o.normal = UnityObjectToWorldNormal(v.normal);
    return o;
}

half4 FRAGMENT_COOKTORRANCE(VertexOutput_COOKTORRANCE i, half facing : VFACE) : SV_TARGET
{
    half3 viewDirection = _WorldSpaceCameraPos.xyz - i.posWS;
    half3 DirectionLightColor = GetDirectionLightCookTorrance(MAX_DIRECTIONAL_LIGHTS,normalize(viewDirection),i.normal,_kLightDirectionArr,_kLightColorArr);
    half4 finalRGBA =  half4(_Color.rgb,1.0) * tex2D(_MainTex, i.uv); 
    finalRGBA.rgb *= DirectionLightColor;
    //finalRGBA.rgb = PointLightColor + SpointLightColor + finalRGBA.rgb;
    return finalRGBA;
}