using UnityEngine;

public class StoryNode : BaseSequenceNode
{
    [field: SerializeField, ExpectedInputType(typeof(string))] public FlowInput Text { get; set; }

    private void Evaluate(NodeEvaluationInfo info) {
        Debug.Log(info.Container.EvaluateInput<string>(Text));
    }
}
