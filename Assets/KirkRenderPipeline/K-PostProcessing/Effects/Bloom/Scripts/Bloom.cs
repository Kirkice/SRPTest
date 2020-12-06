using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

namespace KPostProcessing
{
    [Serializable]
    [PostProcess(typeof(BloomRenderer), PostProcessEvent.AfterStack, "KirkPostProcessing/Bloom")]
    public class Bloom : PostProcessEffectSettings
    {
        #region Public Properties
        public FloatParameter thresholdGamma
        {
            get { return new FloatParameter { value = Mathf.Max(_threshold, 0) }; }
            set { _threshold = value; }
        }

        public FloatParameter thresholdLinear
        {
            get { return new FloatParameter { value = GammaToLinear(thresholdGamma) }; }
            set { _threshold = new FloatParameter { value = LinearToGamma(value) }; }
        }
        [SerializeField, Range(0, 10)]
        [Tooltip("Filters out pixels under this level of brightness.")]
        public FloatParameter _threshold = new FloatParameter { value = 0.8f };
        public FloatParameter softKnee
        {
            get { return _softKnee; }
            set { _softKnee = value; }
        }
        [SerializeField, Range(0, 1)]
        [Tooltip("Makes transition between under/over-threshold gradual.")]
        public FloatParameter _softKnee = new FloatParameter { value = 0.5f };
        [SerializeField, Range(1, 7)]
        [Tooltip("Changes extent of veiling effects\n" +  "in a screen resolution-independent fashion.")]
        public FloatParameter _radius = new FloatParameter { value = 2.5f };
        public FloatParameter intensity
        {
            get { return new FloatParameter { value = Mathf.Max(_intensity, 0) }; }
            set { _intensity = value; }
        }
        [SerializeField]
        [Tooltip("Blend factor of the result image.")]
        public FloatParameter _intensity = new FloatParameter { value = 0.8f };
        [SerializeField]
        [Tooltip("Controls filter quality and buffer resolution.")]
        public BoolParameter _highQuality = new BoolParameter { value = true };
        [SerializeField]
        [Tooltip("Reduces flashing noise with an additional filter.")]
        public BoolParameter _antiFlicker = new BoolParameter { value = true };
        #endregion

        #region Private Members
        float LinearToGamma(float x)
        {
        #if UNITY_5_3_OR_NEWER
            return Mathf.LinearToGammaSpace(x);
        #else
            if (x <= 0.0031308f)
                return 12.92f * x;
            else
                return 1.055f * Mathf.Pow(x, 1 / 2.4f) - 0.055f;
        #endif
        }

        float GammaToLinear(float x)
        {
        #if UNITY_5_3_OR_NEWER
            return Mathf.GammaToLinearSpace(x);
        #else
            if (x <= 0.04045f)
                return x / 12.92f;
            else
                return Mathf.Pow((x + 0.055f) / 1.055f, 2.4f);
        #endif
        }

        #endregion
    }

    public sealed class BloomRenderer : PostProcessEffectRenderer<Bloom>
    {
        private const string PROFILER_TAG = "K-Bloom";
        private Shader shader;
        const int kMaxIterations = 16;
        RenderTexture[] _blurBuffer1 = new RenderTexture[kMaxIterations];
        RenderTexture[] _blurBuffer2 = new RenderTexture[kMaxIterations];

        #region Init
        public override void Init()
        {
            shader = Shader.Find("Hidden/K-PostProcessing/Bloom");
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
            BloomRender(context, sheet, cmd);
            cmd.EndSample(PROFILER_TAG);
        }

        void BloomRender(PostProcessRenderContext context, PropertySheet sheet,CommandBuffer cmd)
        {
            var useRGBM = Application.isMobilePlatform;

            // source texture size
            var tw = context.width;
            var th = context.height;

            // halve the texture size for the low quality mode
            if (!settings._highQuality)
            {
                tw /= 2;
                th /= 2;
            }

            // blur buffer format
            var rtFormat = useRGBM ?
                RenderTextureFormat.Default : RenderTextureFormat.DefaultHDR;

            // determine the iteration count
            var logh = Mathf.Log(th, 2) + settings._radius - 8;
            var logh_i = (int)logh;
            var iterations = Mathf.Clamp(logh_i, 1, kMaxIterations);

            // update the shader properties
            var lthresh = settings.thresholdLinear;
            sheet.properties.SetFloat("_Threshold", lthresh);

            var knee = lthresh * settings._softKnee + 1e-5f;
            var curve = new Vector3(lthresh - knee, knee * 2, 0.25f / knee);
            sheet.properties.SetVector("_Curve", curve);

            var pfo = !settings._highQuality && settings._antiFlicker;
            sheet.properties.SetFloat("_PrefilterOffs", pfo ? -0.5f : 0.0f);

            sheet.properties.SetFloat("_SampleScale", 0.5f + logh - logh_i);
            sheet.properties.SetFloat("_Intensity", settings.intensity);

            // prefilter pass
            var prefiltered = RenderTexture.GetTemporary(tw, th, 0, rtFormat);
            var pass = settings._antiFlicker ? 1 : 0;
            cmd.BlitFullscreenTriangle(context.source, prefiltered, sheet, pass);

            // construct a mip pyramid
            var last = prefiltered;
            for (var level = 0; level < iterations; level++)
            {
                _blurBuffer1[level] = RenderTexture.GetTemporary(
                    last.width / 2, last.height / 2, 0, rtFormat
                );

                pass = (level == 0) ? (settings._antiFlicker ? 3 : 2) : 4;
                cmd.BlitFullscreenTriangle(last, _blurBuffer1[level], sheet, pass);
                last = _blurBuffer1[level];
            }

            // upsample and combine loop
            for (var level = iterations - 2; level >= 0; level--)
            {
                var basetex = _blurBuffer1[level];
                sheet.properties.SetTexture("_BaseTex", basetex);

                _blurBuffer2[level] = RenderTexture.GetTemporary(
                    basetex.width, basetex.height, 0, rtFormat
                );

                pass = settings._highQuality ? 6 : 5;
                cmd.BlitFullscreenTriangle(last, _blurBuffer2[level], sheet, pass);
                last = _blurBuffer2[level];
            }
            RenderTexture renderTexture = RenderTexture.GetTemporary(context.width, context.height, 0, RenderTextureFormat.DefaultHDR);
            cmd.SetRenderTarget(context.source, renderTexture);
            // finish process
            sheet.properties.SetTexture("_BaseTex", renderTexture);
            pass = settings._highQuality ? 8 : 7;
            cmd.BlitFullscreenTriangle(last, context.destination, sheet, pass);

            // release the temporary buffers
            for (var i = 0; i < kMaxIterations; i++)
            {
                if (_blurBuffer1[i] != null)
                    RenderTexture.ReleaseTemporary(_blurBuffer1[i]);

                if (_blurBuffer2[i] != null)
                    RenderTexture.ReleaseTemporary(_blurBuffer2[i]);

                _blurBuffer1[i] = null;
                _blurBuffer2[i] = null;
            }
            RenderTexture.ReleaseTemporary(prefiltered);
            RenderTexture.ReleaseTemporary(renderTexture);
        }
    }
}
