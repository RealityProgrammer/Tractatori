using System.Linq;
using System.Reflection;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor;

public class DynamicEditorSequenceNode : BaseEditorSequenceNode {
    public override void Initialize() {
        title = ObjectNames.NicifyVariableName(UnderlyingRuntimeNode.NodeType.Name);

        GenerateSequencePorts();
        CreateDefaultInputPorts();

        base.Initialize();
    }
}