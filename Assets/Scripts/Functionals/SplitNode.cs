using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitNode : BaseRuntimeNode
{
    [field: ExpectedInputType(typeof(MVector))]
    [field: SerializeField] public FlowInput Input { get; set; }

    private void Evaluate(NodeEvaluationInfo info, [OutputLayoutIndex(0)] out MVector x, [OutputLayoutIndex(1)] out MVector y, [OutputLayoutIndex(2)] out MVector z, [OutputLayoutIndex(3)] out MVector w) {
        MVector v = (MVector)info.Container.EvaluateInput(Input);

        x = new MVector(v.X, 0);
        y = new MVector(0, v.Y);
        z = new MVector(0, 0, v.Z);
        w = new MVector(0, 0, 0, v.W);
    }
}
