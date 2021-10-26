using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
}
