using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorScaleNode : BaseRuntimeNode {
    [field: ExpectedInputType(typeof(MVector)), SerializeField] public FlowInput LeftHandSide { get; set; }
    [field: ExpectedInputType(typeof(MVector)), SerializeField] public FlowInput RightHandSide { get; set; }

    private void Evaluate(NodeEvaluationInfo info, [OutputLayoutIndex(0)] out MVector output) {
        output = (MVector)info.Container.EvaluateInput(LeftHandSide) * ((MVector)info.Container.EvaluateInput(LeftHandSide)).X;
    }
}
