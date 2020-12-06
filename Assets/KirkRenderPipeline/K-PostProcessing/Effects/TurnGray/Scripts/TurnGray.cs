using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

namespace KPostProcessing
{
    [Serializable]
    [PostProcess(typeof(TurnGrayRenderer), PostProcessEvent.AfterStack, "KirkPostProcessing/TurnGray")]
    public sealed class TurnGray : PostProcessEffectSettings
    {
        #region Public Properties
        [SerializeField]
        [Tooltip("TureGray  Slider.")]
        [Range(0, 1f)]
        public FloatParameter Strength = new FloatParameter { value = 0.0f };
        public BoolParameter showPreview = new BoolParameter { value = false };
        #endregion
    }
    public sealed class TurnGrayRenderer : PostProcessEffectRenderer<TurnGray>
    {
        private const string PROFILER_TAG = "K-TurnGray";
        private Shader shader;

        #region Init
        public override void Init()
        {
            shader = Shader.Find("Hidden/K-PostProcessing/TurnGray");
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
            internal static readonly int m_Strength = Shader.PropertyToID("_Strength");
        }

        public override void Render(PostProcessRenderContext context)
        {
            CommandBuffer cmd = context.command;
            PropertySheet sheet = context.propertySheets.Get(shader);
            cmd.BeginSample(PROFILER_TAG);
            sheet.properties.SetFloat(ShaderIDs.m_Strength, settings.Strength);
            cmd.BlitFullscreenTriangle(context.source, context.destination, sheet, settings.showPreview ? 1 : 0);
            cmd.EndSample(PROFILER_TAG);
        }
    }
}
