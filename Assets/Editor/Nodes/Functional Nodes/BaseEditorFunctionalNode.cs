using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;

public abstract class BaseEditorFunctionalNode : BaseEditorNode {
    public override void Initialize() {
        base.Initialize();

        contentContainer.Q("contents").Q("top").Q("output").style.backgroundColor = Color.clear;
        contentContainer.Q("contents").style.backgroundColor = (Color)new Color32(46, 46, 46, 205);
    }

    private static readonly Type[] fallbackTypes = new Type[1] { typeof(object) };
    protected void CreateDefaultInputPorts() {
        foreach (var property in TractatoriEditorUtility.GetAllFlowInputs(UnderlyingRuntimeNode.NodeType)) {
            var expectedTypeAttribute = property.GetAttribute<ExpectedInputTypeAttribute>();

            var expectedTypes = expectedTypeAttribute == null ? fallbackTypes : expectedTypeAttribute.Expected;

            var port = GeneratePort(Direction.Input, Port.Capacity.Single, expectedTypes);
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
    }

    protected void CreateDefaultOutputPorts() {
        var evaluateCache = TractatoriRuntimeUtilities.GetEvaluateCache(UnderlyingRuntimeNode.NodeType);
        if (evaluateCache == null) {
            Debug.LogWarning("Something gone wrong while building port for runtime node of type " + UnderlyingRuntimeNode.NodeType.FullName);
        } else {
            int outTracker = 0;

            for (int i = 0; i < evaluateCache.Parameters.Length; i++) {
                var parameter = evaluateCache.Parameters[i];
                if (!parameter.IsOut) continue;

                var port = GeneratePort(Direction.Output, Port.Capacity.Multi, parameter.ParameterType.GetElementType());
                port.portName = ObjectNames.NicifyVariableName(parameter.Name);
                port.name = parameter.Name;
                port.OutputIndex = evaluateCache.LayoutIndex[i];
                port.portColor = GetPortColor(port.portType);

                var callback = new NodeConnectionCallback() {
                    OnDropCallback = (graphView, edge) => {
                        var output = edge.output.node as BaseEditorNode;
                        var input = edge.input.node as BaseEditorNode;

                        var property = TractatoriEditorUtility.GetAllFlowInputs(input.UnderlyingRuntimeNode.NodeType).Where(x => x.Name == edge.input.name).FirstOrDefault();
                        if (property != null) {
                            property.SetValue(input.UnderlyingRuntimeNode, new FlowInput(output.UnderlyingRuntimeNode.GUID, port.OutputIndex));
                        } else {
                            Debug.LogWarning("Something went wrong while connecting Editor Node. Information: Tried to find Property " + edge.input.name + " of node type " + input.UnderlyingRuntimeNode.NodeType.AssemblyQualifiedName);
                        }
                    }
                };

                port.AddManipulator(new EdgeConnector<Edge>(callback));

                outputContainer.Add(port);

                outTracker++;
            }
        }
    }

    protected VisualElement CreateFieldContainer() {
        var contents = contentContainer.Q("contents");

        var seperator = new VisualElement();
        seperator.name = "seperator";
        seperator.style.borderBottomWidth = 1;
        seperator.style.backgroundColor = (Color)new Color32(35, 35, 35, 255);

        contents.Add(seperator);

        var fieldContainer = new VisualElement();
        fieldContainer.style.height = StyleKeyword.Auto;
        fieldContainer.style.width = StyleKeyword.Auto;
        fieldContainer.style.backgroundColor = new Color(0.24f, 0.24f, 0.24f, 0.65f);

        contents.Add(fieldContainer);

        return fieldContainer;
    }
}