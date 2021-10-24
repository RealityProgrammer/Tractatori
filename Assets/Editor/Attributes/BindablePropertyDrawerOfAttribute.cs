using System;
using System.Collections.Generic;
using UnityEngine;

public class BindablePropertyDrawerOfAttribute : Attribute
{
    public Type Target { get; private set; }

    public BindablePropertyDrawerOfAttribute(Type target) {
        Target = target;
    }
}
