using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using UnityEditor.Rendering.PostProcessing;
using UnityEngine.Rendering.PostProcessing;

namespace KPostProcessing
{
    [PostProcessEditor(typeof(Broken))]
    public sealed class BrokenEditor : PostProcessEffectEditor<Broken>
    {

        SerializedParameterOverride NormalTexture;
        SerializedParameterOverride NormalStrength;

        public override void OnEnable()
        {
            NormalTexture = FindParameterOverride(x => x.NormalTexture);
            NormalStrength = FindParameterOverride(x => x.NormalStrength);
        }

        public override string GetDisplayTitle()
        {
            return KPostProcessingEditorUtility.DISPLAY_TITLE_PREFIX + base.GetDisplayTitle();
        }

        public override void OnInspectorGUI()
        {
            PropertyField(NormalTexture);
            PropertyField(NormalStrength);
        }

    }
}