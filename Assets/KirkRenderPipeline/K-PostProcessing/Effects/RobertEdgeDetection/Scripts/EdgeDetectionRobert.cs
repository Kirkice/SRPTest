using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;


namespace KPostProcessing
{

    [Serializable]
    [PostProcess(typeof(EdgeDetectionRobert2Renderer), PostProcessEvent.AfterStack, "KirkPostProcessing/EdgeDetectionRobert")]
    public class EdgeDetectionRobert : PostProcessEffectSettings
    {
        [Range(0.05f, 5.0f)]
        public FloatParameter EdgeWidth = new FloatParameter { value = 1f };

        [Range(0.1f, 1.0f)]
        public FloatParameter EdgeNeonFade = new FloatParameter { value = 1f };

        [Range(0.0f, 1.0f)]
        public FloatParameter BackgroundFade = new FloatParameter { value = 0f };

        [ColorUsageAttribute(true, true, 0f, 20f, 0.125f, 3f)]
        public ColorParameter BackgroundColor = new ColorParameter { value = new Color(0.0f, 0.0f, 0.0f, 1.0f) };

        [ColorUsageAttribute(true, true, 0f, 20f, 0.125f, 3f)]
        public ColorParameter OutLineColor = new ColorParameter { value = new Color(1.0f, 1.0f, 0.0f, 1.0f) };
    }
    public sealed class EdgeDetectionRobert2Renderer : PostProcessEffectRenderer<EdgeDetectionRobert>
    {

        private const string PROFILER_TAG = "K-EdgeDetectionRobert";
        private Shader shader;


        public override void Init()
        {
            shader = Shader.Find("Hidden/K-PostProcessing/EdgeDetectionRobert");
        }

        public override void Release()
        {
            base.Release();
        }

        static class ShaderIDs
        {
            internal static readonly int Params = Shader.PropertyToID("_Params");
            internal static readonly int BackgroundColor = Shader.PropertyToID("_BackgroundColor");
            internal static readonly int OutLineColor = Shader.PropertyToID("_OutLineColor");
        }

        public override void Render(PostProcessRenderContext context)
        {

            CommandBuffer cmd = context.command;
            PropertySheet sheet = context.propertySheets.Get(shader);
            cmd.BeginSample(PROFILER_TAG);

            sheet.properties.SetVector(ShaderIDs.Params, new Vector4(settings.EdgeWidth, settings.EdgeNeonFade, 0, settings.BackgroundFade));
            sheet.properties.SetColor(ShaderIDs.BackgroundColor, settings.BackgroundColor);
            sheet.properties.SetColor(ShaderIDs.OutLineColor, settings.OutLineColor);

            cmd.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
            cmd.EndSample(PROFILER_TAG);
        }
    }
}
        
