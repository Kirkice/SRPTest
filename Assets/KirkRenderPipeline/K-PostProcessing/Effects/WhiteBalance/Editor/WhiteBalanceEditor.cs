using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using UnityEditor.Rendering.PostProcessing;
using UnityEngine.Rendering.PostProcessing;

namespace KPostProcessing
{
    [PostProcessEditor(typeof(WhiteBalance))]
    public class WhiteBalanceEditor : PostProcessEffectEditor<WhiteBalance>
    {

        SerializedParameterOverride temperature;
        SerializedParameterOverride tint;


        public override void OnEnable()
        {
            temperature = FindParameterOverride(x => x.temperature);
            tint = FindParameterOverride(x => x.tint);
        }

        public override string GetDisplayTitle()
        {
            return KPostProcessingEditorUtility.DISPLAY_TITLE_PREFIX + base.GetDisplayTitle();
        }

        public override void OnInspectorGUI()
        {
            PropertyField(temperature);
            PropertyField(tint);
        }
    }
}
