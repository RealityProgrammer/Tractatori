using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

public static class TractatoriEditorUtility {
    private static readonly Dictionary<Type, FieldOrPropertyInfo[]> _cacheProperties = new Dictionary<Type, FieldOrPropertyInfo[]>();

    public static readonly Type FlowInputType = typeof(FlowInput);
    public static readonly Type BaseRuntimeNodeType = typeof(BaseRuntimeNode);

    public static FieldOrPropertyInfo[] GetAllFlowInputs(Type type) {
        if (!type.IsSubclassOf(BaseRuntimeNodeType) || type.IsAbstract) return null;

        if (_cacheProperties.TryGetValue(type, out var output)) {
            return output;
        } else {
            output = GetAllFields(type).Concat(GetAllProperties(type)).ToArray();

            _cacheProperties.Add(type, output);

            return output;
        }
    }

    private static IEnumerable<FieldOrPropertyInfo> GetAllFields(Type type) {
        return type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.SetField).Where(x => {
            if (x.FieldType != FlowInputType) return false;
            if (x.Name.EndsWith("k__BackingField")) return false;
            if (x.GetCustomAttribute<ExcludeInputAttribute>() != null) return false;
            if (!x.IsPublic) return x.GetCustomAttribute<SerializeField>() != null;

            return true;
        }).Select(x => new FieldOrPropertyInfo(x));
    }

    private static IEnumerable<FieldOrPropertyInfo> GetAllProperties(Type type) {
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty);

        foreach (var property in properties) {
            if (property.PropertyType != FlowInputType) continue;

            var backingField = type.GetField("<" + property.Name + ">k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);

            if (backingField == null) {
                var attr = property.GetCustomAttribute<IncludeFlowInputPropertyAttribute>();

                if (attr != null) {
                    backingField = type.GetField(attr.FallbackField, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

                    if (backingField != null) {
                        yield return new FieldOrPropertyInfo(backingField, property);
                    }
                }
            } else {
                if (property.GetCustomAttribute<ExcludeInputAttribute>() == null) {
                    yield return new FieldOrPropertyInfo(backingField, property);
                }
            }
        }
    }

    public static void LinkSequenceNodes(TractatoriGraphView graphView, DynamicEditorSequenceNode input, DynamicEditorSequenceNode output) {
        LinkPort(graphView, input.Q<TractatoriStandardPort>(DynamicEditorSequenceNode.NextPortName), output.Q<TractatoriStandardPort>(DynamicEditorSequenceNode.PreviousPortName));
    }

    public static void LinkPort(TractatoriGraphView graphView, TractatoriStandardPort output, TractatoriStandardPort input) {
        var edge = new Edge() {
            input = input,
            output = output,
        };

        edge.input.Connect(edge);
        edge.output.Connect(edge);

        graphView.Add(edge);
    }
}