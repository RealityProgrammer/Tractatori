using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorValueNode : BaseRuntimeNode, IConstantValueNode
{
    [field: SerializeField]
    public MVector Value { get; set; } = MVector.Zero1;

    private void Evaluate([OutputLayoutIndex(0)] out MVector value) {
        value = Value;
    }
}