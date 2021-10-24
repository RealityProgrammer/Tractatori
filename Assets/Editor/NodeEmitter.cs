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
            var attr = type.GetCustomAttribute<DrawerForConstantNodeAttribute>();

            if (attr == null) {
                Debug.LogWarning("An editor type derived from BaseEditorConstantNode but without attribute [DrawerForConstantNode] detected: " + type.FullName);
            } else {
                if (_editorDictionary.TryGetValue(attr.RuntimeType, out var drawer)) {
                    Debug.LogWarning($"Constant node type {attr.RuntimeType.FullName} already has a drawer type of {drawer.FullName}");
                } else {
                    if (attr.RuntimeType.GetInterfaces().Contains(ConstantInterfaceType)) {
                        _editorDictionary.Add(attr.RuntimeType, type);
                    } else {
                        Debug.Log($"An editor type derived from BaseEditorConstantNode with attribute [DrawerForConstantNode] target to non-constant node type. Drawer type {drawer.FullName} targeted {attr.RuntimeType.FullName}");
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
    public override NodeCreationResult Handle() {
        NodeCreationResult result = new NodeCreationResult() {
            Output = new DynamicEditorNode((ExistInstance == null ? ScriptableObject.CreateInstance(UnderlyingNodeType) : ExistInstance) as BaseRuntimeNode),
            Result = true,
            Request = this,
        };

        return result;
    }
}

public class SequenceNodeCreationRequest : NodeCreationRequest {
    public override NodeCreationResult Handle() {
        NodeCreationResult result = new NodeCreationResult() {
            Output = new DynamicEditorSequenceNode((ExistInstance == null ? ScriptableObject.CreateInstance(UnderlyingNodeType) : ExistInstance) as BaseSequenceNode),
            Result = true,
            Request = this,
        };

        return result;
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
