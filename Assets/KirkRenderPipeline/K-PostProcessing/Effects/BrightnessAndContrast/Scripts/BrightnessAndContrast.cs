using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

namespace KPostProcessing
{
    [Serializable]
    [PostProcess(typeof(BrightnessAndContrastRenderer), PostProcessEvent.AfterStack, "KirkPostProcessing/BrightnessAndContrast")]
    public sealed class BrightnessAndContrast : PostProcessEffectSettings
    {
        #region Public Properties
        ///亮度
        [SerializeField]
        [Tooltip("Brightness  Slider.")]
        [Range(0, 3f)]
        public FloatParameter brightness = new FloatParameter { value = 1.0f };

        ///对比度
        [SerializeField]
        [Tooltip("Brightness  Slider.")]
        [Range(0, 3f)]
        public FloatParameter contrast = new FloatParameter { value = 1.0f };

        public BoolParameter showPreview = new BoolParameter { value = false };
        #endregion
    }
    public sealed class BrightnessAndContrastRenderer : PostProcessEffectRenderer<BrightnessAndContrast>
    {
        private const string PROFILER_TAG = "K-BrightnessAndContrast";
        private Shader shader;

        #region Init
        public override void Init()
        {
            shader = Shader.Find("Hidden/K-PostProcessing/BrightnessAndContrast");
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
            internal static readonly int m_brightness = Shader.PropertyToID("_Brightness");
            internal static readonly int m_contrast = Shader.PropertyToID("_Contrast");
        }

        public override void Render(PostProcessRenderContext context)
        {
            CommandBuffer cmd = context.command;
            PropertySheet sheet = context.propertySheets.Get(shader);
            cmd.BeginSample(PROFILER_TAG);
            sheet.properties.SetFloat(ShaderIDs.m_brightness, settings.brightness);
            sheet.properties.SetFloat(ShaderIDs.m_contrast, settings.contrast);
            cmd.BlitFullscreenTriangle(context.source, context.destination, sheet, settings.showPreview ? 1 : 0);
            cmd.EndSample(PROFILER_TAG);
        }
    }
}
