using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitNode : BaseSequenceNode
{
    [field: SerializeField, ExpectedInputType(typeof(MVector))]
    public FlowInput Duration { get; set; }

    private WaitForSeconds Evaluate(NodeEvaluationInfo info) {
        return new WaitForSeconds(info.Container.EvaluateInput<MVector>(Duration).X);
    }
}
