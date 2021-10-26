using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomizeSearchPath("Functionals/Mathematicals/Quaternion Multiplication Node")]
public class QQMultiplicationNode : BaseRuntimeNode
{
    [field: SerializeField, ExpectedInputType(typeof(MVector))]
    public FlowInput Left { get; set; } = FlowInput.Null;

    [field: SerializeField, ExpectedInputType(typeof(MVector))]
    public FlowInput Right { get; set; } = FlowInput.Null;

    private void Evaluate(NodeEvaluationInfo info, [OutputLayoutIndex(0)] out MVector output) {
        if (Left.IsNull() || Right.IsNull()) {
            output = MVector.QuaternionIdentity;
            return;
        }

        MVector lhs = info.Container.EvaluateInput<MVector>(Left);
        MVector rhs = info.Container.EvaluateInput<MVector>(Right);

        output = new MVector(
                lhs.W * rhs.X + lhs.X * rhs.W + lhs.Y * rhs.Z - lhs.Z * rhs.Y,
                lhs.W * rhs.Y + lhs.Y * rhs.W + lhs.Z * rhs.X - lhs.X * rhs.Z,
                lhs.W * rhs.Z + lhs.Z * rhs.W + lhs.X * rhs.Y - lhs.Y * rhs.X,
                lhs.W * rhs.W - lhs.X * rhs.X - lhs.Y * rhs.Y - lhs.Z * rhs.Z);
    }
}
