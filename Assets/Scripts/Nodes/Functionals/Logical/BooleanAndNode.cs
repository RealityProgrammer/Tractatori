using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomizeSearchPath("Functionals/Logical/Boolean Bitwise AND Node")]
public class BooleanAndNode : BaseRuntimeNode
{
    [field: SerializeField, ExpectedInputType(typeof(Boolean4))]
    public FlowInput Left { get; set; } = FlowInput.Null;

    [field: SerializeField, ExpectedInputType(typeof(Boolean4))]
    public FlowInput Right { get; set; } = FlowInput.Null;

    private void Evaluate(NodeEvaluationInfo info, [OutputLayoutIndex(0)] out Boolean4 output) {
        if (Left.IsNull() || Right.IsNull()) {
            output = Boolean4.AllFalse;
            return;
        }

        Boolean4 l = info.Container.EvaluateInput<Boolean4>(Left);
        Boolean4 r = info.Container.EvaluateInput<Boolean4>(Right);

        output = l & r;
    }
}
