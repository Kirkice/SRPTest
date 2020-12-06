using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

namespace KPostProcessing
{
    [Serializable]
    [PostProcess(typeof(MosaicRenderer), PostProcessEvent.AfterStack, "KirkPostProcessing/Mosaic")]
    public sealed class Mosaic : PostProcessEffectSettings
    {
        #region Public Properties
        [Range(0.01f, 1.0f)]
        public FloatParameter pixelSize = new FloatParameter { value = 0.5f };

        public BoolParameter useAutoScreenRatio = new BoolParameter { value = true };

        [Range(0.2f, 5.0f)]
        public FloatParameter pixelRatio = new FloatParameter { value = 1f };

        [Range(0.2f, 5.0f), Tooltip("像素缩放X")]
        public FloatParameter pixelScaleX = new FloatParameter { value = 1f };

        [Range(0.2f, 5.0f), Tooltip("像素缩放Y")]
        public FloatParameter pixelScaleY = new FloatParameter { value = 1f };
        #endregion
    }
    public sealed class MosaicRenderer : PostProcessEffectRenderer<Mosaic>
    {
        private const string PROFILER_TAG = "K-Mosaic";
        private Shader shader;

        #region Init
        public override void Init()
        {
            shader = Shader.Find("Hidden/K-PostProcessing/Mosaic");
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
            internal static readonly int Params = Shader.PropertyToID("_Params");
        }

        public override void Render(PostProcessRenderContext context)
        {
            CommandBuffer cmd = context.command;
            PropertySheet sheet = context.propertySheets.Get(shader);
            cmd.BeginSample(PROFILER_TAG);

            float size = (1.01f - settings.pixelSize) * 200f;
            sheet.properties.SetFloat("_PixelSize", size);


            float ratio = settings.pixelRatio;
            if (settings.useAutoScreenRatio)
            {
                ratio = (float)(context.width / (float)context.height);
                if (ratio == 0)
                {
                    ratio = 1f;
                }
            }

            sheet.properties.SetVector(ShaderIDs.Params, new Vector4(size, ratio, settings.pixelScaleX, settings.pixelScaleY));
            cmd.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
            cmd.EndSample(PROFILER_TAG);
        }
    }
}
