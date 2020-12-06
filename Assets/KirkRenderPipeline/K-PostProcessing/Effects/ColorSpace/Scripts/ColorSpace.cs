using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

namespace KPostProcessing
{
    [Serializable]
    [PostProcess(typeof(ColorSpaceRenderer), PostProcessEvent.AfterStack, "KirkPostProcessing/ColorSpace")]
    public sealed class ColorSpace : PostProcessEffectSettings
    {
        [Range(0.001f, 3f)]
        public FloatParameter power = new FloatParameter { value = 1f };
    }
    public sealed class ColorSpaceRenderer : PostProcessEffectRenderer<ColorSpace>
    {
        private const string PROFILER_TAG = "K-ColorSpace";
        private Shader shader;

        #region Init
        public override void Init()
        {
            shader = Shader.Find("Hidden/K-PostProcessing/ColorSpace");
        }
        #endregion

        #region Release
        public override void Release()
        {
            base.Release();
        }
        #endregion

        public override void Render(PostProcessRenderContext context)
        {
            CommandBuffer cmd = context.command;
            PropertySheet sheet = context.propertySheets.Get(shader);
            cmd.BeginSample(PROFILER_TAG);
            sheet.properties.SetFloat("_Power", settings.power);
            cmd.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
            cmd.EndSample(PROFILER_TAG);
        }
    }
}
