using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class NodeCreationResult {
    public BaseEditorNode Output { get; set; }
    public bool Result { get; set; }
    public NodeCreationRequest Request { get; set; }
}

public abstract class NodeCreationRequest {
    public Type UnderlyingNodeType;
    public Vector2 Position { get; set; }

    public BaseRuntimeNode ExistInstance { get; set; }

    public abstract NodeCreationResult Handle();
}

public class ConstantNodeCreationRequest : NodeCreationRequest {
    private static readonly Type ConstantInterfaceType = typeof(IConstantValueNode);
    private static readonly Dictionary<Type, Type> _editorDictionary;
    static ConstantNodeCreationRequest() {
        _editorDictionary = new Dictionary<Type, Type>();

        var cache = TypeCache.GetTypesDerivedFrom<BaseEditorConstantNode>();
        foreach (var type in cache) {
            var attr = type.GetCustomAttribute<DrawerForNodeAttribute>();

            if (attr == null) {
                Debug.LogWarning("An editor type derived from BaseEditorConstantNode but without attribute [DrawerForConstantNode] detected: " + type.AssemblyQualifiedName);
            } else {
                if (_editorDictionary.TryGetValue(attr.Target, out var drawer)) {
                    Debug.LogWarning($"Constant node type {attr.Target.FullName} already has a drawer type of {drawer.FullName}");
                } else {
                    if (attr.Target.GetInterfaces().Contains(ConstantInterfaceType)) {
                        _editorDictionary.Add(attr.Target, type);
                    } else {
                        Debug.Log($"An editor type derived from BaseEditorConstantNode with attribute [DrawerForConstantNode] target to non-constant node type. Drawer type {drawer.FullName} targeted {attr.Target.FullName}");
                    }
                }
            }
        }
    }

    public override NodeCreationResult Handle() {
        if (ExistInstance == null) {
            if (_editorDictionary.TryGetValue(UnderlyingNodeType, out var drawer)) {
                var editor = Activator.CreateInstance(drawer) as BaseEditorConstantNode;
                editor.UnderlyingRuntimeNode = ScriptableObject.CreateInstance(UnderlyingNodeType) as BaseRuntimeNode;

                NodeCreationResult result = new NodeCreationResult() {
                    Output = editor,
                    Result = true,
                    Request = this,
                };

                return result;
            }
        } else {
            if (_editorDictionary.TryGetValue(ExistInstance.NodeType, out var drawer)) {
                var editor = Activator.CreateInstance(drawer) as BaseEditorConstantNode;
                editor.UnderlyingRuntimeNode = ExistInstance;

                NodeCreationResult result = new NodeCreationResult() {
                    Output = editor,
                    Result = true,
                    Request = this,
                };

                return result;
            }
        }

        return new NodeCreationResult() { Result = false };
    }
}
public class FunctionalNodeCreationRequest : NodeCreationRequest {
    private static readonly Dictionary<Type, Type> _editorDictionary;
    static FunctionalNodeCreationRequest() {
        _editorDictionary = new Dictionary<Type, Type>();

        var cache = TypeCache.GetTypesDerivedFrom<BaseEditorFunctionalNode>();
        foreach (var type in cache) {
            var attr = type.GetCustomAttribute<DrawerForNodeAttribute>();

            if (attr != null) {
                if (_editorDictionary.TryGetValue(attr.Target, out var drawer)) {
                    Debug.LogWarning($"Constant node type {attr.Target.FullName} already has a drawer type of {drawer.FullName}");
                } else {
                    _editorDictionary.Add(attr.Target, type);
                }
            }
        }
    }

    private static Type GetFunctionalNodeCreationOfNodeType(Type nodeType) {
        if (_editorDictionary.TryGetValue(nodeType, out var outputType)) {
            return outputType;
        }

        var baseType = nodeType.BaseType;
        while (baseType != null) {
            _editorDictionary.TryGetValue(baseType, out outputType);
            if (outputType != null) {
                return outputType;
            }

            baseType = baseType.BaseType;
        }

        return outputType;
    }

    public override NodeCreationResult Handle() {
        var drawer = GetFunctionalNodeCreationOfNodeType(ExistInstance == null ? UnderlyingNodeType : ExistInstance.NodeType);

        if (drawer != null) {
            NodeCreationResult result = new NodeCreationResult() {
                Result = true,
                Request = this,
            };

            result.Output = Activator.CreateInstance(drawer) as BaseEditorFunctionalNode;
            result.Output.UnderlyingRuntimeNode = (ExistInstance == null ? ScriptableObject.CreateInstance(UnderlyingNodeType) : ExistInstance) as BaseRuntimeNode;

            return result;
        } else {
            NodeCreationResult result = new NodeCreationResult() {
                Output = new DynamicEditorFunctionalNode() { UnderlyingRuntimeNode = (ExistInstance == null ? ScriptableObject.CreateInstance(UnderlyingNodeType) : ExistInstance) as BaseRuntimeNode },
                Result = true,
                Request = this,
            };

            return result;
        }
    }
}
public class SequenceNodeCreationRequest : NodeCreationRequest {
    private static readonly Dictionary<Type, Type> _editorDictionary;
    static SequenceNodeCreationRequest() {
        _editorDictionary = new Dictionary<Type, Type>();

        var cache = TypeCache.GetTypesDerivedFrom<BaseEditorSequenceNode>();
        foreach (var type in cache) {
            var attr = type.GetCustomAttribute<DrawerForNodeAttribute>();

            if (attr != null) {
                if (_editorDictionary.TryGetValue(attr.Target, out var drawer)) {
                    Debug.LogWarning($"Constant node type {attr.Target.FullName} already has a drawer type of {drawer.FullName}");
                } else {
                    _editorDictionary.Add(attr.Target, type);
                }
            }
        }
    }

    private static Type GetSequenceNodeDrawer(Type nodeType) {
        if (_editorDictionary.TryGetValue(nodeType, out var outputType)) {
            return outputType;
        }

        var baseType = nodeType.BaseType;
        while (baseType != null) {
            _editorDictionary.TryGetValue(baseType, out outputType);
            if (outputType != null) {
                return outputType;
            }

            baseType = baseType.BaseType;
        }

        return outputType;
    }

    public override NodeCreationResult Handle() {
        var drawer = GetSequenceNodeDrawer(ExistInstance == null ? UnderlyingNodeType : ExistInstance.NodeType);

        if (drawer != null) {
            NodeCreationResult result = new NodeCreationResult() {
                Result = true,
                Request = this,
            };

            result.Output = Activator.CreateInstance(drawer) as BaseEditorSequenceNode;
            result.Output.UnderlyingRuntimeNode = (ExistInstance == null ? ScriptableObject.CreateInstance(UnderlyingNodeType) : ExistInstance) as BaseSequenceNode;

            return result;
        } else {
            NodeCreationResult result = new NodeCreationResult() {
                Output = new DynamicEditorSequenceNode() { UnderlyingRuntimeNode = (ExistInstance == null ? ScriptableObject.CreateInstance(UnderlyingNodeType) : ExistInstance) as BaseSequenceNode },
                Result = true,
                Request = this,
            };

            return result;
        }

        //NodeCreationResult result = new NodeCreationResult() {
        //    Output = new DynamicEditorSequenceNode() { UnderlyingRuntimeNode = (ExistInstance == null ? ScriptableObject.CreateInstance(UnderlyingNodeType) : ExistInstance) as BaseSequenceNode },
        //    Result = true,
        //    Request = this,
        //};

        //return result;
    }
}

public class NodeEmitter {
    private Queue<NodeCreationRequest> requestQueue = new Queue<NodeCreationRequest>();

    public void EnqueueRequest(NodeCreationRequest request) {
        requestQueue.Enqueue(request);
    }

    public int HandleRequests(NodeCreationResult[] buffer) {
        Array.Clear(buffer, 0, buffer.Length);

        int i = 0;

        while (i < buffer.Length && requestQueue.Count > 0) {
            var request = requestQueue.Dequeue();
            if (request == null) continue;

            buffer[i++] = request.Handle();
        }

        return i;
    }
}
