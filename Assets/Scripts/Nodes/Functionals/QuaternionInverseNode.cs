using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomizeSearchPath("Functionals/Mathematicals/Quaternion Inverse Node")]
public class QuaternionInverseNode : BaseRuntimeNode
{
    [field: ExpectedInputType(typeof(MVector)), SerializeField] public FlowInput Input { get; set; } = FlowInput.Null;

    private void Evaluate(NodeEvaluationInfo info, [OutputLayoutIndex(0)] out MVector output) {
        if (Input.IsNull()) {
            output = MVector.QuaternionIdentity;
            return;
        }

        var vector = info.Container.EvaluateInput<MVector>(Input);
        var inverse = Quaternion.Inverse(new Quaternion(vector.X, vector.Y, vector.Z, vector.W));

        output = new MVector(inverse.x, inverse.y, inverse.z, inverse.w);
    }
}
