using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Manipulation Container", menuName = "RealityProgrammer/Manipulation/Manipulation Container")]
public class ManipulationContainer : ScriptableObject
{
    [field: SerializeField] public string EntrySequence { get; set; }
    [field: SerializeField] public List<BaseRuntimeNode> NodeContainer { get; set; }

    #region Bindable Properties
    [field: SerializeField] public List<ObjectBindableProperty> ObjectBindableProperties { get; set; }
    [field: SerializeField] public List<MVectorBindableProperty> VectorBindableProperties { get; set; }
    [field: SerializeField] public List<BooleanBindableProperty> BooleanBindableProperties { get; set; }
    #endregion

    #region Override Bindable Properties
    public List<ObjectBindableProperty> OverrideObjectBindableProperties { get; set; }
    public List<MVectorBindableProperty> OverrideVectorBindableProperties { get; set; }
    public List<BooleanBindableProperty> OverrideBooleanBindableProperties { get; set; }
    #endregion

    public string CurrentSequenceNode { get; private set; }
    public string JumpSequenceNode { get; set; }

    private NodeEvaluationInfo evaluationInfo;
    public ManipulationContainer() {
        NodeContainer = new List<BaseRuntimeNode>();

        evaluationInfo = new NodeEvaluationInfo(this);

        OverrideObjectBindableProperties = new List<ObjectBindableProperty>();
        OverrideVectorBindableProperties = new List<MVectorBindableProperty>();
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
        var evaluated = EvaluateInput(input);

        return (T)evaluated;
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

        return invokeReturn[TractatoriRuntimeUtilities.FindParameterOutputIndexOutOnly(node.NodeType, input.OutputIndex)];
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

        // Functional nodes don't use method with return value (only out parameter), so discard it
        var invokeReturn = node.Invoke(evaluationInfo, out _);

        return invokeReturn[TractatoriRuntimeUtilities.FindParameterOutputIndexOutOnly(node.NodeType, input.OutputIndex)];
    }

    public ObjectBindableProperty FindObjectBindableProperty(string name) {
        return FindProperty(ObjectBindableProperties, OverrideObjectBindableProperties, name);
    }

    public MVectorBindableProperty FindVectorBindableProperty(string name) {
        return FindProperty(VectorBindableProperties, OverrideVectorBindableProperties, name);
    }

    public BooleanBindableProperty FindBooleanBindableProperty(string name) {
        return FindProperty(BooleanBindableProperties, OverrideBooleanBindableProperties, name);
    }

    public T FindProperty<T>(List<T> properties, List<T> overrideProperties, string name) where T : BaseBindableProperty {
        var original = properties.FirstOrDefault(x => x.Name == name);

        if (original == null) {
            return null;
        }

        if (original.Overridable) {
            var @override = overrideProperties.FirstOrDefault(x => x.Name == name);

            if (@override != null) {
                return @override;
            }
        }

        return original;
    }

    public T FindRawProperty<T>(List<T> properties, string name) where T : BaseBindableProperty {
        return properties.FirstOrDefault(x => x.Name == name);
    }

    public bool RemoveBindingProperty(string name) {
        for (int i = 0; i < ObjectBindableProperties.Count; i++) {
            if (ObjectBindableProperties[i].Name == name) {
                ObjectBindableProperties.RemoveAt(i);
                return true;
            }
        }

        return false;
    }

    private IEnumerator EvaluateCurrentSequenceNode() {
        var node = FindNode(CurrentSequenceNode) as BaseSequenceNode;

        if (node == null) yield break;

        node.InvokeNodeEvaluateMethod(evaluationInfo, out object ret);

        var cache = TractatoriRuntimeUtilities.GetEvaluateCache(node.NodeType);

        if (cache.Method.ReturnType != TractatoriRuntimeUtilities.VoidType) {
            yield return ret;
        }

        if (node.IsFinal) yield break;

        if (string.IsNullOrEmpty(JumpSequenceNode)) {
            CurrentSequenceNode = node.Next.GUID;
        } else {
            CurrentSequenceNode = JumpSequenceNode;
            JumpSequenceNode = string.Empty;
        }

        yield return EvaluateCurrentSequenceNode();
    }

    public IEnumerator StartEvaluate() {
        CurrentSequenceNode = EntrySequence;

        yield return EvaluateCurrentSequenceNode();
    }
}