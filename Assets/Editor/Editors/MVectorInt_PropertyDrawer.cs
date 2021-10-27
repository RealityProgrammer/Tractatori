using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(MVectorInt))]
public class MVectorInt_PropertyDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        position.height = EditorGUIUtility.singleLineHeight;

        if (property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label)) {
            position.y += position.height + EditorGUIUtility.standardVerticalSpacing;

            var axisCountProperty = property.FindPropertyRelative("<" + nameof(MVector.Axis) + ">k__BackingField");

            EditorGUI.PropertyField(position, axisCountProperty, new GUIContent("Axis Count"));
            axisCountProperty.intValue = Mathf.Clamp(axisCountProperty.intValue, 1, 4);

            int axisCount = axisCountProperty.intValue;

            position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("<X>k__BackingField"), new GUIContent("X"));

            var yProperty = property.FindPropertyRelative("<Y>k__BackingField");
            var zProperty = property.FindPropertyRelative("<Z>k__BackingField");
            var wProperty = property.FindPropertyRelative("<W>k__BackingField");

            if (axisCount >= 2) {
                position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(position, yProperty, new GUIContent("Y"));
            }

            if (axisCount >= 3) {
                position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(position, zProperty, new GUIContent("Z"));
            }

            if (axisCount >= 4) {
                position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(position, wProperty, new GUIContent("W"));
            }

            switch (axisCount) {
                case 1:
                    yProperty.intValue = 0;
                    zProperty.intValue = 0;
                    wProperty.intValue = 0;
                    break;

                case 2:
                    zProperty.intValue = 0;
                    wProperty.intValue = 0;
                    break;

                case 3:
                    wProperty.intValue = 0;
                    break;
            }
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        if (!property.isExpanded) {
            return 18;
        }

        var axisCount = property.FindPropertyRelative("<" + nameof(MVector.Axis) + ">k__BackingField").intValue;

        return EditorGUIUtility.singleLineHeight * (axisCount + 2) + EditorGUIUtility.standardVerticalSpacing * (axisCount + 1);
    }
}
