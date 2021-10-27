using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomizeSearchPath("Functionals/Logical/Boolean Sample Node")]
public class BooleanSampleNode : BaseRuntimeNode {
    [field: SerializeField, ExpectedInputType(typeof(Boolean4))]
    public FlowInput Boolean { get; set; }

    [field: SerializeField] public SampleAxis Axis { get; set; }

    public enum SampleAxis { X, Y, Z, W }

    private void Evaluate(NodeEvaluationInfo info, [OutputLayoutIndex(0)] out Boolean4 output) {
        if (Boolean.IsNull()) {
            output = Boolean4.AllFalse;
            return;
        }

        bool b = info.Container.EvaluateInput<Boolean4>(Boolean)[(int)Axis];
        output = new Boolean4(b, b, b, b);
    }
}
