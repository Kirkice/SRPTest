using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

namespace KPostProcessing
{
    [Serializable]
    [PostProcess(typeof(SharpenRenderer), PostProcessEvent.AfterStack, "KirkPostProcessing/Sharpen")]
    public sealed class Sharpen : PostProcessEffectSettings
    {
        #region Public Properties
        [Range(0.0f, 5.0f)]
        public FloatParameter Strength = new FloatParameter { value = 0.5f };

        [Range(0.0f, 1.0f)]
        public FloatParameter Threshold = new FloatParameter { value = 0.1f };
        #endregion
    }
    public sealed class SharpenRenderer : PostProcessEffectRenderer<Sharpen>
    {
        private const string PROFILER_TAG = "K-Sharpen";
        private Shader shader;

        #region Init
        public override void Init()
        {
            shader = Shader.Find("Hidden/K-PostProcessing/Sharpen");
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
            internal static readonly int Strength = Shader.PropertyToID("_Strength");
            internal static readonly int Threshold = Shader.PropertyToID("_Threshold");
        }

        public override void Render(PostProcessRenderContext context)
        {
            CommandBuffer cmd = context.command;
            PropertySheet sheet = context.propertySheets.Get(shader);
            cmd.BeginSample(PROFILER_TAG);

            sheet.properties.SetFloat(ShaderIDs.Strength, settings.Strength);
            sheet.properties.SetFloat(ShaderIDs.Threshold, settings.Threshold);

            context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
            cmd.EndSample(PROFILER_TAG);
        }
    }
}
