using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;

[DrawerForNode(typeof(VectorBindablePropertyNode))]
public class EditorVectorBindablePropertyNode : BaseEditorConstantNode
{
    public override void Initialize() {
        title = "Vector Bindable Node";

        RegenerateOutputPort();

        var contents = contentContainer.Q("contents");

        InitializeFields(contents);

        contents.Q("top").Q("output").style.backgroundColor = Color.clear;
        contents.style.paddingBottom = new Length(4, LengthUnit.Pixel);
        contents.style.backgroundColor = (Color)new Color32(46, 46, 46, 205);
    }

    void RegenerateOutputPort() {
        var cache = TractatoriRuntimeUtilities.GetEvaluateCache(UnderlyingRuntimeNode.NodeType);
        var outputParameter = cache.Parameters[1]; // Hardcode second parameter as output

        var port = GeneratePort(Direction.Output, Port.Capacity.Multi, typeof(MVector));
        port.portName = ObjectNames.NicifyVariableName(outputParameter.Name);
        port.name = outputParameter.Name;
        port.portColor = GetPortColor(port.portType);

        var callback = new NodeConnectionCallback() {
            OnDropCallback = (graphView, edge) => {
                var inputPort = edge.input as TractatoriStandardPort;
                var inputNode = inputPort.node as BaseEditorNode;

                var property = TractatoriEditorUtility.GetAllFlowInputs(inputNode.UnderlyingRuntimeNode.NodeType).FirstOrDefault(x => x.Name == inputPort.name);

                if (property != null) {
                    property.SetValue(inputNode.UnderlyingRuntimeNode, new FlowInput(UnderlyingRuntimeNode.GUID));
                } else {
                    Debug.Log("Wat");
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
        textField.SetValueWithoutNotify(((VectorBindablePropertyNode)UnderlyingRuntimeNode).Name);
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
        ((VectorBindablePropertyNode)UnderlyingRuntimeNode).Name = evt.newValue;
        RegenerateOutputPort();
    }
}
