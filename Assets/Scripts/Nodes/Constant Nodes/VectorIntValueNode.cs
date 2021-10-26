using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorIntValueNode : BaseRuntimeNode, IConstantValueNode {
    [field: SerializeField]
    public MVectorInt Value { get; set; } = MVectorInt.Zero1;

    private void Evaluate([OutputLayoutIndex(0)] out MVectorInt value) {
        value = Value;
    }
}
