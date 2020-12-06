CBUFFER_START(UnityPerMaterial)
    uniform half4 _Color;
    uniform sampler2D _MainTex;
    uniform half _SpecularPow;
    uniform half _Roughness;
    uniform half _Fresnel;
    uniform half _K;
    uniform samplerCUBE _Environment;
    //这边需要平行光参数用于计算
    #define MAX_DIRECTIONAL_LIGHTS 4
    uniform half4 _kLightDirectionArr[MAX_DIRECTIONAL_LIGHTS];
    uniform half4 _kLightColorArr[MAX_DIRECTIONAL_LIGHTS];
    //定义最多4盏点光
    #define MAX_POINT_LIGHTS 4
    uniform half4 _kPLightPosArr[MAX_POINT_LIGHTS];
    uniform half4 _kPLightColorArr[MAX_POINT_LIGHTS];
    //定义最多4盏聚光
    #define MAX_SPOT_LIGHTS 4
    //定义参数数组
    uniform half4 _SLightPosArr[MAX_SPOT_LIGHTS];
    uniform half4 _SLightColorArr[MAX_SPOT_LIGHTS];
    uniform half4 _SLightDirArr[MAX_SPOT_LIGHTS];
CBUFFER_END