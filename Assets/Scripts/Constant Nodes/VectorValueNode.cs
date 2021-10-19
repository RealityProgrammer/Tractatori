using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorValueNode : BaseRuntimeNode, IConstantValueNode
{
    public MVector Value;

    private void Evaluate([OutputLayoutIndex(0)] out MVector value) {
        value = Value;
    }
}