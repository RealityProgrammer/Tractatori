using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class CustomizeSearchPathAttribute : Attribute
{
    public string Path { get; private set; }

    public CustomizeSearchPathAttribute(string p) {
        Path = p;
    }
}
