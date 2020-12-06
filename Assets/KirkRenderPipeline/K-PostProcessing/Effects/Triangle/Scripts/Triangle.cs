using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

namespace KPostProcessing
{
    [Serializable]
    [PostProcess(typeof(TriangleRenderer), PostProcessEvent.AfterStack, "KirkPostProcessing/Triangle")]
    public sealed class Triangle : PostProcessEffectSettings
    {
        [Range(0.001f, 1.0f)]
        public FloatParameter pixelSize = new FloatParameter { value = 0.5f };

        public BoolParameter useAutoScreenRatio = new BoolParameter { value = true };

        [Range(0.2f, 5.0f)]
        public FloatParameter pixelRatio = new FloatParameter { value = 1f };

        [Range(0.2f, 5.0f), Tooltip("像素缩放X")]
        public FloatParameter pixelScaleX = new FloatParameter { value = 1f };

        [Range(0.2f, 5.0f), Tooltip("像素缩放Y")]
        public FloatParameter pixelScaleY = new FloatParameter { value = 1f }; public FloatParameter SetExplosure = new FloatParameter { value = 1.0f };
    }
    public sealed class TriangleRenderer : PostProcessEffectRenderer<Triangle>
    {
        private const string PROFILER_TAG = "K-Triangle";
        private Shader shader;

        #region Init
        public override void Init()
        {
            shader = Shader.Find("Hidden/K-PostProcessing/Triangle");
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

            float size = (1.01f - settings.pixelSize) * 5f;

            float ratio = settings.pixelRatio;
            if (settings.useAutoScreenRatio)
            {
                ratio = (float)(context.width / (float)context.height);
                if (ratio == 0)
                {
                    ratio = 1f;
                }
            }

            sheet.properties.SetVector(ShaderIDs.Params, new Vector4(size, ratio, settings.pixelScaleX * 20, settings.pixelScaleY * 20));

            cmd.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
            cmd.EndSample(PROFILER_TAG);
        }
    }
}
