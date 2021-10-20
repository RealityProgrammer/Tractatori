using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VVAdditionNode : BaseRuntimeNode {
    [field: ExpectedInputType(typeof(MVector)), SerializeField] public FlowInput LeftHandSide { get; set; }
    [field: ExpectedInputType(typeof(MVector)), SerializeField] public FlowInput RightHandSide { get; set; }

    private void Evaluate(NodeEvaluationInfo info, [OutputLayoutIndex(0)] out MVector output) {
        output = info.Container.EvaluateInput<MVector>(LeftHandSide) + info.Container.EvaluateInput<MVector>(RightHandSide);
    }
}
