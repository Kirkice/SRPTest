using System;
using UnityEngine;

// This class implements simple ghosting type Motion Blur.
// If Extra Blur is selected, the scene will allways be a little blurred,
// as it is scaled to a smaller resolution.
// The effect works by accumulating the previous frames in an accumulation
// texture.
namespace KPostProcessing
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public class KirkPostProcessingMotionBlur : PostEffectBase
    {
        /// <summary>
        /// 公共变量
        /// </summary>
        #region Public Parames
        [Range(0.0f, 0.92f)]
        public float blurAmount = 0.8f;
        public bool extraBlur = false;
        #endregion

        /// <summary>
        /// 私有变量
        /// </summary>
        #region Private Parames
        private RenderTexture accumTexture;
        #endregion

        private void Start()
        {
            shader = Shader.Find("Hidden/K-PostProcessing/MotionBlur");
        }
        // Called by camera to apply image effect
        void OnRenderImage (RenderTexture source, RenderTexture destination)
        {
            if (accumTexture == null || accumTexture.width != source.width || accumTexture.height != source.height)
            {
                DestroyImmediate(accumTexture);
                accumTexture = new RenderTexture(source.width, source.height, 0);
                accumTexture.hideFlags = HideFlags.HideAndDontSave;
                Graphics.Blit( source, accumTexture );
            }

            if (extraBlur)
            {
                RenderTexture blurbuffer = RenderTexture.GetTemporary(source.width/4, source.height/4, 0);
                accumTexture.MarkRestoreExpected();
                Graphics.Blit(accumTexture, blurbuffer);
                Graphics.Blit(blurbuffer,accumTexture);
                RenderTexture.ReleaseTemporary(blurbuffer);
            }

            blurAmount = Mathf.Clamp( blurAmount, 0.0f, 0.92f );

            _Material.SetTexture("_MainTex", accumTexture);
            _Material.SetFloat("_AccumOrig", 1.0F-blurAmount);

            accumTexture.MarkRestoreExpected();

            Graphics.Blit (source, accumTexture, _Material);
            Graphics.Blit (accumTexture, destination);
        }

        /// <summary>
        /// 不启用
        /// </summary>
        #region OnDisable
        private void OnDisable()
        {
            DestroyImmediate(accumTexture);
        }
        #endregion
    }
}
