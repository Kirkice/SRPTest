#include "./CBUFFER_INPUT.hlsl"

/******************************************************************/
/******************************************************************/
/*************			Direction Light 			***************/
/******************************************************************/
/******************************************************************/

/// <summary>
/// lambertModel
/// </summary>
half3 GetDirectionLightLambert(half MAX_DIRECTIONAL_LIGHTS_COUNT,half3 viewDir,half3 normalDir,half4 _DirectionArr[4],half4 _ColorArr[4])
{
    half3 dLight = 0;
    for (int n = 0; n < MAX_DIRECTIONAL_LIGHTS_COUNT; n++)
    {			
        half halfLambert = saturate(dot(normalize(normalDir),normalize(_DirectionArr[n].xyz)));
        dLight = halfLambert * _ColorArr[n].rgb + dLight;
    }
    return dLight; 
}

/// <summary>
/// halflambertModel
/// </summary>
half3 GetDirectionLightHalfLambert(half MAX_DIRECTIONAL_LIGHTS_COUNT,half3 viewDir,half3 normalDir,half4 _DirectionArr[4],half4 _ColorArr[4])
{
    half3 dLight = 0;
    for (int n = 0; n < MAX_DIRECTIONAL_LIGHTS_COUNT; n++)
    {			
        half halfLambert = saturate(dot(normalize(normalDir),normalize(_DirectionArr[n].xyz))* 0.5 + 0.5);
        dLight += halfLambert * _ColorArr[n].rgb;
    }
    return dLight; 
}

/// <summary>
/// Phong
/// </summary>
half3 GetDirectionLightPhong(half MAX_DIRECTIONAL_LIGHTS_COUNT,half3 viewDir,half3 normalDir,half4 _DirectionArr[4],half4 _ColorArr[4])
{
    half3 dLight = 0;
    half3 spec = 0;
    for (int n = 0; n < MAX_DIRECTIONAL_LIGHTS_COUNT; n++)
    {	
        if (n == 0)
        {
            half3 reflection = normalize(reflect(-_DirectionArr[n].xyz,normalDir));
            spec = pow(max(0.0,dot(reflection,viewDir)),_SpecularPow);
        }
        half halfLambert = saturate(dot(normalize(normalDir),normalize(_DirectionArr[n].xyz)));
        dLight += (1 + spec) * halfLambert * _ColorArr[n].rgb;
    }
    return dLight; 
}

/// <summary>
/// BlinnPhong
/// </summary>
half3 GetDirectionLightBlinnPhong(half MAX_DIRECTIONAL_LIGHTS_COUNT,half3 viewDir,half3 normalDir,half4 _DirectionArr[4],half4 _ColorArr[4])
{
    half3 dLight = 0;
    half3 spec = 0;
    for (int n = 0; n < MAX_DIRECTIONAL_LIGHTS_COUNT; n++)
    {	
        if (n == 0)
        {
            half3 reflection = normalize(reflect(-_DirectionArr[n].xyz,normalDir));
            half3 halfDirection = normalize(viewDir + _DirectionArr[n].xyz);
            spec = pow(max(0.0,dot(reflection,halfDirection)),_SpecularPow);
        }
        half halfLambert = saturate(dot(normalize(normalDir),normalize(_DirectionArr[n].xyz)));
        dLight += (1 + spec) * halfLambert * _ColorArr[n].rgb;
    }
    return dLight; 
}
/// <summary>
/// CookTorrance
/// </summary>
half3 GetDirectionLightCookTorrance(half MAX_DIRECTIONAL_LIGHTS_COUNT,half3 viewDir,half3 normalDir,half4 _DirectionArr[4],half4 _ColorArr[4])
{
    half3 dLight = 0;
    half3 reflectDir = reflect(-viewDir,normalDir);
    half4 reflectiveColor = texCUBE(_Environment,reflectDir);
    for (int n = 0; n < MAX_DIRECTIONAL_LIGHTS_COUNT; n++)
    {	
        float s;
        float ln = saturate(dot(_DirectionArr[n].xyz,normalDir));

        if(ln > 0.0)
        {
            float3 h = normalize(viewDir + _DirectionArr[n].xyz);
            float nh = saturate(dot(normalDir, h));
            float nv = saturate(dot(normalDir, viewDir));                
            float vh = saturate(dot(viewDir, h));
                        
            //G项
            float nh2 = 2.0*nh;
            float g1 = (nh2*nv)/vh;
            float g2 = (nh2*ln)/vh;
            float g = min(1.0,min(g1,g2));

            //D项
            float m2 = _Roughness*_Roughness;
            float r1 = 1.0/(4.0 * m2 *pow(nh,4.0));
            float r2 = (nh*nh -1.0)/(m2 * nh*nh);
            float roughness = r1*exp(r2);

            //F项
            float fresnel = pow(1.0 - vh,5.0);
            fresnel *= (1.0-_Fresnel);
            fresnel += _Fresnel;
            s = saturate((fresnel*g*roughness)/(nv*ln*3.14));
        }      
        dLight += _ColorArr[n].rgb * ln * (_K * reflectiveColor * 2 + s * (1-_K) * reflectiveColor * 2);
    }
    return dLight;
}
/******************************************************************/
/******************************************************************/
/*************			    Point Light 			***************/
/******************************************************************/
/******************************************************************/

/// <summary>
/// lambertModel
/// </summary>
half3 GetPointLightLambert(half MAX_POINT_LIGHTS_COUNT,half3 normalDir,half3 viewDir,half3 posWS,half4 _PLightPos[4],half4 _PLightColor[4])
{
    half3 pLight = 0;
    for (int n = 0; n < MAX_POINT_LIGHTS_COUNT; n++)
    {
        half3 pLightVector = _PLightPos[n].xyz - posWS;
        half3 pLightDir = normalize(pLightVector);
        //距离平方，用于计算点光衰减
        half distanceSqr = max(dot(pLightVector, pLightVector), 0.00001);
        //点光衰减公式pow(max(1 - pow((distance*distance/range*range),2),0),2)
        half pLightAttenuation = pow(max(1 - pow((distanceSqr / (_PLightColor[n].a * _PLightColor[n].a)), 2),0), 2);
        half3 halfDir = normalize(viewDir + pLightDir);
        pLight += saturate(dot(normalDir, pLightDir)) * _PLightColor[n].rgb * pLightAttenuation;
    }
    return pLight;
}

/// <summary>
/// halflambertModel
/// </summary>
half3 GetPointLightHalfLambert(half MAX_POINT_LIGHTS_COUNT,half3 normalDir,half3 viewDir,half3 posWS,half4 _PLightPos[4],half4 _PLightColor[4])
{
    half3 pLight = 0;
    for (int n = 0; n < MAX_POINT_LIGHTS_COUNT; n++)
    {
        half3 pLightVector = _PLightPos[n].xyz - posWS;
        half3 pLightDir = normalize(pLightVector);
        //距离平方，用于计算点光衰减
        half distanceSqr = max(dot(pLightVector, pLightVector), 0.00001);
        //点光衰减公式pow(max(1 - pow((distance*distance/range*range),2),0),2)
        half pLightAttenuation = pow(max(1 - pow((distanceSqr / (_PLightColor[n].a * _PLightColor[n].a)), 2),0), 2);
        pLight += saturate(dot(normalDir, pLightDir) * 0.5 + 0.5) * _PLightColor[n].rgb * pLightAttenuation;
    }
    return pLight;
}

/// <summary>
/// Phong
/// </summary>
half3 GetPointLightPhong(half MAX_POINT_LIGHTS_COUNT,half3 normalDir,half3 viewDir,half3 posWS,half4 _PLightPos[4],half4 _PLightColor[4])
{
    half3 pLight = 0;
    half3 specular = 0;
    for (int n = 0; n < MAX_POINT_LIGHTS_COUNT; n++)
    {
        half3 pLightVector = _PLightPos[n].xyz - posWS;
        half3 pLightDir = normalize(pLightVector);
        //距离平方，用于计算点光衰减
        half distanceSqr = max(dot(pLightVector, pLightVector), 0.00001);
        //点光衰减公式pow(max(1 - pow((distance*distance/range*range),2),0),2)
        half pLightAttenuation = pow(max(1 - pow((distanceSqr / (_PLightColor[n].a * _PLightColor[n].a)), 2),0), 2);

        half3 reflection = normalize(reflect(-pLightDir,normalDir));
        specular = pow(max(0.0,dot(reflection,viewDir)),_SpecularPow);

        pLight += (specular + 1) * saturate(dot(normalDir, pLightDir)) * _PLightColor[n].rgb * pLightAttenuation;
    }
    return pLight;
}

/// <summary>
/// BlinnPhong
/// </summary>
half3 GetPointLightBlinnPhong(half MAX_POINT_LIGHTS_COUNT,half3 normalDir,half3 viewDir,half3 posWS,half4 _PLightPos[4],half4 _PLightColor[4])
{
    half3 pLight = 0;
    half3 specular = 0;
    for (int n = 0; n < MAX_POINT_LIGHTS_COUNT; n++)
    {
        half3 pLightVector = _PLightPos[n].xyz - posWS;
        half3 pLightDir = normalize(pLightVector);
        //距离平方，用于计算点光衰减
        half distanceSqr = max(dot(pLightVector, pLightVector), 0.00001);
        //点光衰减公式pow(max(1 - pow((distance*distance/range*range),2),0),2)
        half pLightAttenuation = pow(max(1 - pow((distanceSqr / (_PLightColor[n].a * _PLightColor[n].a)), 2),0), 2);

        half3 reflection = normalize(reflect(-pLightDir,normalDir));
        half3 halfDirection = normalize(pLightDir + viewDir);
        specular = pow(max(0.0,dot(reflection,halfDirection)),_SpecularPow);

        pLight += (specular + 1) * saturate(dot(normalDir, pLightDir)) * _PLightColor[n].rgb * pLightAttenuation;
    }
    return pLight;
}
/******************************************************************/
/******************************************************************/
/*************			    Spoint Light 			***************/
/******************************************************************/
/******************************************************************/

/// <summary>
/// lambertModel
/// </summary>
half3 GetSpointLightLambert(half MAX_SPOT_LIGHTS_COUNT,half3 normalDir,half3 viewDir,half3 posWS,half4 _SLightPosArr[4],half4 _SLightColorArr[4],half4 _SLightDirArr[4])
{
    half3 sLight = 0;
    for (int n = 0; n < MAX_SPOT_LIGHTS; n++)
    {
        //灯光到受光物体矢量，类似点光方向
        half3 sLightVector = _SLightPosArr[n].xyz - posWS;
        //聚光灯朝向
        half3 sLightDir = normalize(_SLightDirArr[n].xyz);
        //距离平方，与点光的距离衰减计算一样
        half distanceSqr = max(dot(sLightVector, sLightVector), 0.00001);
        //距离衰减公式同点光pow(max(1 - pow((distance*distance/range*range),2),0),2)
        half rangeAttenuation = pow(max(1 - pow((distanceSqr / (_SLightColorArr[n].a * _SLightColorArr[n].a)), 2), 0), 2);
        //灯光物体矢量与照射矢量点积
        float spotCos = saturate(dot(normalize(sLightVector), sLightDir));
        //角度衰减公式
        float spotAttenuation = saturate((spotCos - _SLightDirArr[n].w) / _SLightPosArr[n].w);

        sLight += saturate(dot(normalDir, sLightDir)) * _SLightColorArr[n].rgb * rangeAttenuation * spotAttenuation * spotAttenuation;
    }
    return sLight;
}

/// <summary>
/// halflambertModel
/// </summary>
half3 GetSpointLightHalfLambert(half MAX_SPOT_LIGHTS_COUNT,half3 normalDir,half3 viewDir,half3 posWS,half4 _SLightPosArr[4],half4 _SLightColorArr[4],half4 _SLightDirArr[4])
{
    half3 sLight = 0;
    for (int n = 0; n < MAX_SPOT_LIGHTS; n++)
    {
        //灯光到受光物体矢量，类似点光方向
        half3 sLightVector = _SLightPosArr[n].xyz - posWS;
        //聚光灯朝向
        half3 sLightDir = normalize(_SLightDirArr[n].xyz);
        //距离平方，与点光的距离衰减计算一样
        half distanceSqr = max(dot(sLightVector, sLightVector), 0.00001);
        //距离衰减公式同点光pow(max(1 - pow((distance*distance/range*range),2),0),2)
        half rangeAttenuation = pow(max(1 - pow((distanceSqr / (_SLightColorArr[n].a * _SLightColorArr[n].a)), 2), 0), 2);
        //灯光物体矢量与照射矢量点积
        float spotCos = saturate(dot(normalize(sLightVector), sLightDir));
        //角度衰减公式
        float spotAttenuation = saturate((spotCos - _SLightDirArr[n].w) / _SLightPosArr[n].w);
        sLight += saturate(dot(normalDir, sLightDir) * 0.5 + 0.5) * _SLightColorArr[n].rgb * rangeAttenuation * spotAttenuation * spotAttenuation;
    }
    return sLight;
}

/// <summary>
/// Phong
/// </summary>
half3 GetSpointLightPhong(half MAX_SPOT_LIGHTS_COUNT,half3 normalDir,half3 viewDir,half3 posWS,half4 _SLightPosArr[4],half4 _SLightColorArr[4],half4 _SLightDirArr[4])
{
    half3 sLight = 0;
    half3 sSpecular = 0;
    for (int n = 0; n < MAX_SPOT_LIGHTS; n++)
    {
        //灯光到受光物体矢量，类似点光方向
        half3 sLightVector = _SLightPosArr[n].xyz - posWS;
        //聚光灯朝向
        half3 sLightDir = normalize(_SLightDirArr[n].xyz);
        //距离平方，与点光的距离衰减计算一样
        half distanceSqr = max(dot(sLightVector, sLightVector), 0.00001);
        //距离衰减公式同点光pow(max(1 - pow((distance*distance/range*range),2),0),2)
        half rangeAttenuation = pow(max(1 - pow((distanceSqr / (_SLightColorArr[n].a * _SLightColorArr[n].a)), 2), 0), 2);
        //灯光物体矢量与照射矢量点积
        float spotCos = saturate(dot(normalize(sLightVector), sLightDir));
        //角度衰减公式
        float spotAttenuation = saturate((spotCos - _SLightDirArr[n].w) / _SLightPosArr[n].w);

        half3 reflection = normalize(reflect(-sLightDir,normalDir));
        sSpecular = pow(max(0.0,dot(reflection,viewDir)),_SpecularPow);
        sLight += (1 + sSpecular) * saturate(dot(normalDir, sLightDir)) * _SLightColorArr[n].rgb * rangeAttenuation * spotAttenuation * spotAttenuation;
    }
    return sLight;
}

/// <summary>
/// BlinnPhong
/// </summary>
half3 GetSpointLightBlinnPhong(half MAX_SPOT_LIGHTS_COUNT,half3 normalDir,half3 viewDir,half3 posWS,half4 _SLightPosArr[4],half4 _SLightColorArr[4],half4 _SLightDirArr[4])
{
    half3 sLight = 0;
    half3 sSpecular = 0;
    for (int n = 0; n < MAX_SPOT_LIGHTS; n++)
    {
        //灯光到受光物体矢量，类似点光方向
        half3 sLightVector = _SLightPosArr[n].xyz - posWS;
        //聚光灯朝向
        half3 sLightDir = normalize(_SLightDirArr[n].xyz);
        //距离平方，与点光的距离衰减计算一样
        half distanceSqr = max(dot(sLightVector, sLightVector), 0.00001);
        //距离衰减公式同点光pow(max(1 - pow((distance*distance/range*range),2),0),2)
        half rangeAttenuation = pow(max(1 - pow((distanceSqr / (_SLightColorArr[n].a * _SLightColorArr[n].a)), 2), 0), 2);
        //灯光物体矢量与照射矢量点积
        float spotCos = saturate(dot(normalize(sLightVector), sLightDir));
        //角度衰减公式
        float spotAttenuation = saturate((spotCos - _SLightDirArr[n].w) / _SLightPosArr[n].w);

        half3 reflection = normalize(reflect(-sLightDir,normalDir));
        half3 halfDirection = normalize(sLightDir + viewDir);
        sSpecular = pow(max(0.0,dot(reflection,halfDirection)),_SpecularPow);
        sLight += (1 + sSpecular) * saturate(dot(normalDir, sLightDir)) * _SLightColorArr[n].rgb * rangeAttenuation * spotAttenuation * spotAttenuation;
    }
    return sLight;
}