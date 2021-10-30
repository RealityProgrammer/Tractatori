using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddForceNode : BaseSequenceNode
{
    [field: SerializeField, ExpectedInputType(typeof(Rigidbody))] public FlowInput TargetRigidbody { get; set; } = FlowInput.Null;
    [field: SerializeField, ExpectedInputType(typeof(MVector))] public FlowInput Force { get; set; } = FlowInput.Null;

    private void Evaluate(NodeEvaluationInfo info) {
        if(TargetRigidbody.IsNull())
        {
            Debug.LogError("target rigidbody in ur addforce node is null assign it or ur entire project is messed up");
            return;
        }

        info.Container.EvaluateInput<Rigidbody>(TargetRigidbody).AddForce((Vector4)info.Container.EvaluateInput<MVector>(Force));
    }
}
