using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using UnityEditor.Rendering.PostProcessing;
using UnityEngine.Rendering.PostProcessing;

namespace KPostProcessing
{
    [PostProcessEditor(typeof(Sharpen))]
    public class SharpenEditor : PostProcessEffectEditor<Sharpen>
    {
        SerializedParameterOverride Strength;
        SerializedParameterOverride Threshold;

        public override void OnEnable()
        {
            Strength = FindParameterOverride(x => x.Strength);
            Threshold = FindParameterOverride(x => x.Threshold);
        }
        public override string GetDisplayTitle()
        {
            return KPostProcessingEditorUtility.DISPLAY_TITLE_PREFIX + base.GetDisplayTitle();
        }

        public override void OnInspectorGUI()
        {
            PropertyField(Strength);
            PropertyField(Threshold);
        }
    }
}
