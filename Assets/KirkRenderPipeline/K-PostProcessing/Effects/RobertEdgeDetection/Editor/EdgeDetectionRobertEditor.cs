
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using UnityEditor.Rendering.PostProcessing;
using UnityEngine.Rendering.PostProcessing;

namespace KPostProcessing
{
    [PostProcessEditor(typeof(EdgeDetectionRobert))]
    public sealed class EdgeDetectionRobertEditor : PostProcessEffectEditor<EdgeDetectionRobert>
    {

        SerializedParameterOverride EdgeWidth;
        SerializedParameterOverride EdgeNeonFade;
        SerializedParameterOverride BackgroundFade;
        SerializedParameterOverride BackgroundColor;
        SerializedParameterOverride OutLineColor;

        public override void OnEnable()
        {
            EdgeWidth = FindParameterOverride(x => x.EdgeWidth);
            EdgeNeonFade = FindParameterOverride(x => x.EdgeNeonFade);
            BackgroundFade = FindParameterOverride(x => x.BackgroundFade);
            BackgroundColor = FindParameterOverride(x => x.BackgroundColor);
            OutLineColor = FindParameterOverride(x => x.OutLineColor);
        }

        public override string GetDisplayTitle()
        {
            return KPostProcessingEditorUtility.DISPLAY_TITLE_PREFIX + base.GetDisplayTitle();
        }

        public override void OnInspectorGUI()
        {
            EditorUtilities.DrawHeaderLabel("Edge Property");
            PropertyField(EdgeWidth);
            PropertyField(EdgeNeonFade);
            PropertyField(OutLineColor);

            EditorUtilities.DrawHeaderLabel("Background Property( For Edge Neon Fade <1 )");
            PropertyField(BackgroundFade);
            PropertyField(BackgroundColor);

        }
    }
}
        
