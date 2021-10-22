using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class BaseBindableProperty
{
    [field: SerializeField] public string Name { get; set; }

    public abstract object GetValue();
}
