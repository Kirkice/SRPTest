

void SetMaterialTextureFactor(half2 uv)
{
    if(_UseTexture > 0.5)
    {
        _Metallic = tex2D(_MetallicTex,TRANSFORM_TEX(uv,_MetallicTex)).r;
        _Roughness = tex2D(_RoughnessTex,TRANSFORM_TEX(uv,_RoughnessTex)).r;
        _AOFactor = tex2D(_AmbientOcclusion,TRANSFORM_TEX(uv,_AmbientOcclusion)).r;
    }
    else
    {
        _AOFactor = 1;
    }
}

half3 GetNormal(half2 uv)
{
    if(_UseTexture > 0.5)
    {
        half3 normal = UnpackNormal(tex2D(_NormalTex,TRANSFORM_TEX(uv,_NormalTex)));
        normal *= _NormalScale;
        return normalize(half3(normal.x, 1, normal.y));
    }
    else
    {
        return half3(0,0,0);
    }
}