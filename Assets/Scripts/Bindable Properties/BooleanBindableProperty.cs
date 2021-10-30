using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[BindingPropertyType(typeof(Boolean4))]
public class BooleanBindableProperty : BaseBindableProperty
{
    [field: SerializeField] public Boolean4 Value { get; set; }
}
