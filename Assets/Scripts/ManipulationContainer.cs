using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Manipulation Container", menuName = "RealityProgrammer/Manipulation/Manipulation Container")]
public class ManipulationContainer : ScriptableObject
{
    [field: SerializeField] public string EntrySequence { get; set; }
    [field: SerializeField] public List<BaseRuntimeNode> NodeContainer { get; set; }

    private string CurrentSequenceNode { get; set; }

    private NodeEvaluationInfo evaluationInfo;
    public ManipulationContainer() {
        NodeContainer = new List<BaseRuntimeNode>();

        evaluationInfo = new NodeEvaluationInfo(this);
    }

    public BaseRuntimeNode FindNode(string guid) {
        for (int i = 0; i < NodeContainer.Count; i++) {
            if (NodeContainer[i].GUID == guid) {
                return NodeContainer[i];
            }
        }

        return null;
    }

    public object EvaluateInput(FlowInput input) {
        var node = FindNode(input.GUID);

        return node.Invoke(evaluationInfo)[ManipulationUtilities.FindParameterOutputIndexOutOnly(node.NodeType, input.OutputIndex)];
    }

    private void EvaluateCurrentSequenceNode() {
        var node = FindNode(CurrentSequenceNode) as BaseSequenceNode;

        if (node == null) return;

        node.InvokeNodeEvaluateMethod(evaluationInfo);

        if (node.IsFinal) return;

        CurrentSequenceNode = node.Next.GUID;
        EvaluateCurrentSequenceNode();
    }

    public void StartEvaluate() {
        CurrentSequenceNode = EntrySequence;

        EvaluateCurrentSequenceNode();
    }
}