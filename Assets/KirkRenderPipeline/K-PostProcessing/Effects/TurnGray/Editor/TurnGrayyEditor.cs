using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using UnityEditor.Rendering.PostProcessing;
using UnityEngine.Rendering.PostProcessing;

namespace KPostProcessing
{
    [PostProcessEditor(typeof(TurnGray))]
    public class TurnGrayyEditor : PostProcessEffectEditor<TurnGray>
    {
        SerializedParameterOverride Strength;

        public override void OnEnable()
        {
            Strength = FindParameterOverride(x => x.Strength);
        }
        public override string GetDisplayTitle()
        {
            return KPostProcessingEditorUtility.DISPLAY_TITLE_PREFIX + base.GetDisplayTitle();
        }

        public override void OnInspectorGUI()
        {
            EditorUtilities.DrawHeaderLabel("Strength Property");
            PropertyField(Strength);
        }
    }
}
