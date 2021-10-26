using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpNode : BaseSequenceNode
{
    [field: SerializeField, ExpectedInputType(typeof(GameObject))] public FlowInput TargetObject { get; set; } = FlowInput.Null;
    [field: SerializeField, ExpectedInputType(typeof(MVector))] public FlowInput TargetPosition { get; set; } = FlowInput.Null;

    private void Evaluate(NodeEvaluationInfo info) {
        DoLerp(info.Container.EvaluateInput<GameObject>(TargetObject), info.Container.EvaluateInput<MVector>(TargetPosition));
    }

    void DoLerp(GameObject obj, MVector targetPos) {
        // TODO: do something here
    }
}
