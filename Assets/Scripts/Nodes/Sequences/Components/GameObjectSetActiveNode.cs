using UnityEngine;

public class GameObjectSetActiveNode : BaseSequenceNode
{
    [field: SerializeField, ExpectedInputType(typeof(GameObject))] public FlowInput TargetGameObject { get; set; }
    [field: SerializeField, ExpectedInputType(typeof(bool))] public FlowInput Value { get; set; }

    private void Evaluate(NodeEvaluationInfo info) {
        if(TargetGameObject.IsNull())
        {
            Debug.LogError("target gameobject in ur setactive node is null assign it or ur entire project is messed up");
            return;
        }

        info.Container.EvaluateInput<GameObject>(TargetGameObject).SetActive(info.Container.EvaluateInput<bool>(Value));
    }
}
