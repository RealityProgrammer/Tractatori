﻿using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;

[DrawerForConstantNode(typeof(ObjectBindablePropertyNode))]
public class EditorObjectBindablePropertyNode : BaseEditorConstantNode
{
    public override void Initialize() {
        title = "Object Bindable Node";

        RegenerateOutputPort();

        var contents = contentContainer.Q("contents");

        InitializeFields(contents);

        contents.Q("top").Q("output").style.backgroundColor = Color.clear;
        contents.style.paddingBottom = new Length(4, LengthUnit.Pixel);
        contents.style.backgroundColor = (Color)new Color32(46, 46, 46, 205);
    }

    void RegenerateOutputPort() {
        var cache = ManipulationUtilities.GetEvaluateCache(UnderlyingRuntimeNode.NodeType);
        var outputParameter = cache.Parameters[1]; // Hardcode second parameter as output

        var port = GeneratePort(Direction.Output, Port.Capacity.Multi, typeof(Object));
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
                    } else {
                        Debug.Log("Wat");
                    }
                }
            },
        };

        port.AddManipulator(new EdgeConnector<Edge>(callback));
        outputContainer.Add(port);

        RefreshExpandedState();
        RefreshPorts();
    }

    public override void InitializeFields(VisualElement contents) {
        TextField textField = new TextField();
        textField.multiline = false;
        textField.SetValueWithoutNotify(((ObjectBindablePropertyNode)UnderlyingRuntimeNode).Name);
        textField.RegisterValueChangedCallback(ValueChangeCallback);

        textField.style.maxWidth = new Length(200, LengthUnit.Pixel);
        textField.style.width = StyleKeyword.Auto;

        var textInput = textField.Q("unity-text-input").style;
        textInput.unityTextAlign = TextAnchor.UpperLeft;
        textInput.whiteSpace = WhiteSpace.Normal;

        contents.style.height = StyleKeyword.Auto;

        contents.Add(textField);
    }

    void ValueChangeCallback(ChangeEvent<string> evt) {
        ((ObjectBindablePropertyNode)UnderlyingRuntimeNode).Name = evt.newValue;
        RegenerateOutputPort();
    }
}
