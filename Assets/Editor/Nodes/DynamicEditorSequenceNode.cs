using System.Linq;
using System.Reflection;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor;

public class DynamicEditorSequenceNode : DynamicEditorFunctionalNode {
    public const string PreviousPortName = "previous-sequence";
    public const string NextPortName = "next-sequence";

    public BaseSequenceNode UnderlyingSequenceNode {
        get => UnderlyingRuntimeNode as BaseSequenceNode;
        set {
            if (value == null) {
                Debug.LogError("Cannot set the underlying sequence node to null");
            }

            UnderlyingRuntimeNode = value;
        }
    }

    public override void Initialize() {
        title = ObjectNames.NicifyVariableName(UnderlyingRuntimeNode.NodeType.Name);

        GenerateSequencePorts();

        base.Initialize();
    }

    private void GenerateSequencePorts() {
        var previousSequencePort = GeneratePort(Direction.Input, Port.Capacity.Single, typeof(BaseSequenceNode));
        previousSequencePort.portName = "Previous";
        previousSequencePort.name = PreviousPortName;
        var callback = new NodeConnectionCallback();
        callback.OnDropCallback = (graphView, edge) => {
            BaseEditorNode output = edge.output.node as BaseEditorNode;
            BaseEditorNode input = edge.input.node as BaseEditorNode;

            if (output.IsEntryPoint) {
                TractatoriGraphEditorWindow.CurrentEditingAsset.EntrySequence = input.UnderlyingRuntimeNode.GUID;
            } else {
                ((BaseSequenceNode)input.UnderlyingRuntimeNode).Previous = new FlowInput(output.UnderlyingRuntimeNode.GUID);
                ((BaseSequenceNode)output.UnderlyingRuntimeNode).Next = new FlowInput(input.UnderlyingRuntimeNode.GUID);
            }
        };

        previousSequencePort.AddManipulator(new EdgeConnector<Edge>(callback));
        inputContainer.Add(previousSequencePort);

        var nextSequencePort = GeneratePort(Direction.Output, Port.Capacity.Single, typeof(BaseSequenceNode));
        nextSequencePort.portName = "Next";
        nextSequencePort.name = NextPortName;
        callback = new NodeConnectionCallback();
        callback.OnDropCallback = (graphView, edge) => {
            BaseEditorNode output = edge.output.node as BaseEditorNode;
            BaseEditorNode input = edge.input.node as BaseEditorNode;

            ((BaseSequenceNode)input.UnderlyingRuntimeNode).Previous = new FlowInput(output.UnderlyingRuntimeNode.GUID);
            ((BaseSequenceNode)output.UnderlyingRuntimeNode).Next = new FlowInput(input.UnderlyingRuntimeNode.GUID);
        };

        nextSequencePort.AddManipulator(new EdgeConnector<Edge>(callback));
        outputContainer.Add(nextSequencePort);
    }
}