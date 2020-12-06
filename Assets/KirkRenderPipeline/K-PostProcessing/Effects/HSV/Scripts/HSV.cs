using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

namespace KPostProcessing
{
    [Serializable]
    [PostProcess(typeof(HSVRenderer), PostProcessEvent.AfterStack, "KirkPostProcessing/HSV")]
    public sealed class HSV : PostProcessEffectSettings
    {
        #region Public Properties
        ///H
        [SerializeField]
        [Tooltip("Hue  Slider.")]
        [Range(0, 1f)]
        public FloatParameter Hue = new FloatParameter { value = 0.0f };

        ///S
        [SerializeField]
        [Tooltip("Saturation  Slider.")]
        [Range(-1, 1f)]
        public FloatParameter Saturation = new FloatParameter { value = 0.0f };

        ///V
        [SerializeField]
        [Tooltip("Value  Slider.")]
        [Range(-1, 1f)]
        public FloatParameter Value = new FloatParameter { value = 0.0f };

        public BoolParameter showPreview = new BoolParameter { value = false };
        #endregion
    }
    public sealed class HSVRenderer : PostProcessEffectRenderer<HSV>
    {
        private const string PROFILER_TAG = "K-HSV";
        private Shader shader;

        #region Init
        public override void Init()
        {
            shader = Shader.Find("Hidden/K-PostProcessing/HSV");
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
            internal static readonly int m_H = Shader.PropertyToID("_H");
            internal static readonly int m_S = Shader.PropertyToID("_S");
            internal static readonly int m_V = Shader.PropertyToID("_V");
        }

        public override void Render(PostProcessRenderContext context)
        {
            CommandBuffer cmd = context.command;
            PropertySheet sheet = context.propertySheets.Get(shader);
            cmd.BeginSample(PROFILER_TAG);
            sheet.properties.SetFloat(ShaderIDs.m_H, settings.Hue);
            sheet.properties.SetFloat(ShaderIDs.m_S, settings.Saturation);
            sheet.properties.SetFloat(ShaderIDs.m_V, settings.Value);
            cmd.BlitFullscreenTriangle(context.source, context.destination, sheet, settings.showPreview ? 1 : 0);
            cmd.EndSample(PROFILER_TAG);
        }
    }
}
