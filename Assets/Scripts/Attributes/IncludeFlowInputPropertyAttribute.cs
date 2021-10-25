using System;
using System.Collections.Generic;
using UnityEngine;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class IncludeFlowInputPropertyAttribute : Attribute {
    public string FallbackField { get; private set; }

    public IncludeFlowInputPropertyAttribute(string fallbackField) {
        FallbackField = fallbackField;
    }
}