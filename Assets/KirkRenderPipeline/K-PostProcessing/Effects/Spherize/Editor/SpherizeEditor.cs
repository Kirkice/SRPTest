using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using UnityEditor.Rendering.PostProcessing;
using UnityEngine.Rendering.PostProcessing;

namespace KPostProcessing
{
    [PostProcessEditor(typeof(Spherize))]
    public class SpherizeEditor : PostProcessEffectEditor<Spherize>
    {
        SerializedParameterOverride Spherify;

        public override void OnEnable()
        {
            Spherify = FindParameterOverride(x => x.Spherify);
        }
        public override string GetDisplayTitle()
        {
            return KPostProcessingEditorUtility.DISPLAY_TITLE_PREFIX + base.GetDisplayTitle();
        }

        public override void OnInspectorGUI()
        {
            PropertyField(Spherify);
        }
    }
}
