using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

namespace KPostProcessing
{
    [Serializable]
    [PostProcess(typeof(WhiteBalanceRenderer), PostProcessEvent.AfterStack, "KirkPostProcessing/WhiteBalance")]
    public sealed class WhiteBalance : PostProcessEffectSettings
    {
        #region Public Properties
        /// <summary>
        /// custom color temperature.
        /// </summary>
        [Range(-1f, 1f)]
        public FloatParameter temperature = new FloatParameter { value = 0f };

        /// <summary>
        /// for a green or magenta tint.
        /// </summary>
        [Range(-1f, 1f)]
        public FloatParameter tint = new FloatParameter { value = 0f };
        #endregion
    }
    public sealed class WhiteBalanceRenderer : PostProcessEffectRenderer<WhiteBalance>
    {
        private const string PROFILER_TAG = "K-WhiteBalance";
        private Shader shader;

        #region Init
        public override void Init()
        {
            shader = Shader.Find("Hidden/K-PostProcessing/WhiteBalance");
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

            sheet.properties.SetFloat("_Temperature", settings.temperature);
            sheet.properties.SetFloat("_Tint", settings.tint);

            context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
            cmd.EndSample(PROFILER_TAG);
        }
    }
}
