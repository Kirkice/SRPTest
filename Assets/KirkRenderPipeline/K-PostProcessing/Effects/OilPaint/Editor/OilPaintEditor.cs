using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using UnityEditor.Rendering.PostProcessing;
using UnityEngine.Rendering.PostProcessing;

namespace KPostProcessing
{
    [PostProcessEditor(typeof(OilPaint))]
    public class OilPaintEditor : PostProcessEffectEditor<OilPaint>
    {

        SerializedParameterOverride Radius;
        SerializedParameterOverride ResolutionValue;

        public override void OnEnable()
        {
            Radius = FindParameterOverride(x => x.Radius);
            ResolutionValue = FindParameterOverride(x => x.ResolutionValue);
        }
        public override string GetDisplayTitle()
        {
            return KPostProcessingEditorUtility.DISPLAY_TITLE_PREFIX + base.GetDisplayTitle();
        }

        public override void OnInspectorGUI()
        {
            PropertyField(Radius);
            PropertyField(ResolutionValue);
        }
    }
}
