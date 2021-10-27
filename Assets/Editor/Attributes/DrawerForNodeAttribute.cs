using System;
using System.Collections.Generic;
using UnityEngine;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class DrawerForNodeAttribute : Attribute {
    public Type Target { get; private set; }

    public DrawerForNodeAttribute(Type type) {
        Target = type;
    }
}
