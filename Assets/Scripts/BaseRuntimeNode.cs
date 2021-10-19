using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BaseRuntimeNode : ScriptableObject
{
    private Type _nodeType;
    public Type NodeType {
        get {
            if (_nodeType == null) {
                _nodeType = GetType();
            }

            return _nodeType;
        }
    }

    public string GUID { get; private set; }

    public BaseRuntimeNode() {
        GUID = Guid.NewGuid().ToString();
    }

    public virtual object[] Invoke(NodeEvaluationInfo container) {
        return this.InvokeNodeEvaluateMethod(container);
    }

    [field: SerializeField] public Vector2 NodePosition { get; set; }
}