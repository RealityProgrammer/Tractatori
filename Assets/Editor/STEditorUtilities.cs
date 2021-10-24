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

public static class STEditorUtilities {
    public class NodePropertyCache {
        public PropertyInfo Property;
        public IncludeFlowInputPropertyAttribute IncludeFlowInputProperty;
        public FieldInfo BackingField;
        public MethodInfo CallbackMethod;

        public bool UseBackingField => BackingField != null;
    }

    private static readonly Dictionary<Type, FieldInfo[]> _cacheFieldTypes = new Dictionary<Type, FieldInfo[]>();
    private static readonly Dictionary<Type, NodePropertyCache[]> _cacheNodeProperties = new Dictionary<Type, NodePropertyCache[]>();

    public static readonly Type FlowInputType = typeof(FlowInput);
    public static readonly Type BaseRuntimeNodeType = typeof(BaseRuntimeNode);

    public static FieldInfo[] GetAllFlowInputFields(Type type) {
        if (!type.IsSubclassOf(BaseRuntimeNodeType)) return null;

        if (_cacheFieldTypes.TryGetValue(type, out var fields)) {
            return fields;
        } else {
            fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic).Where(x => {
                if (x.FieldType != FlowInputType) return false;
                if (x.Name.EndsWith("k__BackingField")) return false;
                if (x.GetCustomAttribute<ExcludeInputAttribute>() != null) return false;
                if (!x.IsPublic) return x.GetCustomAttribute<SerializeField>() != null;

                return true;
            }).ToArray();

            _cacheFieldTypes[type] = fields;

            return fields;
        }
    }

    public static FieldInfo[] GetAllFlowInputFields<T>() where T : BaseRuntimeNode {
        return GetAllFlowInputFields(typeof(T));
    }

    public static NodePropertyCache[] GetAllFlowInputProperties<T>() {
        return GetAllFlowInputProperties(typeof(T));
    }

    public static NodePropertyCache[] GetAllFlowInputProperties(Type type) {
        if (!type.IsSubclassOf(BaseRuntimeNodeType)) return null;

        if (_cacheNodeProperties.TryGetValue(type, out var cache)) {
            return cache;
        } else {
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetProperty | BindingFlags.SetProperty).Where(x => {
                return x.PropertyType == FlowInputType;
            });

            _cacheNodeProperties[type] = InitializePropertyCache(properties, type).ToArray();

            return _cacheNodeProperties[type];
        }
    }

    private static IEnumerable<NodePropertyCache> InitializePropertyCache(IEnumerable<PropertyInfo> infos, Type type) {
        foreach (var info in infos) {
            var backingField = type.GetField("<" + info.Name + ">k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);

            if (backingField != null) {
                if (backingField.GetCustomAttribute<ExcludeInputAttribute>() == null) {
                    yield return new NodePropertyCache() {
                        Property = info,
                        BackingField = backingField,
                    };
                }
            } else {
                var attribute = info.GetCustomAttribute<IncludeFlowInputPropertyAttribute>();

                if (attribute != null) {
                    backingField = type.GetField(attribute.FallbackField, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                    if (backingField == null) {
                        Debug.LogWarning($"Trying to cache property {attribute.FallbackField} of type {type.FullName} with [IncludeFlowInputProperty(\"{attribute.FallbackField}\", {(string.IsNullOrEmpty(attribute.CallbackMethod) ? "null" : $"\"attribute.CallbackMethod\"")})], but fallback field cannot be found.");
                    } else {
                        MethodInfo callbackMethod = null;

                        if (!string.IsNullOrEmpty(attribute.CallbackMethod)) {
                            callbackMethod = type.GetMethod(attribute.CallbackMethod, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, Type.DefaultBinder, null, null);
                            if (callbackMethod != null) {
                                if (callbackMethod.ReturnType != null || callbackMethod.GetParameters().Length > 0) {
                                    callbackMethod = null;
                                }
                            }
                        }

                        yield return new NodePropertyCache() {
                            Property = info,
                            IncludeFlowInputProperty = attribute,
                            BackingField = backingField,
                            CallbackMethod = callbackMethod,
                        };
                    }
                }
            }
        }
    }

    public static void LinkSequenceNodes(STEditorGraphView graphView, DynamicEditorSequenceNode input, DynamicEditorSequenceNode output) {
        LinkPort(graphView, input.Q<STNodePort>(DynamicEditorNode.NextPortName), output.Q<STNodePort>(DynamicEditorSequenceNode.PreviousPortName));
    }

    public static void LinkPort(STEditorGraphView graphView, STNodePort output, STNodePort input) {
        var edge = new Edge() {
            input = input,
            output = output,
        };

        edge.input.Connect(edge);
        edge.output.Connect(edge);

        graphView.Add(edge);
    }
}