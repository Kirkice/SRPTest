using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using UnityEditor.Rendering.PostProcessing;
using UnityEngine.Rendering.PostProcessing;

namespace KPostProcessing
{
    [PostProcessEditor(typeof(BrightnessAndContrast))]
    public class BrightnessAndContrastEditor : PostProcessEffectEditor<BrightnessAndContrast>
    {
        SerializedParameterOverride brightness;
        SerializedParameterOverride contrast;

        public override void OnEnable()
        {
            brightness = FindParameterOverride(x => x.brightness);
            contrast = FindParameterOverride(x => x.contrast);
        }
        public override string GetDisplayTitle()
        {
            return KPostProcessingEditorUtility.DISPLAY_TITLE_PREFIX + base.GetDisplayTitle();
        }

        public override void OnInspectorGUI()
        {

            EditorUtilities.DrawHeaderLabel("Brightness Property");
            PropertyField(brightness);
            EditorUtilities.DrawHeaderLabel("Contrast Property");
            PropertyField(contrast);
        }
    }
}
