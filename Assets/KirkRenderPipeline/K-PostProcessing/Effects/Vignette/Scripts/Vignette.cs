using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

namespace KPostProcessing
{
    public enum VignetteType
    {
        ClassicMode = 0,
        ColorMode = 1,
    }

    [Serializable]
    public sealed class VignetteTypeParameter : ParameterOverride<VignetteType> { }

    [Serializable]
    [PostProcess(typeof(VignetteRenderer), PostProcessEvent.AfterStack, "KirkPostProcessing/Vignette")]
    public sealed class Vignette : PostProcessEffectSettings
    {
        #region Public Properties
        public VignetteTypeParameter vignetteType = new VignetteTypeParameter { value = VignetteType.ClassicMode };

        [Range(0.0f, 5.0f)]
        public FloatParameter vignetteIndensity = new FloatParameter { value = 1f };

        public Vector2Parameter vignetteCenter = new Vector2Parameter { value = new Vector2(0.5f, 0.5f) };

        [ColorUsageAttribute(true, true, 0f, 20f, 0.125f, 3f)]
        public ColorParameter vignetteColor = new ColorParameter { value = new Color(0.1f, 0.8f, 1.0f) };
        #endregion
    }
    public sealed class VignetteRenderer : PostProcessEffectRenderer<Vignette>
    {
        private const string PROFILER_TAG = "K-Vignette";
        private Shader shader;

        #region Init
        public override void Init()
        {
            shader = Shader.Find("Hidden/K-PostProcessing/Vignette");
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

            sheet.properties.SetFloat("_VignetteIndensity", settings.vignetteIndensity);
            sheet.properties.SetVector("_VignetteCenter", settings.vignetteCenter);

            if (settings.vignetteType.value == VignetteType.ColorMode)
            {
                sheet.properties.SetColor("_VignetteColor", settings.vignetteColor);
            }

            cmd.BlitFullscreenTriangle(context.source, context.destination, sheet, (int)settings.vignetteType.value);
            cmd.EndSample(PROFILER_TAG);
        }
    }
}
