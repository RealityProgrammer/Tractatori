using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ManipulationInitializer))]
public class ManipulationInitializer_Editor : Editor
{
    SerializedProperty containerProperty;

    private void OnEnable() {
        containerProperty = serializedObject.FindProperty("container");
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();
        EditorGUI.BeginChangeCheck();

        EditorGUILayout.PropertyField(containerProperty);
        if (containerProperty.objectReferenceValue != null) {

        }

        if (EditorGUI.EndChangeCheck()) {
            serializedObject.ApplyModifiedProperties();
        }
    }
}
