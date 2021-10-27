using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

[DrawerForNode(typeof(BranchingNode))]
public class EditorBranchingNode : BaseEditorSequenceNode
{
    public BranchingNode Node {
        get => UnderlyingRuntimeNode as BranchingNode;
    }

    public override void Initialize() {
        title = ObjectNames.NicifyVariableName(UnderlyingRuntimeNode.NodeType.Name);

        GenerateSequencePorts();
        CreatePredicatePort();
        CreateFalsePort();

        base.Initialize();
    }

    public override void HandleGraphLoad(GraphLoadInformation container) {
        ConnectNextSequence(container);

        var runtime = Node;

        if (!runtime.Predicate.IsNull()) {
            if (container.GraphView.TrySearchEditorNode<BaseEditorNode>(runtime.Predicate.GUID, out var find)) {
                TractatoriEditorUtility.LinkPort(container.GraphView, find.Query<TractatoriStandardPort>().Where(x => x.direction == Direction.Output && x.OutputIndex == runtime.Predicate.OutputIndex).First(), this.Q<TractatoriStandardPort>(nameof(BranchingNode.Predicate)));
            }
        }

        if (!runtime.False.IsNull()) {
            if (container.GraphView.TrySearchEditorNode<BaseEditorSequenceNode>(runtime.False.GUID, out var find)) {
                TractatoriEditorUtility.LinkPort(container.GraphView, this.Q<TractatoriStandardPort>(nameof(BranchingNode.False)), find.Query<TractatoriStandardPort>().Where(x => x.direction == Direction.Input).First());
            }
        }
    }

    private static readonly Type[] _predicatePortType = new Type[1] { typeof(Boolean4) };
    private void CreatePredicatePort() {
        var property = TractatoriEditorUtility.GetAllFlowInputs(UnderlyingRuntimeNode.NodeType).FirstOrDefault(x => x.Name == nameof(BranchingNode.Predicate));

        var port = GeneratePort(Direction.Input, Port.Capacity.Single, _predicatePortType);
        port.portName = ObjectNames.NicifyVariableName(property.Name);
        port.name = property.Name;
        port.portColor = GetPortColor(port.portType);

        var callback = new NodeConnectionCallback() {
            OnDropCallback = (graphView, edge) => {
                var output = edge.output.node as BaseEditorNode;

                property.SetValue(UnderlyingRuntimeNode, output.UnderlyingRuntimeNode.GUID);
            }
        };

        port.AddManipulator(new EdgeConnector<Edge>(callback));
        inputContainer.Add(port);
    }

    private void CreateFalsePort() {
        var property = TractatoriEditorUtility.GetAllFlowInputs(UnderlyingRuntimeNode.NodeType).FirstOrDefault(x => x.Name == nameof(BranchingNode.False));

        var port = GeneratePort(Direction.Output, Port.Capacity.Single, SequenceNodeType);
        port.portName = ObjectNames.NicifyVariableName(property.Name);
        port.name = property.Name;
        port.portColor = GetPortColor(port.portType);

        var callback = new NodeConnectionCallback() {
            OnDropCallback = (graphView, edge) => {
                property.SetValue(UnderlyingRuntimeNode, new FlowInput((edge.input.node as BaseEditorNode).UnderlyingRuntimeNode.GUID));
            }
        };

        port.AddManipulator(new EdgeConnector<Edge>(callback));
        outputContainer.Add(port);
    }
}
