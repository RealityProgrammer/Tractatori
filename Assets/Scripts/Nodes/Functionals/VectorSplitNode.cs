using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomizeSearchPath("Functionals/Miscs/Vector Split Node")]
public class VectorSplitNode : BaseRuntimeNode
{
    [field: ExpectedInputType(typeof(MVector), typeof(MVectorInt))]
    [field: SerializeField] public FlowInput Input { get; set; } = FlowInput.Null;

    private void Evaluate(NodeEvaluationInfo info, [OutputLayoutIndex(0)] out MVector x, [OutputLayoutIndex(1)] out MVector y, [OutputLayoutIndex(2)] out MVector z, [OutputLayoutIndex(3)] out MVector w) {
        MVector v = info.Container.EvaluateInput<ITractatoriConvertible>(Input).ToMVector();

        x = new MVector(v.X, 0);
        y = new MVector(0, v.Y);
        z = new MVector(0, 0, v.Z);
        w = new MVector(0, 0, 0, v.W);
    }
}
