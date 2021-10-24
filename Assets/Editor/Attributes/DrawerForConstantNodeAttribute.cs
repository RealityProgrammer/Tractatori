using System;
using System.Collections.Generic;
using UnityEngine;

public class DrawerForConstantNodeAttribute : Attribute
{
    public Type RuntimeType { get; private set; }

    public DrawerForConstantNodeAttribute(Type type) {
        RuntimeType = type;
    }
}
