using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

namespace KPostProcessing
{
    [Serializable]
    [PostProcess(typeof(ExplosureRenderer), PostProcessEvent.AfterStack, "KirkPostProcessing/Explosure")]
    public sealed class Explosure : PostProcessEffectSettings
    {
        [Range(-20, 20)]
        public FloatParameter SetExplosure = new FloatParameter { value = 1.0f };
    }
    public sealed class ExplosureRenderer : PostProcessEffectRenderer<Explosure>
    {
        private const string PROFILER_TAG = "K-Explosure";
        private Shader shader;

        #region Init
        public override void Init()
        {
            shader = Shader.Find("Hidden/K-PostProcessing/Explosure");
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
            sheet.properties.SetFloat("_Explosure", settings.SetExplosure);
            cmd.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
            cmd.EndSample(PROFILER_TAG);
        }
    }
}
