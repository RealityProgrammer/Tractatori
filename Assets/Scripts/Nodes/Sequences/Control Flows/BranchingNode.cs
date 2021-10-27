using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomizeSearchPath("Control Flow/Branching Node")]
public class BranchingNode : BaseSequenceNode
{
    [field: SerializeField, ExpectedInputType(typeof(Boolean4))]
    public FlowInput Predicate { get; set; }

    [field: SerializeField, ExcludeInput]
    public FlowInput False { get; set; }

    private void Evaluate(NodeEvaluationInfo info) {
        if (Predicate.IsNull() || False.IsNull()) return;

        bool value = info.Container.EvaluateInput<Boolean4>(Predicate).B1;

        if (!value) {
            info.Container.JumpSequenceNode = False.GUID;
        }
    }
}
