using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[BindablePropertyDrawerOf(typeof(MVectorBindableProperty))]
public class MVectorBindablePropertyDrawer : BaseBindablePropertyDrawer {
    public override void Initialize(VisualElement container) {
        DoExposedField(container);

        var vector = ((MVectorBindableProperty)Parent.Property).Value;

        var x = new FloatField("X");
        x.SetValueWithoutNotify(vector.X);
        x.RegisterValueChangedCallback(XAxisChangeCallback);
        ModifyLabel(x);

        var y = new FloatField("Y");
        y.SetValueWithoutNotify(vector.Y);
        y.RegisterValueChangedCallback(YAxisChangeCallback);
        ModifyLabel(y);

        var z = new FloatField("Z");
        z.SetValueWithoutNotify(vector.Z);
        z.RegisterValueChangedCallback(ZAxisChangeCallback);
        ModifyLabel(z);

        var w = new FloatField("W");
        w.SetValueWithoutNotify(vector.W);
        w.RegisterValueChangedCallback(WAxisChangeCallback);
        ModifyLabel(w);

        container.Add(x);
        container.Add(y);
        container.Add(z);
        container.Add(w);
    }

    void XAxisChangeCallback(ChangeEvent<float> evt) {
        var old = ((MVectorBindableProperty)Parent.Property).Value;
        ((MVectorBindableProperty)Parent.Property).Value = new MVector(evt.newValue, old.Y, old.Z, old.W);
    }

    void YAxisChangeCallback(ChangeEvent<float> evt) {
        var old = ((MVectorBindableProperty)Parent.Property).Value;
        ((MVectorBindableProperty)Parent.Property).Value = new MVector(old.X, evt.newValue, old.Z, old.W);
    }

    void ZAxisChangeCallback(ChangeEvent<float> evt) {
        var old = ((MVectorBindableProperty)Parent.Property).Value;
        ((MVectorBindableProperty)Parent.Property).Value = new MVector(old.X, old.Y, evt.newValue, old.W);
    }

    void WAxisChangeCallback(ChangeEvent<float> evt) {
        var old = ((MVectorBindableProperty)Parent.Property).Value;
        ((MVectorBindableProperty)Parent.Property).Value = new MVector(old.X, old.Y, old.Z, evt.newValue);
    }
}
