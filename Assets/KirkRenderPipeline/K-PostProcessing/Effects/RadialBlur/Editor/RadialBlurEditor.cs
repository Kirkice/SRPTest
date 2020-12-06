using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using UnityEditor.Rendering.PostProcessing;
using UnityEngine.Rendering.PostProcessing;

namespace KPostProcessing
{
    [PostProcessEditor(typeof(RadialBlur))]
    public class RadialBlurEditor : PostProcessEffectEditor<RadialBlur>
    {
        SerializedParameterOverride blurFactor;
        SerializedParameterOverride blurCount;
        SerializedParameterOverride blurCenter;
        SerializedParameterOverride showPreview;

        public override void OnEnable()
        {
            blurFactor = FindParameterOverride(x => x.blurFactor);
            blurCenter = FindParameterOverride(x => x.blurCenter);
            showPreview = FindParameterOverride(x => x.showPreview);
            blurCount = FindParameterOverride(x => x.blurCount);
        }

        public override string GetDisplayTitle()
        {
            return KPostProcessingEditorUtility.DISPLAY_TITLE_PREFIX + base.GetDisplayTitle();
        }

        public override void OnInspectorGUI()
        {

            EditorUtilities.DrawHeaderLabel("BlurFactor Property");
            PropertyField(blurFactor);
            EditorUtilities.DrawHeaderLabel("BlurCount Property");
            PropertyField(blurCount);

            EditorUtilities.DrawHeaderLabel("BlurCenter Property");
            PropertyField(blurCenter);

            EditorUtilities.DrawHeaderLabel("Debug");
            PropertyField(showPreview);

        }
    }
}
