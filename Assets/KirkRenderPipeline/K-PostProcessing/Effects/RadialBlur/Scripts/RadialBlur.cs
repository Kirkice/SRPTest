using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

namespace KPostProcessing
{
    [Serializable]
    [PostProcess(typeof(RadialBlurRenderer), PostProcessEvent.AfterStack, "KirkPostProcessing/RadialBlur")]
    public sealed class RadialBlur : PostProcessEffectSettings
    {
        #region Public Properties
        ///径向模糊强度
        [SerializeField]
        [Tooltip("Radial Blur Slider.")]
        [Range(0, 0.05f)] 
        public FloatParameter blurFactor = new FloatParameter { value = 1.0f };

        ///径向模糊迭代次数
        [SerializeField]
        [Tooltip("Radial Blur Count.")]
        public FloatParameter blurCount = new FloatParameter { value = 6.0f };

        ///径向模糊中心
        [SerializeField]
        [Tooltip("Radial Blur Center.")]
        public Vector2Parameter blurCenter = new Vector2Parameter { value = new Vector2(0.5f, 0.5f) };

        public BoolParameter showPreview = new BoolParameter { value = false };
        #endregion
    }
    public sealed class RadialBlurRenderer : PostProcessEffectRenderer<RadialBlur>
    {
        private const string PROFILER_TAG = "K-RadialBlur";
        private Shader shader;

        #region Init
        public override void Init()
        {
            shader = Shader.Find("Hidden/K-PostProcessing/RadialBlur"); 
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
            internal static readonly int m_blurFactor = Shader.PropertyToID("_BlurFactor");
            internal static readonly int m_blurCenter = Shader.PropertyToID("_BlurCenter");
            internal static readonly int m_blurCount = Shader.PropertyToID("_SAMPLE_COUNT");

        }

        public override void Render(PostProcessRenderContext context)
        {
            CommandBuffer cmd = context.command;
            PropertySheet sheet = context.propertySheets.Get(shader);
            cmd.BeginSample(PROFILER_TAG);
            sheet.properties.SetFloat(ShaderIDs.m_blurFactor, settings.blurFactor);
            sheet.properties.SetVector(ShaderIDs.m_blurCenter, settings.blurCenter);
            sheet.properties.SetFloat(ShaderIDs.m_blurCount, settings.blurCount);
            cmd.BlitFullscreenTriangle(context.source, context.destination, sheet, settings.showPreview ? 1 : 0);
            cmd.EndSample(PROFILER_TAG);
        }
    }
}
