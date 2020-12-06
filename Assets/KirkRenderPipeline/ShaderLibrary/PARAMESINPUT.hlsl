CBUFFER_START(UnityPerMaterial)

uniform int _MeshIndex;
uniform float _IOR;
static const float _TraceCount = 8;
uniform float _AbsorbIntensity;
uniform float _ColorAdd;
uniform float _ColorMultiply;

uniform sampler2D _MainTex;
uniform half4 _MainTex_ST;
uniform sampler2D _LUT;
uniform half4 _LUT_ST;
uniform sampler2D _MetallicTex;
uniform half4 _MetallicTex_ST;
uniform sampler2D _RoughnessTex;
uniform half4 _RoughnessTex_ST;
uniform sampler2D _AmbientOcclusion;
uniform half4 _AmbientOcclusion_ST;
uniform sampler2D _NormalTex;
uniform half4 _NormalTex_ST;

uniform float3 _Color;
uniform half _Metallic;
uniform half _Roughness;
uniform half _Specular;
uniform half _SpecTrans;
uniform half3 _TransColor;
uniform half3 _Emission;
uniform half _UseTexture;
uniform half _AOFactor;
uniform half _NormalScale;

UNITY_DECLARE_TEXCUBE(_CubeMap);
half4 _CubeMap_HDR;

static const float EPSILON = 1e-8;

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