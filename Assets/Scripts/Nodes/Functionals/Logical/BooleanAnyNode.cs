using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomizeSearchPath("Functionals/Logical/Boolean Any Node")]
public class BooleanAnyNode : BaseRuntimeNode
{
    [field: SerializeField, ExpectedInputType(typeof(Boolean4))]
    public FlowInput Boolean { get; set; }

    private void Evaluate(NodeEvaluationInfo info, [OutputLayoutIndex(0)] out Boolean4 output) {
        if (Boolean.IsNull()) {
            output = Boolean4.AllFalse;
            return;
        }

        bool b = info.Container.EvaluateInput<Boolean4>(Boolean).Any();
        output = new Boolean4(b, b, b, b);
    }
}
