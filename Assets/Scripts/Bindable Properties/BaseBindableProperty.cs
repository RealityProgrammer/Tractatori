using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BaseBindableProperty
{
    [field: SerializeField] public string Name { get; set; }
    [field: SerializeField] public bool Exposed { get; set; }

    public virtual object GetValue() => null;
}
