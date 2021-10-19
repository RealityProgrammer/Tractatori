using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Explicit tell Tractatori the ID number of Evaluate output parameter (which can support
/// backward compability in case of modification of Evaluate method of the node)
/// 
/// Note: Always use the Attribute with the value larger or equals than 0, negative values
/// are either preserved, or not supported
/// </summary>
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
public sealed class OutputLayoutIndexAttribute : Attribute
{
    public int Index { get; private set; }

    public OutputLayoutIndexAttribute(int index) {
        Index = index;
    }
}
