using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(MVector))]
public class MVector_PropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        position.height = EditorGUIUtility.singleLineHeight;

        if (property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label)) {
            position.y += position.height + EditorGUIUtility.standardVerticalSpacing;

            var axisCountProperty = property.FindPropertyRelative("<" + nameof(MVector.Axis) + ">k__BackingField");
            var vectorProperty = property.FindPropertyRelative("<" + nameof(MVector.Vector) + ">k__BackingField");

            EditorGUI.PropertyField(position, axisCountProperty, new GUIContent("Axis Count"));
            axisCountProperty.intValue = Mathf.Clamp(axisCountProperty.intValue, 1, 4);

            int axisCount = axisCountProperty.intValue;

            position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(position, vectorProperty.FindPropertyRelative("x"));

            var yProperty = vectorProperty.FindPropertyRelative("y");
            var zProperty = vectorProperty.FindPropertyRelative("z");
            var wProperty = vectorProperty.FindPropertyRelative("w");

            if (axisCount >= 2) {
                position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(position, yProperty);
            }

            if (axisCount >= 3) {
                position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(position, zProperty);
            }

            if (axisCount >= 4) {
                position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(position, wProperty);
            }

            switch (axisCount) {
                case 1:
                    yProperty.floatValue = 0;
                    zProperty.floatValue = 0;
                    wProperty.floatValue = 0;
                    break;

                case 2:
                    zProperty.floatValue = 0;
                    wProperty.floatValue = 0;
                    break;

                case 3:
                    wProperty.floatValue = 0;
                    break;
            }
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        if (!property.isExpanded) {
            return 18;
        }

        var axisCount = property.FindPropertyRelative("<" + nameof(MVector.Axis) + ">k__BackingField").intValue;

        return EditorGUIUtility.singleLineHeight * (axisCount + 2) + EditorGUIUtility.standardVerticalSpacing * (axisCount + 2);
    }
}
