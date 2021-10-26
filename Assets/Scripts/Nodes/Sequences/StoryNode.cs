using UnityEngine;

public class StoryNode : BaseSequenceNode
{
    [field: SerializeField, ExpectedInputType(typeof(string))] public FlowInput Text { get; set; } = FlowInput.Null;

    private void Evaluate(NodeEvaluationInfo info) {
        Debug.Log(info.Container.EvaluateInput<string>(Text));
    }
}
