using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

namespace KPostProcessing
{
    [Serializable]
    [PostProcess(typeof(VoronoiRenderer), PostProcessEvent.AfterStack, "KirkPostProcessing/Voronoi")]
    public sealed class Voronoi : PostProcessEffectSettings
    {
        #region Public Properties
        [Range(0f, 5f)]
        public FloatParameter BlurRadius = new FloatParameter { value = 3f };
        [Range(1, 15)]
        public IntParameter Iteration = new IntParameter { value = 6 };
        [Range(1, 8)]
        public FloatParameter RTDownScaling = new FloatParameter { value = 2f };
        [Range(10,60)]
        public FloatParameter Tilling = new FloatParameter { value = 10f };
        #endregion
    }
    public sealed class VoronoiRenderer : PostProcessEffectRenderer<Voronoi>
    {
        private const string PROFILER_TAG = "K-Voronoi";
        private Shader shader;

        #region Init
        public override void Init()
        {
            shader = Shader.Find("Hidden/K-PostProcessing/Voronoi");
        }
        #endregion

        #region Release
        public override void Release()
        {
            base.Release();
        }
        #endregion

        static class ShaderIDs
        {
            internal static readonly int BlurRadius = Shader.PropertyToID("_BlurOffset");
            internal static readonly int BufferRT1 = Shader.PropertyToID("_BufferRT1");
            internal static readonly int BufferRT2 = Shader.PropertyToID("_BufferRT2");
            internal static readonly int Tillimg = Shader.PropertyToID("_Tilling");
        }

        public override void Render(PostProcessRenderContext context)
        {
            CommandBuffer cmd = context.command;
            PropertySheet sheet = context.propertySheets.Get(shader);

            cmd.BeginSample(PROFILER_TAG);

            int RTWidth = (int)(context.screenWidth / settings.RTDownScaling);
            int RTHeight = (int)(context.screenHeight / settings.RTDownScaling);
            cmd.GetTemporaryRT(ShaderIDs.BufferRT1, RTWidth, RTHeight, 0, FilterMode.Bilinear);
            cmd.GetTemporaryRT(ShaderIDs.BufferRT2, RTWidth, RTHeight, 0, FilterMode.Bilinear);
            context.command.BlitFullscreenTriangle(context.source, ShaderIDs.BufferRT1);
            sheet.properties.SetFloat(ShaderIDs.Tillimg, settings.Tilling);
            for (int i = 0; i < settings.Iteration; i++)
            {
                // horizontal blur
                sheet.properties.SetVector(ShaderIDs.BlurRadius, new Vector4(settings.BlurRadius / context.screenWidth, 0, 0, 0));
                context.command.BlitFullscreenTriangle(ShaderIDs.BufferRT1, ShaderIDs.BufferRT2, sheet, 0);

                // vertical blur
                sheet.properties.SetVector(ShaderIDs.BlurRadius, new Vector4(0, settings.BlurRadius / context.screenHeight, 0, 0));
                context.command.BlitFullscreenTriangle(ShaderIDs.BufferRT2, ShaderIDs.BufferRT1, sheet, 0);
            }
            RenderTexture renderTexture = RenderTexture.GetTemporary(context.width, context.height, 0, RenderTextureFormat.DefaultHDR);
            cmd.SetRenderTarget(context.source, renderTexture);
            sheet.properties.SetTexture("_Source", renderTexture);
            cmd.BlitFullscreenTriangle(ShaderIDs.BufferRT1, context.destination, sheet, 1);
            cmd.ReleaseTemporaryRT(ShaderIDs.BufferRT1);
            cmd.ReleaseTemporaryRT(ShaderIDs.BufferRT2);
            RenderTexture.ReleaseTemporary(renderTexture);
            cmd.EndSample(PROFILER_TAG);
        }
    }
}
