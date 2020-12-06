using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using UnityEditor.Rendering.PostProcessing;
using UnityEngine.Rendering.PostProcessing;

namespace KPostProcessing
{
    [PostProcessEditor(typeof(Bloom))]
    public class BloomEditor : PostProcessEffectEditor<Bloom>
    {
        SerializedParameterOverride _threshold;
        SerializedParameterOverride _softKnee;
        SerializedParameterOverride _radius;
        SerializedParameterOverride _intensity;
        SerializedParameterOverride _highQuality;
        SerializedParameterOverride _antiFlicker;
        readonly BloomGraphDrawer m_GraphDrawer = new BloomGraphDrawer();

        public override void OnEnable()
        {
            _threshold = FindParameterOverride(x => x._threshold);
            _softKnee = FindParameterOverride(x => x._softKnee);
            _radius = FindParameterOverride(x => x._radius);
            _intensity = FindParameterOverride(x => x._intensity);
            _highQuality = FindParameterOverride(x => x._highQuality);
            _antiFlicker = FindParameterOverride(x => x._antiFlicker);
        }

        public override string GetDisplayTitle()
        {
            return KPostProcessingEditorUtility.DISPLAY_TITLE_PREFIX + base.GetDisplayTitle();
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Bloom Settings");
            m_GraphDrawer.Prepare(_threshold.value.floatValue, _softKnee.value.floatValue, _intensity.value.floatValue);
            m_GraphDrawer.DrawGraph();
            PropertyField(_threshold);

            PropertyField(_softKnee);

            PropertyField(_radius);

            PropertyField(_intensity);

            PropertyField(_highQuality);

            PropertyField(_antiFlicker);
        }
    }
}
