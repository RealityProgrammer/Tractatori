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

    public T EvaluateInput<T>(FlowInput input) {
        return (T)EvaluateInput(input);
    }
	
    /// <summary>
    /// Evaluate the Node from GUID (See <see cref="FlowInput.GUID"/>) and return the output parameter with the ID number (See <see cref="FlowInput.OutputIndex"/> with <see cref="OutputLayoutIndexAttribute"/>)
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public object EvaluateInput(FlowInput input) {
        var node = FindNode(input.GUID);

        // If the node input is null, return the fallback default value of type
        if (node == null) {
            Debug.LogError("Node " + input.GUID + " is null");
            return null;
        }

        // Functional nodes don't use return value, so discard it
        var invokeReturn = node.Invoke(evaluationInfo, out _);

        return invokeReturn[ManipulationUtilities.FindParameterOutputIndexOutOnly(node.NodeType, input.OutputIndex)];
    }

    /// <summary>
    /// Evaluate the Node from GUID (See <see cref="FlowInput.GUID"/>) and return the output parameter with the ID number (See <see cref="FlowInput.OutputIndex"/> with <see cref="OutputLayoutIndexAttribute"/>). Return fallback value if anything go wrong (Ex: Node cannot be found).
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public object EvaluateInput(FlowInput input, object fallback) {
        var node = FindNode(input.GUID);

        // If the node input is null, return the fallback default value of type
        if (node == null) {
            return fallback;
        }

        // Functional nodes don't use return value, so discard it
        var invokeReturn = node.Invoke(evaluationInfo, out _);

        return invokeReturn[ManipulationUtilities.FindParameterOutputIndexOutOnly(node.NodeType, input.OutputIndex)];
    }

    private IEnumerator EvaluateCurrentSequenceNode() {
        var node = FindNode(CurrentSequenceNode) as BaseSequenceNode;

        if (node == null) yield break;

        node.InvokeNodeEvaluateMethod(evaluationInfo, out object ret);

        var cache = ManipulationUtilities.GetEvaluateCache(node.NodeType);

        if (cache.Method.ReturnType != ManipulationUtilities.VoidType) {
            yield return ret;
        }

        if (node.IsFinal) yield break;

        CurrentSequenceNode = node.Next.GUID;
        yield return EvaluateCurrentSequenceNode();
    }

    public IEnumerator StartEvaluate() {
        CurrentSequenceNode = EntrySequence;

        yield return EvaluateCurrentSequenceNode();
    }
}