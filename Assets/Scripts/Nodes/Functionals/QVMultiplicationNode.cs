using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomizeSearchPath("Functionals/Mathematicals/Quaternion-Vector Multiplication Node")]
public class QVMultiplicationNode : BaseRuntimeNode
{
    [field: SerializeField, ExpectedInputType(typeof(MVector))]
    public FlowInput Quaternion { get; set; } = FlowInput.Null;

    [field: SerializeField, ExpectedInputType(typeof(MVector), typeof(MVectorInt))]
    public FlowInput Vector { get; set; } = FlowInput.Null;

    private void Evaluate(NodeEvaluationInfo info, [OutputLayoutIndex(0)] out MVector output) {
        if (Quaternion.IsNull()) {
            output = info.Container.EvaluateInput<MVector>(Vector);
            return;
        }

        MVector rotation = info.Container.EvaluateInput<MVector>(Quaternion);
        MVector point = info.Container.EvaluateInput<ITractatoriConvertible>(Vector).ToMVector();

        float x = rotation.X * 2;
        float y = rotation.Y * 2;
        float z = rotation.Z * 2;
        float xx = rotation.X * x;
        float yy = rotation.Y * y;
        float zz = rotation.Z * z;
        float xy = rotation.X * y;
        float xz = rotation.X * z;
        float yz = rotation.Y * z;
        float wx = rotation.W * x;
        float wy = rotation.W * y;
        float wz = rotation.W * z;

        output = MVector.Zero3;
        output.X = (1F - (yy + zz)) * point.X + (xy - wz) * point.Y + (xz + wy) * point.Z;
        output.Y = (xy + wz) * point.X + (1F - (xx + zz)) * point.Y + (yz - wx) * point.Z;
        output.Z = (xz - wy) * point.X + (yz + wx) * point.Y + (1F - (xx + yy)) * point.Z;
    }
}
