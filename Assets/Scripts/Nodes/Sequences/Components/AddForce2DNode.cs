using UnityEngine;

public class AddForce2DNode : BaseSequenceNode
{
    [field: SerializeField, ExpectedInputType(typeof(Rigidbody2D))] public FlowInput TargetRigidbody { get; set; } = FlowInput.Null;
    [field: SerializeField, ExpectedInputType(typeof(MVector))] public FlowInput Force { get; set; } = FlowInput.Null;
    [field: SerializeField] public ForceMode2D ForceMode { get; set; }

    private void Evaluate(NodeEvaluationInfo info) {
        if(TargetRigidbody.IsNull())
        {
            Debug.LogError("target rigidbody in ur addforce node is null assign it or ur entire project is messed up");
            return;
        }

        info.Container.EvaluateInput<Rigidbody2D>(TargetRigidbody).AddForce((Vector4)info.Container.EvaluateInput<MVector>(Force), ForceMode);
    }
}
