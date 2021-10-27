using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boolean4ValueNode : BaseRuntimeNode, IConstantValueNode
{
    [field: SerializeField]
    public Boolean4 Value { get; set; }

    private void Evaluate([OutputLayoutIndex(0)] out Boolean4 value) {
        value = Value;
    }
}