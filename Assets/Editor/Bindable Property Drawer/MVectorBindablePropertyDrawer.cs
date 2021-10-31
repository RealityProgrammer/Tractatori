using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[BindablePropertyDrawerOf(typeof(MVectorBindableProperty))]
public class MVectorBindablePropertyDrawer : BaseBindablePropertyDrawer {
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

            if (TractatoriGraphEditorWindow.CurrentEditingAsset.FindVectorBindableProperty(evt.newValue) == null) {
                Property.Name = evt.newValue;
                nameLabel.text = evt.newValue;
            } else {
                nameField.SetValueWithoutNotify(evt.previousValue);
                EditorApplication.Beep();
            }
        });

        contentContainer.Add(nameField);

        CreateOverridableToggle();

        var vector = ((MVectorBindableProperty)Property).Value;

        var x = new FloatField("X");
        x.SetValueWithoutNotify(vector.X);
        x.RegisterValueChangedCallback(XAxisChangeCallback);

        var y = new FloatField("Y");
        y.SetValueWithoutNotify(vector.Y);
        y.RegisterValueChangedCallback(YAxisChangeCallback);

        var z = new FloatField("Z");
        z.SetValueWithoutNotify(vector.Z);
        z.RegisterValueChangedCallback(ZAxisChangeCallback);

        var w = new FloatField("W");
        w.SetValueWithoutNotify(vector.W);
        w.RegisterValueChangedCallback(WAxisChangeCallback);

        contentContainer.Add(x);
        contentContainer.Add(y);
        contentContainer.Add(z);
        contentContainer.Add(w);
    }

    void XAxisChangeCallback(ChangeEvent<float> evt) {
        var old = ((MVectorBindableProperty)Property).Value;
        ((MVectorBindableProperty)Property).Value = new MVector(evt.newValue, old.Y, old.Z, old.W);
    }

    void YAxisChangeCallback(ChangeEvent<float> evt) {
        var old = ((MVectorBindableProperty)Property).Value;
        ((MVectorBindableProperty)Property).Value = new MVector(old.X, evt.newValue, old.Z, old.W);
    }

    void ZAxisChangeCallback(ChangeEvent<float> evt) {
        var old = ((MVectorBindableProperty)Property).Value;
        ((MVectorBindableProperty)Property).Value = new MVector(old.X, old.Y, evt.newValue, old.W);
    }

    void WAxisChangeCallback(ChangeEvent<float> evt) {
        var old = ((MVectorBindableProperty)Property).Value;
        ((MVectorBindableProperty)Property).Value = new MVector(old.X, old.Y, old.Z, evt.newValue);
    }
}
