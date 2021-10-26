using System;
using System.Collections.Generic;
using UnityEngine;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class FunctionalNodeCreationAttribute : Attribute
{
    public Type TargetType { get; private set; }

    public FunctionalNodeCreationAttribute(Type type) {
        TargetType = type;
    }
}
