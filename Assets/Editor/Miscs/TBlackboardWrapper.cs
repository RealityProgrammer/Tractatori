using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;

public class TBlackboardWrapper
{
    public Blackboard Blackboard { get; private set; }
    public enum DisplayingSection {
        Vector, Object,
    }

    private DisplayingSection displaying;
    public DisplayingSection Displaying {
        get => displaying;
        set {
            if (displaying == value) return;

            displaying = value;
            onDisplayChangeCallback?.Invoke(value);

            Blackboard.Q("contentContainer").Clear();
            GenerateFields();
        }
    }

    public Action<DisplayingSection> onDisplayChangeCallback;

    public TractatoriGraphEditorWindow Window { get; private set; }

    public TBlackboardWrapper(TractatoriGraphView _graphView, TractatoriGraphEditorWindow _window) {
        Window = _window;

        Blackboard = new Blackboard(_graphView);
        Blackboard.title = "Properties";
        Blackboard.subTitle = string.Empty;
        Blackboard.scrollable = true;

        var cc = Blackboard.Q("unity-content-container");
        cc.style.flexDirection = FlexDirection.ColumnReverse;

        var button = new Button();
        button.clicked += () => {
            GenericMenu menu = new GenericMenu();

            var allValues = Enum.GetValues(typeof(DisplayingSection));
            foreach (var e in allValues) {
                menu.AddItem(new GUIContent(e.ToString()), (DisplayingSection)e == Displaying, () => {
                    Displaying = (DisplayingSection)e;
                    button.text = "Current Displaying: " + Displaying.ToString();
                });
            }

            menu.ShowAsContext();
        };
        button.text = "Current Displaying: " + Displaying.ToString();

        cc.Add(button);

        Blackboard.addItemRequested += (bb) => {
            switch (Displaying) {
                case DisplayingSection.Vector:
                    _window.CreateVectorBindableProperty(new MVectorBindableProperty() { Name = Guid.NewGuid().ToString().Replace('-', '\0').Substring(0, 13), Value = MVector.Zero4 });
                    break;

                case DisplayingSection.Object:
                    _window.CreateObjectBindableProperty(new ObjectBindableProperty() { Name = Guid.NewGuid().ToString().Replace('-', '\0').Substring(0, 13), Value = null });
                    break;
            }
        };
        Blackboard.editTextRequested += (Blackboard bb, VisualElement ve, string newValue) => {
            var old = ((BlackboardField)ve).text;

            switch (Displaying) {
                case DisplayingSection.Object: {
                    if (TractatoriGraphEditorWindow.CurrentEditingAsset.ObjectBindableProperties.Any(x => x.Name == newValue)) {
                        Debug.LogWarning("Binding Property with the name of \"" + newValue + "\" is already exist");
                        EditorApplication.Beep();
                        return;
                    }

                    var objProp = TractatoriGraphEditorWindow.CurrentEditingAsset.FindObjectBindableProperty(old);
                    if (objProp != null) {
                        objProp.Name = newValue;
                        ((BlackboardField)ve).text = newValue;
                    }
                    break;
                }

                case DisplayingSection.Vector: {
                    if (TractatoriGraphEditorWindow.CurrentEditingAsset.VectorBindableProperties.Any(x => x.Name == newValue)) {
                        Debug.LogWarning("Binding Property with the name of \"" + newValue + "\" is already exist");
                        EditorApplication.Beep();
                        return;
                    }

                    var objProp = TractatoriGraphEditorWindow.CurrentEditingAsset.FindVectorBindableProperty(old);
                    if (objProp != null) {
                        objProp.Name = newValue;
                        ((BlackboardField)ve).text = newValue;
                    }
                    break;
                }
            }
        };

        _graphView.Add(Blackboard);
    }

    private void ClearFieldDisplays() {
        Blackboard.Q("contentContainer").Clear();
    }

    public void GenerateFields() {
        switch (Displaying) {
            case DisplayingSection.Object:
                foreach (var objField in TractatoriGraphEditorWindow.CurrentEditingAsset.ObjectBindableProperties) {
                    Window.CreateFieldForBindableProperty(objField).Field.userData = TractatoriGraphEditorWindow.ObjectBindablePropertyUserData;
                }
                break;

            case DisplayingSection.Vector:
                foreach (var vectorField in TractatoriGraphEditorWindow.CurrentEditingAsset.VectorBindableProperties) {
                    Window.CreateFieldForBindableProperty(vectorField).Field.userData = TractatoriGraphEditorWindow.VectorBindablePropertyUserData;
                }
                break;
        }
    }
}
