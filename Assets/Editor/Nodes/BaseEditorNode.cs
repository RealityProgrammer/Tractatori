using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor;

public class BaseEditorNode : Node
{
    static readonly Dictionary<Type, Color> _portColorDictionary = new Dictionary<Type, Color>();
    static readonly Color fallbackColor = new Color(0.85f, 0.85f, 0.85f);

    static BaseEditorNode() {
        _portColorDictionary = new Dictionary<Type, Color>() {
            [typeof(MVector)] = new Color(0f, 0.65f, 1f),
            [typeof(MVectorInt)] = Color.cyan,

            [typeof(string)] = new Color32(0xCE, 0x9D, 0x82, 0xFF),
            [typeof(Boolean4)] = new Color32(0xFF, 0x00, 0xDC, 0xFF),
        };
    }

    public static void RegisterPortColor<T>(Color value) {
        _portColorDictionary[typeof(T)] = value;
    }
    public static void RegisterPortColor(Type type, Color value) {
        _portColorDictionary[type] = value;
    }

    private static Type sequenceNodeType = typeof(BaseSequenceNode);
    public static Color GetPortColor(Type type) {
        if (_portColorDictionary.TryGetValue(type, out var output)) {
            return output;
        }

        if (type.IsSubclassOf(sequenceNodeType) || type == sequenceNodeType) {
            return new Color32(0, 255, 33, 255);
        }

        return fallbackColor;
    }

    public bool IsEntryPoint { get; set; }

    public BaseRuntimeNode UnderlyingRuntimeNode { get; set; }

    public virtual void Initialize() {
        title = ObjectNames.NicifyVariableName(UnderlyingRuntimeNode.NodeType.Name);
    }

    public TractatoriStandardPort GeneratePort(Direction direction, Port.Capacity capacity, Type expectedType) {
        var port = new TractatoriStandardPort(Orientation.Horizontal, direction, capacity, expectedType);
        port.portColor = GetPortColor(port.portType);
        port.ExpectedTypes = new Type[1] { expectedType };

        return port;
    }

    public TractatoriStandardPort GeneratePort(Direction direction, Port.Capacity capacity, Type[] expectedTypes) {
        var port = new TractatoriStandardPort(Orientation.Horizontal, direction, capacity, expectedTypes[0]);
        port.portColor = GetPortColor(port.portType);
        port.ExpectedTypes = expectedTypes;

        return port;
    }

    protected static readonly Type[] fallbackTypes = new Type[1] { typeof(object) };
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

    protected void ConnectFlowInputs(GraphLoadInformation container) {
        var properties = TractatoriEditorUtility.GetAllFlowInputs(UnderlyingRuntimeNode.NodeType);

        foreach (var property in properties) {
            var flow = (FlowInput)property.Property.GetValue(UnderlyingRuntimeNode);

            if (!flow.IsNull()) {
                if (container.GraphView.TrySearchEditorNode<BaseEditorNode>(flow.GUID, out var find)) {
                    TractatoriEditorUtility.LinkPort(container.GraphView, find.Query<TractatoriStandardPort>().Where(x => x.direction == Direction.Output && x.OutputIndex == flow.OutputIndex).First(), this.Q<TractatoriStandardPort>(property.Property.Name));
                } else {
                    property.SetValue(UnderlyingRuntimeNode, FlowInput.Null);
                }
            }
        }
    }

    public virtual void HandleGraphLoad(GraphLoadInformation container) {
        ConnectFlowInputs(container);
    }

    private VisualElement _fieldElement;
    public VisualElement FieldContainer {
        get {
            _fieldElement = _fieldElement ?? CreateFieldContainer();

            return _fieldElement;
        }
    }
    private VisualElement CreateFieldContainer() {
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
