using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

namespace KPostProcessing
{
    [Serializable]
    [PostProcess(typeof(RGBSplitRenderer), PostProcessEvent.AfterStack, "KirkPostProcessing/RGBSplit")] 
    public sealed class RGBSplit : PostProcessEffectSettings
    {
        #region Public Properties
        public BoolParameter showPreview = new BoolParameter { value = false };
        [SerializeField]
        [Tooltip("RGBSplit Strength.")]
        [Range(-0.2f, 0.2f)]
        public FloatParameter splitStrength = new FloatParameter { value = 0.0f };
        #endregion
    }

    public sealed class RGBSplitRenderer : PostProcessEffectRenderer<RGBSplit>
    {
        private const string PROFILER_TAG = "K-RadialBlur";
        private Shader shader;

        #region Init
        public override void Init()
        {
            shader = Shader.Find("Hidden/K-PostProcessing/RGBSplit");
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
            internal static readonly int m_splitFactor = Shader.PropertyToID("_Scale");
        }

        public override void Render(PostProcessRenderContext context)
        {
            CommandBuffer cmd = context.command;
            PropertySheet sheet = context.propertySheets.Get(shader);
            cmd.BeginSample(PROFILER_TAG);
            sheet.properties.SetFloat(ShaderIDs.m_splitFactor, settings.splitStrength);
            cmd.BlitFullscreenTriangle(context.source, context.destination, sheet, settings.showPreview ? 1 : 0);
            cmd.EndSample(PROFILER_TAG);
        }
    }
}
