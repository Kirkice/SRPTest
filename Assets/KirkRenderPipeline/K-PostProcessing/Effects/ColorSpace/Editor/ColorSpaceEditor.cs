using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using UnityEditor.Rendering.PostProcessing;
using UnityEngine.Rendering.PostProcessing;

namespace KPostProcessing
{
    [PostProcessEditor(typeof(ColorSpace))]
    public class ColorSpaceEditor : PostProcessEffectEditor<ColorSpace>
    {
        SerializedParameterOverride power;

        public override void OnEnable()
        {
            power = FindParameterOverride(x => x.power);
        }
        public override string GetDisplayTitle()
        {
            return KPostProcessingEditorUtility.DISPLAY_TITLE_PREFIX + base.GetDisplayTitle();
        }

        public override void OnInspectorGUI()
        {
            PropertyField(power);
        }
    }
}
