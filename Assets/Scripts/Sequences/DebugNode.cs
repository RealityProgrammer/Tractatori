using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugNode : BaseSequenceNode {
    [field: SerializeField, ExpectedInputType(typeof(object))] public FlowInput Input { get; set; }

    protected override void Evaluate(NodeEvaluationInfo info) {
        Debug.Log(info.Container.EvaluateInput<object>(Input));
    }
}