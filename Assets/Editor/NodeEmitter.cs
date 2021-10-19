using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private static readonly Dictionary<Type, Func<ConstantNodeCreationRequest, BaseEditorNode>> _constantNodeCreation = new Dictionary<Type, Func<ConstantNodeCreationRequest, BaseEditorNode>>() {
        [typeof(VectorValueNode)] = (creation) => new EditorVectorValueNode(ScriptableObject.CreateInstance(creation.UnderlyingNodeType) as VectorValueNode),
    };

    private static readonly Dictionary<Type, Func<ConstantNodeCreationRequest, BaseEditorNode>> _constantNodeCreationExists = new Dictionary<Type, Func<ConstantNodeCreationRequest, BaseEditorNode>>() {
        [typeof(VectorValueNode)] = (creation) => new EditorVectorValueNode(creation.ExistInstance as VectorValueNode),
    };

    public override NodeCreationResult Handle() {
        if (ExistInstance == null) {
            if (_constantNodeCreation.TryGetValue(UnderlyingNodeType, out var @delegate)) {
                NodeCreationResult result = new NodeCreationResult() {
                    Output = @delegate.Invoke(this),
                    Result = true,
                    Request = this,
                };

                return result;
            }
        } else {
            if (_constantNodeCreationExists.TryGetValue(ExistInstance.GetType(), out var @delegate)) {
                NodeCreationResult result = new NodeCreationResult() {
                    Output = @delegate.Invoke(this),
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
