using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorNegateNode : BaseRuntimeNode {
    [field: ExpectedInputType(typeof(MVector)), SerializeField] public FlowInput LeftHandSide { get; set; }

    private void Evaluate(NodeEvaluationInfo info, [OutputLayoutIndex(0)] out MVector output) {
        output = -(MVector)info.Container.EvaluateInput(LeftHandSide);
    }
}
