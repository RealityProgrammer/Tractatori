using System;
using System.Collections.Generic;
using UnityEngine;

[CustomizeSearchPath("Functionals/Mathematicals/Vector Normalize Node")]
public class VectorNormalizeNode : BaseRuntimeNode
{
    [field: ExpectedInputType(typeof(MVector), typeof(MVectorInt)), SerializeField] public FlowInput Input { get; set; } = FlowInput.Null;

    private void Evaluate(NodeEvaluationInfo info, [OutputLayoutIndex(0)] out MVector output) {
        if (Input.IsNull()) {
            output = MVector.Zero1;
            return;
        }

        var vector = info.Container.EvaluateInput<ITractatoriConvertible>(Input).ToMVector();
        var mag = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z + vector.W * vector.W);

        output = vector / mag;
    }
}
