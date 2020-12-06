using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using UnityEditor.Rendering.PostProcessing;
using UnityEngine.Rendering.PostProcessing;

namespace KPostProcessing
{
    [PostProcessEditor(typeof(Explosure))]
    public class ExplosureEditor : PostProcessEffectEditor<Explosure>
    {

        SerializedParameterOverride SetExplosure;

        public override void OnEnable()
        {
            SetExplosure = FindParameterOverride(x => x.SetExplosure);
        }
        public override string GetDisplayTitle()
        {
            return KPostProcessingEditorUtility.DISPLAY_TITLE_PREFIX + base.GetDisplayTitle();
        }

        public override void OnInspectorGUI()
        {
            PropertyField(SetExplosure);
        }
    }
}
