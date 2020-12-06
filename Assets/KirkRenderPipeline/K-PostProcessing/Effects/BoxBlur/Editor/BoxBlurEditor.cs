using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using UnityEditor.Rendering.PostProcessing;
using UnityEngine.Rendering.PostProcessing;

namespace KPostProcessing
{
    [PostProcessEditor(typeof(BoxBlur))]
    public sealed class BoxBlurEditor : PostProcessEffectEditor<BoxBlur>
    {

        SerializedParameterOverride BlurRadius;
        SerializedParameterOverride Iteration;
        SerializedParameterOverride RTDownScaling;

        public override void OnEnable()
        {
            BlurRadius = FindParameterOverride(x => x.BlurRadius);
            Iteration = FindParameterOverride(x => x.Iteration);
            RTDownScaling = FindParameterOverride(x => x.RTDownScaling);
        }

        public override string GetDisplayTitle()
        {
            return KPostProcessingEditorUtility.DISPLAY_TITLE_PREFIX + base.GetDisplayTitle();
        }

        public override void OnInspectorGUI()
        {
            PropertyField(BlurRadius);
            PropertyField(Iteration);
            PropertyField(RTDownScaling);
        }

    }
}