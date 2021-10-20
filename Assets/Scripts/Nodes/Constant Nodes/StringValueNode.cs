using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringValueNode : BaseRuntimeNode, IConstantValueNode
{
    [field: SerializeField]
    public string Value { get; set; }

    private void Evaluate(out string output) {
        output = Value;
    }
}
