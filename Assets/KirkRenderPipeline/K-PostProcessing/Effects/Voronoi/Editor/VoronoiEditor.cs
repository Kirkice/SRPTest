using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using UnityEditor.Rendering.PostProcessing;
using UnityEngine.Rendering.PostProcessing;

namespace KPostProcessing
{
    [PostProcessEditor(typeof(Voronoi))]
    public class VoronoiEditor : PostProcessEffectEditor<Voronoi>
    {
        SerializedParameterOverride BlurRadius;
        SerializedParameterOverride Iteration;
        SerializedParameterOverride RTDownScaling;
        SerializedParameterOverride Tilling;

        public override void OnEnable()
        {
            BlurRadius = FindParameterOverride(x => x.BlurRadius);
            Iteration = FindParameterOverride(x => x.Iteration);
            RTDownScaling = FindParameterOverride(x => x.RTDownScaling);
            Tilling = FindParameterOverride(x => x.Tilling);
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
            PropertyField(Tilling);
        }
    }
}
