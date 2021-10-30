using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;

public abstract class BaseBindablePropertyDrawer : VisualElement
{
    public BaseBindableProperty Property { get; private set; }

    protected void CreateOverridableToggle() {
        Toggle toggle = new Toggle("Overridable");
        toggle.SetValueWithoutNotify(Property.Overridable);
        toggle.RegisterValueChangedCallback(OverridableModifyCallback);

        toggle.Q<Label>(className: "unity-text-element").style.flexGrow = 1;
        toggle.Q(className: "unity-base-field__input").style.flexGrow = 0;

        contentContainer.Add(toggle);
    }

    protected virtual void OverridableModifyCallback(ChangeEvent<bool> evt) {
        Property.Overridable = evt.newValue;
    }

    public abstract void Initialize();
}
