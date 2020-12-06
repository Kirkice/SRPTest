using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

namespace KPostProcessing
{
    [Serializable]
    [PostProcess(typeof(SpherizeRenderer), PostProcessEvent.AfterStack, "KirkPostProcessing/Spherize")]
    public sealed class Spherize : PostProcessEffectSettings
    {
        [Range(0.0f, 1.0f)]
        public FloatParameter Spherify = new FloatParameter { value = 1f };
    }
    public sealed class SpherizeRenderer : PostProcessEffectRenderer<Spherize>
    {
        private const string PROFILER_TAG = "K-Spherize";
        private Shader shader;

        #region Init
        public override void Init()
        {
            shader = Shader.Find("Hidden/K-PostProcessing/Spherize");
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
            sheet.properties.SetFloat("_Spherify", settings.Spherify);
            cmd.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
            cmd.EndSample(PROFILER_TAG);
        }
    }
}
