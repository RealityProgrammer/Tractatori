using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

public abstract class BaseEditorSequenceNode : BaseEditorNode
{
    public const string PreviousPortName = "previous-sequence";
    public const string NextPortName = "next-sequence";

    protected static readonly Type SequenceNodeType = typeof(BaseSequenceNode);

    public BaseSequenceNode UnderlyingSequenceNode {
        get => UnderlyingRuntimeNode as BaseSequenceNode;
        set {
            if (value == null) {
                Debug.LogError("Cannot set the underlying sequence node to null");
            }

            UnderlyingRuntimeNode = value;
        }
    }

    protected void GenerateSequencePorts() {
        var previousSequencePort = GeneratePort(Direction.Input, Port.Capacity.Single, SequenceNodeType);
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

        var nextSequencePort = GeneratePort(Direction.Output, Port.Capacity.Single, SequenceNodeType);
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

    public override void HandleGraphLoad(GraphLoadInformation container) {
        ConnectNextSequence(container);
        ConnectFlowInputs(container);
    }

    protected void ConnectNextSequence(GraphLoadInformation container) {
        if (!UnderlyingSequenceNode.Next.IsNull()) {
            if (container.GraphView.TrySearchEditorNode<BaseEditorSequenceNode>(UnderlyingSequenceNode.Next.GUID, out var next)) {
                TractatoriEditorUtility.LinkSequenceNodes(container.GraphView, this, next);
            }
        }
    }
}
