using UnityEngine;

public class GameObjectSetActiveNode : BaseSequenceNode
{
    [field: SerializeField, ExpectedInputType(typeof(GameObject))] public FlowInput TargetGameObject { get; set; } = FlowInput.Null;
    [field: SerializeField, ExpectedInputType(typeof(Boolean4))] public FlowInput Value { get; set; } = FlowInput.Null;

    private void Evaluate(NodeEvaluationInfo info) {
        if(TargetGameObject.IsNull())
        {
            Debug.LogError("target gameobject in ur setactive node is null assign it or ur entire project is messed up");
            return;
        }

        info.Container.EvaluateInput<GameObject>(TargetGameObject).SetActive(info.Container.EvaluateInput<Boolean4>(Value).B1);
    }
}
