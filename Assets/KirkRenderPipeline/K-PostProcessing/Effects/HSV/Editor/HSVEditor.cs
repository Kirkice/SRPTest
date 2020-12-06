using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using UnityEditor.Rendering.PostProcessing;
using UnityEngine.Rendering.PostProcessing;

namespace KPostProcessing
{
    [PostProcessEditor(typeof(HSV))]
    public class HSVEditor : PostProcessEffectEditor<HSV>
    {
        SerializedParameterOverride Hue;
        SerializedParameterOverride Saturation;
        SerializedParameterOverride Value;

        public override void OnEnable()
        {
            Hue = FindParameterOverride(x => x.Hue);
            Saturation = FindParameterOverride(x => x.Saturation);
            Value = FindParameterOverride(x => x.Value);
        }
        public override string GetDisplayTitle()
        {
            return KPostProcessingEditorUtility.DISPLAY_TITLE_PREFIX + base.GetDisplayTitle();
        }

        public override void OnInspectorGUI()
        {
            EditorUtilities.DrawHeaderLabel("Hue Property");
            PropertyField(Hue);
            EditorUtilities.DrawHeaderLabel("Saturation Property");
            PropertyField(Saturation);
            EditorUtilities.DrawHeaderLabel("Value Property");
            PropertyField(Value);
        }
    }
}
