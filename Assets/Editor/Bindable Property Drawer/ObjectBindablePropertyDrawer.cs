using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[BindablePropertyDrawerOf(typeof(ObjectBindableProperty))]
public class ObjectBindablePropertyDrawer : BaseBindablePropertyDrawer {
    public override void Initialize() {
        var nameLabel = new Label(Property.Name);
        nameLabel.style.marginLeft = 3;
        nameLabel.style.unityFontStyleAndWeight = FontStyle.Bold;

        contentContainer.Add(nameLabel);

        var nameField = new TextField("Name");
        nameField.SetValueWithoutNotify(Property.Name);
        nameField.isDelayed = true;
        nameField.RegisterValueChangedCallback((evt) => {
            if (string.IsNullOrEmpty(evt.newValue)) {
                EditorApplication.Beep();
                nameField.SetValueWithoutNotify(evt.previousValue);
                return;
            }

            if (TractatoriGraphEditorWindow.CurrentEditingAsset.FindObjectBindableProperty(evt.newValue) == null) {
                Property.Name = evt.newValue;
                nameLabel.text = evt.newValue;
            } else {
                nameField.SetValueWithoutNotify(evt.previousValue);
                EditorApplication.Beep();
            }
        });

        contentContainer.Add(nameField);

        CreateOverridableToggle();

        var objectField = new ObjectField("Object");
        objectField.objectType = typeof(Object);
        objectField.allowSceneObjects = false;
        objectField.SetValueWithoutNotify(((ObjectBindableProperty)Property).Value);
        objectField.RegisterValueChangedCallback(ValueChangeCallback);

        //ModifyLabel(objectField);

        // objectField.Q<Label>(className: "unity-object-field-display__label").style.width = 0;

        contentContainer.Add(objectField);
    }

    void ValueChangeCallback(ChangeEvent<Object> evt) {
        ((ObjectBindableProperty)Property).Value = evt.newValue;
    }
}
