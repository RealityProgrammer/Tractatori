using System;
using System.Collections.Generic;
using UnityEngine;

[CustomizeSearchPath("Functionals/Miscs/Vector Swap Axis Node")]
public class VectorAxisSwapNode : BaseRuntimeNode
{
    public enum AxisMode {
        X, Y, Z, W
    }

    [field: SerializeField, ExpectedInputType(typeof(MVector), typeof(MVectorInt))]
    public FlowInput Input { get; set; } = FlowInput.Null;

    [field: SerializeField]
    public AxisMode From { get; set; }

    [field: SerializeField]
    public AxisMode To { get; set; }

    private void Evaluate(NodeEvaluationInfo info, [OutputLayoutIndex(0)] out MVector output) {
        if (Input.IsNull()) {
            output = MVector.Zero4;
            return;
        }

        output = info.Container.EvaluateInput<ITractatoriConvertible>(Input).ToMVector();

        float temp = output[(int)From];
        output[(int)From] = output[(int)To];
        output[(int)To] = temp;
    }
}
