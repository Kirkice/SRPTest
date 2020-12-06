using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using UnityEditor.Rendering.PostProcessing;
using UnityEngine.Rendering.PostProcessing;

namespace KPostProcessing
{
    [PostProcessEditor(typeof(Relief))]
    public class ReliefEditor : PostProcessEffectEditor<Relief>
    {

        SerializedParameterOverride Color;

        public override void OnEnable()
        {
            Color = FindParameterOverride(x => x.Color);
        }
        public override string GetDisplayTitle()
        {
            return KPostProcessingEditorUtility.DISPLAY_TITLE_PREFIX + base.GetDisplayTitle();
        }

        public override void OnInspectorGUI()
        {
            PropertyField(Color);
        }
    }
}
