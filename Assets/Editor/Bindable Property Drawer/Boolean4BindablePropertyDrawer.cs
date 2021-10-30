using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[BindablePropertyDrawerOf(typeof(BooleanBindableProperty))]
public class Boolean4BindablePropertyDrawer : BaseBindablePropertyDrawer {
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

            Property.Name = evt.newValue;
            nameLabel.text = evt.newValue;
        });

        contentContainer.Add(nameField);

        CreateOverridableToggle();

        var boolean = ((BooleanBindableProperty)Property).Value;

        var toggleContainer = new VisualElement();
        toggleContainer.style.flexDirection = FlexDirection.Row;

        var label = new Label("Value");
        label.style.flexGrow = 1;
        label.style.marginLeft = 3;
        toggleContainer.Add(label);

        for (int i = 0; i < 4; i++) {
            var toggle = new Toggle();
            toggle.SetValueWithoutNotify(boolean[i]);

            int _i = i;
            toggle.RegisterValueChangedCallback((evt) => {
                var old = ((BooleanBindableProperty)Property).Value;
                old[_i] = evt.newValue;

                ((BooleanBindableProperty)Property).Value = old;
            });

            toggleContainer.Add(toggle);
        }

        contentContainer.Add(toggleContainer);
    }
}
