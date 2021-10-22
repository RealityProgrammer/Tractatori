using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;

public class EditorBindingPropertyNode : BaseEditorConstantNode
{
    public EditorBindingPropertyNode(BindingPropertyNode node) : base() {
        UnderlyingRuntimeNode = node;
    }

    public override void Initialize() {
        title = "Binding Property Node";

        RegenerateOutputPort();
    }

    void RegenerateOutputPort() {
        var cache = ManipulationUtilities.GetEvaluateCache(UnderlyingRuntimeNode.NodeType);
        var outputParameter = cache.Parameters[1]; // Hardcode second parameter as output

        var property = STGraphEditorWindow.CurrentEditingAsset.FindBindingProperty(((BindingPropertyNode)UnderlyingRuntimeNode).Name);
        if (property != null) {
            var attribute = property.GetType().GetCustomAttribute<BindingPropertyTypeAttribute>();

            if (attribute == null) {
                Debug.LogWarning("No [BindingPropertyType] attribute found on Binding Property of type: " + property.GetType().FullName);
            } else {
                var port = GeneratePort(Direction.Output, Port.Capacity.Multi, attribute.ReturnType);
                port.portName = ObjectNames.NicifyVariableName(outputParameter.Name);
                port.name = outputParameter.Name;
                port.portColor = GetPortColor(port.portType);

                var callback = new NodeConnectionCallback() {
                    OnDropCallback = (graphView, edge) => {
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
                                Debug.Log("Thonking...");
                            } else {
                                Debug.Log("Wat");
                            }
                        }
                    },
                };

                port.AddManipulator(new EdgeConnector<Edge>(callback));
                outputContainer.Add(port);

                var contents = contentContainer.Q("contents");

                InitializeFields(contents);

                contents.style.paddingBottom = new Length(4, LengthUnit.Pixel);

                contents.Q("top").Q("output").style.backgroundColor = Color.clear;
                contents.style.backgroundColor = (Color)new Color32(46, 46, 46, 205);

                RefreshExpandedState();
                RefreshPorts();
            }
        } else {
            this.Q<STNodePort>(outputParameter.Name).RemoveFromHierarchy();
        }
    }

    public override void InitializeFields(VisualElement contents) {
        TextField textField = new TextField();
        textField.multiline = false;
        textField.SetValueWithoutNotify(((BindingPropertyNode)UnderlyingRuntimeNode).Name);
        textField.RegisterValueChangedCallback(ValueChangeCallback);

        textField.style.maxWidth = new Length(200, LengthUnit.Pixel);
        textField.style.width = StyleKeyword.Auto;

        var textInput = textField.Q("unity-text-input").style;
        textInput.unityTextAlign = TextAnchor.UpperLeft;
        textInput.whiteSpace = WhiteSpace.Normal;

        contents.Add(textField);

        contents.style.height = StyleKeyword.Auto;
    }

    void ValueChangeCallback(ChangeEvent<string> evt) {
        ((BindingPropertyNode)UnderlyingRuntimeNode).Name = evt.newValue;
        RegenerateOutputPort();
    }
}
