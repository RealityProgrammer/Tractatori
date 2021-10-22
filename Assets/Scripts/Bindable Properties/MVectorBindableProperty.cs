using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[BindingPropertyType(typeof(MVector))]
public class MVectorBindableProperty : BaseBindableProperty {
    public MVector Value { get; set; }

    public override object GetValue() {
        return Value;
    }
}
