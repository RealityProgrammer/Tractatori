using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityObjectNode : BaseRuntimeNode, IConstantValueNode
{
    [field: SerializeField]
    public Object Value;

    private void Evaluate(out Object output) {
        output = Value;
    }
}
