using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects, CustomEditor(typeof(KirkPostProcessingDepthOfField))]
public class DepthOfFieldEditor : Editor
{
    SerializedProperty _pointOfFocus;
    SerializedProperty _focusDistance;
    SerializedProperty _fNumber;
    SerializedProperty _useCameraFov;
    SerializedProperty _focalLength;
    SerializedProperty _kernelSize;
    SerializedProperty _visualize;

    static GUIContent _labelPointOfFocus = new GUIContent(
        "Point Of Focus",
        "Transform that represents the point of focus."
    );

    static GUIContent _labelFocusDistance = new GUIContent(
        "Distance",
        "Distance to the point of focus (only used when none is specified in PointOfFocus)."
    );

    static GUIContent _labelFNumber = new GUIContent(
        "Aperture (f-stop)",
        "Ratio of aperture (known as f-stop or f-number). The smaller the value is, the shallower the depth of field is."
    );

    static GUIContent _labelUseCameraFov = new GUIContent(
        "Use Camera FOV",
        "Calculate the focal length from the field-of-view value."
    );

    static GUIContent _labelFocalLength = new GUIContent(
        "Focal Length (mm)",
        "Distance between the lens and the film. The larger the value is, the shallower the depth of field is."
    );

    static GUIContent _labelKernelSize = new GUIContent(
        "Kernel Size",
        "Convolution kernel size of the bokeh filter, which determines the maximum radius of bokeh. It also affects the performance (the larger the kernel is, the longer the GPU time is required)."
    );

    static GUIContent _labelVisualize = new GUIContent(
        "Visualize",
        "Visualize the depths as red (focused), green (far) or blue (near)."
    );

    void OnEnable()
    {
        _pointOfFocus = serializedObject.FindProperty("_pointOfFocus");
        _focusDistance = serializedObject.FindProperty("_focusDistance");
        _fNumber = serializedObject.FindProperty("_fNumber");
        _useCameraFov = serializedObject.FindProperty("_useCameraFov");
        _focalLength = serializedObject.FindProperty("_focalLength");
        _kernelSize = serializedObject.FindProperty("_kernelSize");
        _visualize = serializedObject.FindProperty("_visualize");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Point of focus
        EditorGUILayout.PropertyField(_pointOfFocus, _labelPointOfFocus);
        if (_pointOfFocus.hasMultipleDifferentValues || _pointOfFocus.objectReferenceValue == null)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_focusDistance, _labelFocusDistance);
            EditorGUI.indentLevel--;
        }

        // Aperture
        EditorGUILayout.PropertyField(_fNumber, _labelFNumber);

        // Focal Length
        EditorGUILayout.PropertyField(_useCameraFov, _labelUseCameraFov);

        if (_useCameraFov.hasMultipleDifferentValues || !_useCameraFov.boolValue)
        {
            if (_focalLength.hasMultipleDifferentValues)
            {
                EditorGUILayout.PropertyField(_focalLength);
            }
            else
            {
                EditorGUI.BeginChangeCheck();

                var f = _focalLength.floatValue * 1000;
                f = EditorGUILayout.Slider(_labelFocalLength, f, 10.0f, 300.0f);

                if (EditorGUI.EndChangeCheck())
                    _focalLength.floatValue = f / 1000;
            }
        }

        // Kernel Size
        EditorGUILayout.PropertyField(_kernelSize, _labelKernelSize);

        // Visualize
        EditorGUILayout.PropertyField(_visualize, _labelVisualize);

        serializedObject.ApplyModifiedProperties();
    }
}
