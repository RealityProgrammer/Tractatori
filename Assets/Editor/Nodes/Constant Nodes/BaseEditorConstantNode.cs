using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

public abstract class BaseEditorConstantNode : BaseEditorNode {
    public override void Initialize() {
        var cache = ManipulationUtilities.GetEvaluateCache(UnderlyingRuntimeNode.NodeType);
        var parameters = cache.Parameters;

        foreach (var parameter in parameters) {
            if (!parameter.IsOut) continue;

            var port = GeneratePort(Direction.Output, Port.Capacity.Multi, parameter.ParameterType.GetElementType());
            port.portName = ObjectNames.NicifyVariableName(parameter.Name);
            port.name = parameter.Name;
            port.portColor = GetPortColor(port.portType);

            var callback = new NodeConnectionCallback() {
                OnDropCallback = (graphView, edge) => {
                    Debug.Log("Input: " + edge.input.node.GetType().FullName);
                    Debug.Log("Output: " + edge.output.node.GetType().FullName); // Output == this

                    var inputPort = edge.input as STNodePort;
                    var inputNode = inputPort.node as BaseEditorNode;

                    var underlyingNodeType = inputNode.UnderlyingRuntimeNode.NodeType;

                    FieldInfo field = underlyingNodeType.GetField(inputPort.name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
                    if (field != null) {
                        field.SetValue(inputNode.UnderlyingRuntimeNode, new FlowInput(UnderlyingRuntimeNode.GUID));
                    } else {
                        PropertyInfo info = underlyingNodeType.GetProperty(inputPort.name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                        if (info != null) {
                            info.SetValue(inputNode.UnderlyingRuntimeNode, new FlowInput(UnderlyingRuntimeNode.GUID));
                        } else {
                            Debug.Log("Wat");
                        }
                    }
                },
            };

            port.AddManipulator(new EdgeConnector<Edge>(callback));

            outputContainer.Add(port);
        }

        var contents = contentContainer.Q("contents");

        InitializeFields(contents);

        contents.style.paddingBottom = new Length(4, LengthUnit.Pixel);

        contents.Q("top").Q("output").style.backgroundColor = Color.clear;
        contents.style.backgroundColor = (Color)new Color32(46, 46, 46, 205);

        RefreshExpandedState();
        RefreshPorts();
    }

    public abstract void InitializeFields(VisualElement contents);
}
