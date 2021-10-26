using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomizeSearchPath("Functionals/Mathematicals/Vector Negate Node")]
public class VectorNegateNode : BaseRuntimeNode {
    [field: ExpectedInputType(typeof(MVector), typeof(MVectorInt)), SerializeField] public FlowInput LeftHandSide { get; set; } = FlowInput.Null;

    private void Evaluate(NodeEvaluationInfo info, [OutputLayoutIndex(0)] out MVector output) {
        output = -info.Container.EvaluateInput<ITractatoriConvertible>(LeftHandSide).ToMVector();
    }
}
