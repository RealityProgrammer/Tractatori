using System;
using System.Collections;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public sealed class ExpectedInputTypeAttribute : Attribute
{
    public Type Expected { get; set; }

    public ExpectedInputTypeAttribute(Type type) {
        Expected = type;
    }
}
