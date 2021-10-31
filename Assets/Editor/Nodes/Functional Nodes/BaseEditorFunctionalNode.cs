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
}