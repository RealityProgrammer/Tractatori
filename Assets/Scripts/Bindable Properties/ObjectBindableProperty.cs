using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[BindingPropertyType(typeof(UnityEngine.Object))]
public class ObjectBindableProperty : BaseBindableProperty {
    [field: SerializeField] public UnityEngine.Object Value { get; set; }

    public override string ToString() {
        return "ObjectBindableProperty(" + Name + ": " + Value + ", Overridable = " + Overridable + ")";
    }
}
