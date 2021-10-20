using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddForceNode : BaseSequenceNode
{
    [field: SerializeField, ExpectedInputType(typeof(Rigidbody))] public FlowInput TargetRigidbody { get; set; }
    [field: SerializeField, ExpectedInputType(typeof(MVector))] public FlowInput Direction { get; set; }

    private void Evaluate(NodeEvaluationInfo info) {
        info.Container.EvaluateInput<Rigidbody>(TargetRigidbody).AddForce((Vector4)info.Container.EvaluateInput<MVector>(Direction));
    }
}
