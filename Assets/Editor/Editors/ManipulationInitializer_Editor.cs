using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(ManipulationInitializer))]
public class ManipulationInitializer_Editor : Editor
{
    SerializedProperty containerProperty;

    ReorderableList objectOverrideList, vectorOverrideList;

    ManipulationInitializer component;

    private void OnEnable() {
        component = target as ManipulationInitializer;

        containerProperty = serializedObject.FindProperty("container");
        objectOverrideList = new ReorderableList(serializedObject, serializedObject.FindProperty("objectOverrideList"), true, true, true, true) {
            drawHeaderCallback = (Rect rect) => {
                EditorGUI.LabelField(rect, "Object Overrides");
            },
            drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
                rect.height = EditorGUIUtility.singleLineHeight;

                var property = objectOverrideList.serializedProperty.GetArrayElementAtIndex(index);

                GUI.enabled = false;
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("<Name>k__BackingField"), new GUIContent("Name"));
                GUI.enabled = true;

                rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("<Value>k__BackingField"), new GUIContent("Value"));
            },
            elementHeightCallback = (int index) => {
                return EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing * 2; // Unity somehow doesn't work correctly, force me to hardcoding the height
            },
            onAddDropdownCallback = (Rect buttonRect, ReorderableList list) => {
                var all = CollectObjectOverridableProperties();

                if (all.Any()) {
                    GenericMenu menu = new GenericMenu();

                    foreach (var overridable in all) {
                        string _overridable = overridable;
                        menu.AddItem(new GUIContent(_overridable), false, () => {
                            component.ObjectOverrideList.Add(new ObjectBindableProperty() {
                                Name = _overridable,
                            });
                        });
                    }

                    menu.ShowAsContext();
                } else {
                    Debug.LogWarning("No object overridable property found");
                }
            }
        };

        vectorOverrideList = new ReorderableList(serializedObject, serializedObject.FindProperty("vectorOverrideList"), true, true, true, true) {
            drawHeaderCallback = (Rect rect) => {
                EditorGUI.LabelField(rect, "Vector Overrides");
            },
            drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
                rect.height = EditorGUIUtility.singleLineHeight;

                var property = vectorOverrideList.serializedProperty.GetArrayElementAtIndex(index);

                GUI.enabled = false;
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("<Name>k__BackingField"), new GUIContent("Name"));
                GUI.enabled = true;

                rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;

                var valueProperty = property.FindPropertyRelative("<Value>k__BackingField");
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("<Value>k__BackingField"), new GUIContent("Value"));
            },
            elementHeightCallback = (int index) => {
                float h = 21;

                h += EditorGUI.GetPropertyHeight(vectorOverrideList.serializedProperty.GetArrayElementAtIndex(index).FindPropertyRelative("<Value>k__BackingField"));

                return h;
            },
            onAddDropdownCallback = (Rect buttonRect, ReorderableList list) => {
                var all = CollectVectorOverridableProperties();

                if (all.Any()) {
                    GenericMenu menu = new GenericMenu();

                    foreach (var overridable in all) {
                        string _overridable = overridable;
                        menu.AddItem(new GUIContent(_overridable), false, () => {
                            component.VectorOverrideList.Add(new MVectorBindableProperty() {
                                Name = _overridable
                            });
                        });
                    }

                    menu.ShowAsContext();
                } else {
                    Debug.LogWarning("No object overridable property found");
                }
            }
        };
    }

    private IEnumerable<string> CollectObjectOverridableProperties() {
        if (component.Container != null) {
            var list = component.Container.ObjectBindableProperties;

            for (int i = 0; i < list.Count; i++) {
                if (list[i].Overridable) {
                    yield return list[i].Name;
                }
            }
        }
    }

    private IEnumerable<string> CollectVectorOverridableProperties() {
        if (containerProperty.objectReferenceValue != null) {
            var list = component.Container.VectorBindableProperties;

            for (int i = 0; i < list.Count; i++) {
                if (list[i].Overridable) {
                    yield return list[i].Name;
                }
            }
        }
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();
        EditorGUI.BeginChangeCheck();

        //EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(containerProperty);
        //if (EditorGUI.EndChangeCheck()) {
        //    objectOverrideList.serializedProperty.ClearArray();
        //    vectorOverrideList.serializedProperty.ClearArray();
        //}

        if (containerProperty.objectReferenceValue != null) {
            objectOverrideList.DoLayoutList();
            vectorOverrideList.DoLayoutList();
        }

        if (EditorGUI.EndChangeCheck()) {
            serializedObject.ApplyModifiedProperties();
        }
    }
}
