using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomizeSearchPath("Functionals/Logical/Boolean All Node")]
public class BooleanAllNode : BaseRuntimeNode
{
    [field: SerializeField, ExpectedInputType(typeof(Boolean4))]
    public FlowInput Boolean { get; set; } = FlowInput.Null;

    private void Evaluate(NodeEvaluationInfo info, [OutputLayoutIndex(0)] out Boolean4 output) {
        if (Boolean.IsNull()) {
            output = Boolean4.AllFalse;
            return;
        }

        bool b = info.Container.EvaluateInput<Boolean4>(Boolean).All();
        output = new Boolean4(b, b, b, b);
    }
}
