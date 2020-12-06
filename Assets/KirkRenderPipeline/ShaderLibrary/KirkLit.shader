Shader "KirkGI/Lit"
{
    Properties
    {
        [Enum(BRDF,0,BSDF,1)] _SpecTrans("PBR Model", Int) = 0
        [Main(g1)] _UseTexture("Use Texture",float) = 0
        [Tex(g1)]_MainTex("Base Texture",2D) = "white"{}
        [Sub(g1)]_Color("Main Color",color) = (1,1,1,1)


        [Tex(g1)]_MetallicTex("Metallic Texture",2D) = "White"{}
        [Sub(g1)]_Metallic("Metallic",Range(0,1)) = 0.5

             
        [Tex(g1)]_RoughnessTex("Roughness Texture",2D) = "White"{}
        [Sub(g1)]_Roughness("Roughness",Range(0.1,1)) = 0.5

        [Tex(g1)]_NormalTex("Normal Texture",2D) = "White"{}
        [Sub(g1)]_NormalScale("Normal Scale",Range(0,3)) = 1

        [Tex(g1)]_AmbientOcclusion("Ambient Occlusion",2D) = "White"{}

        [Main(g2,_,2)] _group2("Common Set",float) = 1
        [Sub(g2)]_Specular("Specular",Range(0,1)) = 0.5
        [Sub(g2)]_TransColor("TransColor",Color) = (1,1,1,1)
        [Sub(g2)]_Emission("Emission",Color) = (1,1,1,1)
        [Sub(g2)]_CubeMap ("Main", CUBE) = "" {}
        [Sub(g2)]_LUT("LUT",2D) = "White"{}
    }
    SubShader
    {
        Tags{ "LightMode" = "KirkLit" }
        Pass
        {
			HLSLPROGRAM
			#pragma vertex VERT_LIT
			#pragma fragment FRAGMENT_LIT
            #include "UnityCG.cginc"
            #include "PARAMESINPUT.hlsl"  
            #include "BaseFunction.hlsl" 
            #include "LitPass.hlsl"
			ENDHLSL
        }
        UsePass "Hidden/KirkGI/Voxelization/VOXELIZATION"
    }
}
