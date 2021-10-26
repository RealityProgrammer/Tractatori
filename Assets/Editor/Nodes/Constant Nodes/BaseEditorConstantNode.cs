using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

public abstract class BaseEditorConstantNode : BaseEditorNode {
    public override void Initialize() {
        var cache = TractatoriRuntimeUtilities.GetEvaluateCache(UnderlyingRuntimeNode.NodeType);
        var parameters = cache.Parameters;

        foreach (var parameter in parameters) {
            if (!parameter.IsOut) continue;

            var port = GeneratePort(Direction.Output, Port.Capacity.Multi, parameter.ParameterType.GetElementType());
            port.portName = ObjectNames.NicifyVariableName(parameter.Name);
            port.name = parameter.Name;
            port.portColor = GetPortColor(port.portType);

            var callback = new NodeConnectionCallback() {
                OnDropCallback = (graphView, edge) => {
                    var inputNode = edge.input.node as BaseEditorNode;

                    var property = TractatoriEditorUtility.GetAllFlowInputs(inputNode.UnderlyingRuntimeNode.NodeType).Where(x => x.Name == edge.input.name).FirstOrDefault();

                    if (property != null) {
                        property.SetValue(inputNode.UnderlyingRuntimeNode, new FlowInput(UnderlyingRuntimeNode.GUID));
                    } else {
                        Debug.LogWarning("Something went wrong...");
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
