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

public class DynamicEditorNode : BaseEditorNode {
    public const string PreviousPortName = "previous-sequence";
    public const string NextPortName = "next-sequence";

    public const BindingFlags FieldBindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;
    public const BindingFlags PropertyBindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;

    private static readonly Type objectType = typeof(object);

    public DynamicEditorNode(BaseRuntimeNode underlying) {
        UnderlyingRuntimeNode = underlying;
    }

    public override void Initialize() {
        title = ObjectNames.NicifyVariableName(UnderlyingRuntimeNode.NodeType.Name);

        contentContainer.Q("contents").Q("top").Q("output").style.backgroundColor = Color.clear;
        contentContainer.Q("contents").style.backgroundColor = (Color)new Color32(46, 46, 46, 205);

        var fields = STEditorUtilities.GetAllFlowInputFields(UnderlyingRuntimeNode.NodeType);
        var properties = STEditorUtilities.GetAllFlowInputProperties(UnderlyingRuntimeNode.NodeType);

        foreach (var field in fields) {
            var expectedTypeAttribute = field.GetCustomAttribute<ExpectedInputTypeAttribute>();
            var expectedType = expectedTypeAttribute == null ? objectType : expectedTypeAttribute.Expected;

            var port = GeneratePort(Direction.Input, Port.Capacity.Single, expectedType);
            port.portName = ObjectNames.NicifyVariableName(field.Name);
            port.name = field.Name;

            var callback = new NodeConnectionCallback() {
                OnDropCallback = (graphView, edge) => {
                    var output = edge.output.node as BaseEditorNode;

                    field.SetValue(UnderlyingRuntimeNode, output.UnderlyingRuntimeNode.GUID);
                }
            };

            port.AddManipulator(new EdgeConnector<Edge>(callback));
            inputContainer.Add(port);
        }

        foreach (var property in properties) {
            var expectedTypeAttribute = property.BackingField.GetCustomAttribute<ExpectedInputTypeAttribute>();
            var expectedType = expectedTypeAttribute == null ? objectType : expectedTypeAttribute.Expected;

            var port = GeneratePort(Direction.Input, Port.Capacity.Single, expectedType);
            port.portName = ObjectNames.NicifyVariableName(property.Property.Name);
            port.name = property.Property.Name;

            var callback = new NodeConnectionCallback() {
                OnDropCallback = (graphView, edge) => {
                    var output = edge.output.node as BaseEditorNode;

                    property.Property.SetValue(UnderlyingRuntimeNode, output.UnderlyingRuntimeNode.GUID);
                    property.CallbackMethod.Invoke(UnderlyingRuntimeNode, null);

                    Debug.Log("Callback Time boi");
                }
            };

            port.AddManipulator(new EdgeConnector<Edge>(callback));
            inputContainer.Add(port);
        }

        var evaluateCache = ManipulationUtilities.GetEvaluateCache(UnderlyingRuntimeNode.NodeType);
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

                var callback = new NodeConnectionCallback() {
                    OnDropCallback = (graphView, edge) => {
                        var output = edge.output.node as BaseEditorNode;
                        var input = edge.input.node as BaseEditorNode;

                        var field = STEditorUtilities.GetAllFlowInputFields(input.UnderlyingRuntimeNode.NodeType).Where(x => x.Name == edge.input.name).FirstOrDefault();
                        if (field != null) {
                            field.SetValue(input.UnderlyingRuntimeNode, new FlowInput(output.UnderlyingRuntimeNode.GUID, port.OutputIndex));
                        } else {
                            var property = STEditorUtilities.GetAllFlowInputProperties(input.UnderlyingRuntimeNode.NodeType).Where(x => x.Property.Name == edge.input.name).FirstOrDefault();

                            if (property != null) {
                                property.Property.SetValue(input.UnderlyingRuntimeNode, new FlowInput(output.UnderlyingRuntimeNode.GUID, port.OutputIndex));
                            } else {
                                Debug.LogWarning("Erm...");
                            }
                        }
                    }
                };

                port.AddManipulator(new EdgeConnector<Edge>(callback));

                outputContainer.Add(port);

                outTracker++;
            }
        }

        RefreshExpandedState();
        RefreshPorts();
    }

    public bool IsValidField(FieldInfo field) {
        if (!field.IsPublic) {
            if (field.GetCustomAttribute<SerializeField>() == null) return false;
        }

        return !field.Name.EndsWith("k__BackingField");
    }

    public bool IsValidProperty(PropertyInfo property) {
        return property.CanRead && property.CanWrite && UnderlyingRuntimeNode.NodeType.GetField("<" + property.Name + ">k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic) != null;
    }
}