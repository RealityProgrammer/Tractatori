using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class SequenceNodeSearchPathAttribute : Attribute
{
    public string Path { get; set; }
}
