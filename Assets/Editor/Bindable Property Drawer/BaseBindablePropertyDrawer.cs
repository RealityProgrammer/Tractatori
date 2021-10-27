using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;

public abstract class BaseBindablePropertyDrawer
{
    public PropertyBindingField Parent { get; private set; }

    protected void DoExposedField(VisualElement container) {
        Toggle toggle = new Toggle("Overridable");
        toggle.SetValueWithoutNotify(Parent.Property.Overridable);
        toggle.RegisterValueChangedCallback(ExposedModifyCallback);

        toggle.Q<Label>(className: "unity-text-element").style.flexGrow = 1;
        toggle.Q(className: "unity-base-field__input").style.flexGrow = 0;

        container.Add(toggle);
    }

    protected virtual void ExposedModifyCallback(ChangeEvent<bool> evt) {
        Parent.Property.Overridable = evt.newValue;
    }

    protected void ModifyLabel(VisualElement element, string labelClass = "unity-label", float width = 20) {
        var label = element.Q<Label>(className: labelClass).style;
        label.width = new Length(width, LengthUnit.Percent);
        label.minWidth = StyleKeyword.Auto;
    }

    public abstract void Initialize(VisualElement container);
}
