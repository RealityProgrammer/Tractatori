using System;
using System.Reflection;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;

public class DynamicEditorFunctionalNode : BaseEditorFunctionalNode {
    public const BindingFlags FieldBindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;
    public const BindingFlags PropertyBindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;

    public override void Initialize() {
        base.Initialize();

        CreateDefaultInputPorts();
        CreateDefaultOutputPorts();

        RefreshExpandedState();
        RefreshPorts();
    }
}