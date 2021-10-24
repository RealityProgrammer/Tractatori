using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[BindablePropertyDrawerOf(typeof(ObjectBindableProperty))]
public class ObjectBindablePropertyDrawer : BaseBindablePropertyDrawer {
    public override void Initialize(VisualElement container) {
        DoExposedField(container);

        var objectField = new ObjectField("Object");
        objectField.objectType = typeof(Object);
        objectField.allowSceneObjects = false;
        objectField.SetValueWithoutNotify(((ObjectBindableProperty)Parent.Property).Value);
        objectField.RegisterValueChangedCallback(ValueChangeCallback);

        ModifyLabel(objectField);

        objectField.Q<Label>(className: "unity-object-field-display__label").style.width = 0;

        container.Add(objectField);
    }

    void ValueChangeCallback(ChangeEvent<Object> evt) {
        ((ObjectBindableProperty)Parent.Property).Value = evt.newValue;
        Parent.Field.typeText = evt.newValue == null ? "Null" : evt.newValue.GetType().FullName;
    }
}
