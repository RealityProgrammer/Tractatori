using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringConcatNode : BaseRuntimeNode
{
    [field: SerializeField, ExpectedInputType(typeof(string))]
    public FlowInput StringInput { get; set; }

    [field: SerializeField, ExpectedInputType(typeof(object))]
    public FlowInput ObjectInput { get; set; }

    private void Evaluate(NodeEvaluationInfo info, [OutputLayoutIndex(0)] out string output) {
        output = (string)info.Container.EvaluateInput(StringInput) + info.Container.EvaluateInput(ObjectInput);
    }
}
