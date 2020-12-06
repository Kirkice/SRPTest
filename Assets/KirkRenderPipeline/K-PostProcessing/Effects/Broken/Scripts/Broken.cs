using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

namespace KPostProcessing
{
    [Serializable]
    [PostProcess(typeof(BrokenRenderer), PostProcessEvent.AfterStack, "KirkPostProcessing/Broken")]
    public sealed class Broken : PostProcessEffectSettings
    {
        #region Public Properties
        public TextureParameter NormalTexture = new TextureParameter { value = null };

        [Range(0, 1)]
        public FloatParameter NormalStrength= new FloatParameter { value = 0.0f };
        #endregion
    }

    public sealed class BrokenRenderer : PostProcessEffectRenderer<Broken>
    {

        private const string PROFILER_TAG = "K-Broken";
        private Shader shader;

        public override void Init()
        {
            shader = Shader.Find("Hidden/K-PostProcessing/Broken");
        }

        public override void Release()
        {
            base.Release();
        }

        static class ShaderIDs
        {
            internal static readonly int BrokenTexture = Shader.PropertyToID("_BrokenNormalMap");
            internal static readonly int BrokenScale = Shader.PropertyToID("_BrokenScale");
        }

        public override void Render(PostProcessRenderContext context)
        {
            CommandBuffer cmd = context.command;
            PropertySheet sheet = context.propertySheets.Get(shader);
            cmd.BeginSample(PROFILER_TAG);
            sheet.properties.SetFloat(ShaderIDs.BrokenScale, settings.NormalStrength);
            sheet.properties.SetTexture(ShaderIDs.BrokenTexture, settings.NormalTexture);
            cmd.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
            cmd.EndSample(PROFILER_TAG);
        }
    }
}