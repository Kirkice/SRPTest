﻿using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

namespace KPostProcessing
{
    [Serializable]
    [PostProcess(typeof(BokehBlurRenderer), PostProcessEvent.AfterStack, "KirkPostProcessing/BokehBlur")]
    public sealed class BokehBlur : PostProcessEffectSettings
    {
        #region Public Properties
        [Range(0f, 3f)]
        public FloatParameter BlurRadius = new FloatParameter { value = 1f };

        [Range(8, 128)]
        public IntParameter Iteration = new IntParameter { value = 32 };

        [Range(1, 10)]
        public FloatParameter RTDownScaling = new FloatParameter { value = 2 };
        #endregion
    }

    public sealed class BokehBlurRenderer : PostProcessEffectRenderer<BokehBlur>
    {

        private const string PROFILER_TAG = "K-BokehBlur";
        private Shader shader;
        private Vector4 mGoldenRot = new Vector4();

        public override void Init()
        {
            shader = Shader.Find("Hidden/K-PostProcessing/BokehBlur");
            float c = Mathf.Cos(2.39996323f);
            float s = Mathf.Sin(2.39996323f);
            mGoldenRot.Set(c, s, -s, c);
        }

        public override void Release()
        {
            base.Release();
        }

        static class ShaderIDs
        {
            internal static readonly int GoldenRot = Shader.PropertyToID("_GoldenRot");
            internal static readonly int Params = Shader.PropertyToID("_Params");
            internal static readonly int BufferRT1 = Shader.PropertyToID("_BufferRT1");
        }

        public override void Render(PostProcessRenderContext context)
        {
            CommandBuffer cmd = context.command;
            PropertySheet sheet = context.propertySheets.Get(shader);
            cmd.BeginSample(PROFILER_TAG);

            int RTWidth = (int)(context.screenWidth / settings.RTDownScaling);
            int RTHeight = (int)(context.screenHeight / settings.RTDownScaling);
            cmd.GetTemporaryRT(ShaderIDs.BufferRT1, RTWidth, RTHeight, 0, FilterMode.Bilinear);

            context.command.BlitFullscreenTriangle(context.source, ShaderIDs.BufferRT1);

            sheet.properties.SetVector(ShaderIDs.GoldenRot, mGoldenRot);
            sheet.properties.SetVector(ShaderIDs.Params, new Vector4(settings.Iteration, settings.BlurRadius, 1f / context.width, 1f / context.height));

            cmd.BlitFullscreenTriangle(ShaderIDs.BufferRT1, context.destination, sheet, 0);
            cmd.ReleaseTemporaryRT(ShaderIDs.BufferRT1);
            cmd.EndSample(PROFILER_TAG);
        }
    }
}