using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using UnityEditor.Rendering.PostProcessing;
using UnityEngine.Rendering.PostProcessing;

namespace KPostProcessing
{
    [PostProcessEditor(typeof(RGBSplit))]
    public sealed class RGBSplitEditor : PostProcessEffectEditor<RGBSplit>
    {
        SerializedParameterOverride splitFactor;
        public override void OnEnable()
        {
            splitFactor = FindParameterOverride(x => x.splitStrength);
        }

        public override string GetDisplayTitle()
        {
            return KPostProcessingEditorUtility.DISPLAY_TITLE_PREFIX + base.GetDisplayTitle();
        }

        public override void OnInspectorGUI()
        {
            EditorUtilities.DrawHeaderLabel("RGBSplit Factor");
            PropertyField(splitFactor);
        }
    }
}