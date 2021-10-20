using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorValueNode : BaseRuntimeNode, IConstantValueNode
{
    [field: SerializeField]
    public MVector Value { get; set; }

    private void Evaluate([OutputLayoutIndex(0)] out MVector value) {
        value = Value;
    }
}