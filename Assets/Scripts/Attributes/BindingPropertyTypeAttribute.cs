using System;
using System.Collections.Generic;
using UnityEngine;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class BindingPropertyTypeAttribute : Attribute
{
    public Type ReturnType { get; private set; }

    public BindingPropertyTypeAttribute(Type type) {
        ReturnType = type;
    }
}
