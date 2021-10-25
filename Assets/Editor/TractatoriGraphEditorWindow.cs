using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class TractatoriGraphEditorWindow : EditorWindow
{
    public const string ObjectBindablePropertyUserData = "Object";
    public const string VectorBindablePropertyUserData = "Vector";

    private TractatoriGraphView _graphView;

    public static ManipulationContainer OriginalAsset { get; private set; }
    public static ManipulationContainer CurrentEditingAsset { get; private set; }

    public static bool HasUnsavedChanges { get; private set; } = true;
    public static TractatoriGraphEditorWindow WindowInstance { get; private set; }

    [OnOpenAsset(1)]
    public static bool OnOpenAsset(int instanceID, int line) {
        if (Selection.activeObject is ManipulationContainer container) {
            if (!HasOpenInstances<TractatoriGraphEditorWindow>()) {
                WindowInstance = GetWindow<TractatoriGraphEditorWindow>("Story Tailor Graph");

                OriginalAsset = container;
                CurrentEditingAsset = Instantiate(container);
                WindowInstance.LoadGraph();

                return true;
            } else {
                Debug.LogWarning("Cannot open multiple graph window because one instance of it is already exist");

                return false;
            }
        }

        return false;
    }

    private void OnEnable() {
        ConstructGraphView();
        ConstructToolbar();
        ConstructBlackboard();
    }

    private void Update() {
        if (CurrentEditingAsset == null) {
            Close();
            return;
        }
        _graphView.Update();
    }

    private void ConstructGraphView() {
        _graphView = new TractatoriGraphView() {
            name = "Graph View",
        };
        _graphView.Initialize(this);

        _graphView.StretchToParentSize();
        rootVisualElement.Add(_graphView);
    }

    private void ConstructToolbar() {
        var toolbar = new Toolbar();

        var saveButton = new Button(SaveGraph);
        saveButton.text = "Save";

        toolbar.Add(saveButton);

        rootVisualElement.Add(toolbar);
    }

    public TBlackboardWrapper BlackboardWrapper { get; private set; }
    private void ConstructBlackboard() {
        BlackboardWrapper = new TBlackboardWrapper(_graphView, this);
    }

    public void CreateObjectBindableProperty(ObjectBindableProperty property) {
        CurrentEditingAsset.ObjectBindableProperties.Add(property);

        CreateFieldForBindableProperty(property).Field.userData = ObjectBindablePropertyUserData;
    }

    public void CreateVectorBindableProperty(MVectorBindableProperty property) {
        CurrentEditingAsset.VectorBindableProperties.Add(property);

        CreateFieldForBindableProperty(property).Field.userData = VectorBindablePropertyUserData;
    }

    public PropertyBindingField CreateFieldForBindableProperty(BaseBindableProperty property) {
        return new PropertyBindingField(property, BlackboardWrapper.Blackboard);
    }

    private void SaveGraph() {
        List<Node> nodes = new List<Node>();
        _graphView.nodes.ToList(nodes);

        CurrentEditingAsset.NodeContainer.Clear();
        CurrentEditingAsset.NodeContainer.Capacity = nodes.Count - 1;

        foreach (var node in nodes) {
            if (node is BaseEditorNode editorNode) {
                if (!editorNode.IsEntryPoint) {
                    editorNode.UnderlyingRuntimeNode.NodePosition = editorNode.GetPosition().position;
                    editorNode.UnderlyingRuntimeNode.name = editorNode.UnderlyingRuntimeNode.GUID;

                    CurrentEditingAsset.NodeContainer.Add(editorNode.UnderlyingRuntimeNode);
                }
            } else {
                Debug.LogWarning("Undefined node type of " + node.GetType().FullName + " which isn't the child of BaseEditorNode");
            }
        }

        string assetPath = AssetDatabase.GetAssetPath(OriginalAsset);

        EditorUtility.CopySerialized(CurrentEditingAsset, OriginalAsset);

        var oldAssets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
        foreach (var oldAsset in oldAssets) {
            if (oldAsset is BaseRuntimeNode) {
                AssetDatabase.RemoveObjectFromAsset(oldAsset);
            }
        }

        for (int i = 0; i < OriginalAsset.NodeContainer.Count; i++) {
            AssetDatabase.AddObjectToAsset(OriginalAsset.NodeContainer[i], assetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    private void LoadGraph() {
        Debug.Log("Loading Graph...");

        BlackboardWrapper.GenerateFields();

        var container = CurrentEditingAsset.NodeContainer;
        for (int i = 0; i < container.Count; i++) {
            var runtimeNode = container[i];

            switch (runtimeNode) {
                case IConstantValueNode cn:
                    _graphView.NodeEmitter.EnqueueRequest(new ConstantNodeCreationRequest() {
                        ExistInstance = Instantiate(runtimeNode),
                        Position = runtimeNode.NodePosition,
                    });
                    break;

                case BaseSequenceNode sn:
                    _graphView.NodeEmitter.EnqueueRequest(new SequenceNodeCreationRequest() {
                        ExistInstance = Instantiate(runtimeNode),
                        Position = runtimeNode.NodePosition,
                    });
                    break;

                default:
                    if (runtimeNode == null) break;

                    _graphView.NodeEmitter.EnqueueRequest(new FunctionalNodeCreationRequest() {
                        ExistInstance = Instantiate(runtimeNode),
                        Position = runtimeNode.NodePosition,
                    });
                    break;
            }
        }

        while (_graphView.HandleNodeEmitRequests()) { }

        var nodes = _graphView.nodes.ToList().Cast<BaseEditorNode>().ToList(); // Shut up, I use Linq for prototyping
        var sequenceNodes = nodes.Where(x => x is DynamicEditorSequenceNode).Cast<DynamicEditorSequenceNode>().ToList();
        var nonSequenceNodes = nodes.Where(x => !x.IsEntryPoint && !(x is DynamicEditorSequenceNode)).ToList();

        foreach (var node in nonSequenceNodes) {
            if (node.IsEntryPoint) continue;
            if (node.UnderlyingRuntimeNode == null) {
                Debug.LogWarning("An editor node with null runtime node discovered. This shouldn't happened.");
                continue;
            }

            var properties = TractatoriEditorUtility.GetAllFlowInputs(node.UnderlyingRuntimeNode.NodeType);

            foreach (var property in properties) {
                FlowInput flow = (FlowInput)property.GetValue(node.UnderlyingRuntimeNode);

                if (!string.IsNullOrEmpty(flow.GUID)) {
                    var findNode = FindEditorNode(nodes, flow.GUID);

                    if (findNode != null) {
                        var inputPort = node.Q<TractatoriStandardPort>(property.Name);
                        var outputPort = findNode.Query<TractatoriStandardPort>().Where(x => x.direction == Direction.Output && x.OutputIndex == flow.OutputIndex).First();

                        TractatoriEditorUtility.LinkPort(_graphView, outputPort, inputPort);
                    }
                }
            }
        }

        ConnectAllSequenceNodes(sequenceNodes, nodes);

        Debug.Log("Finish Loading Graph");
    }

    BaseEditorNode FindEditorNode<T>(List<T> allNodes, string guid) where T : BaseEditorNode {
        for (int i = 0; i < allNodes.Count; i++) {
            if (allNodes[i].IsEntryPoint) continue;

            if (allNodes[i].UnderlyingRuntimeNode.GUID == guid) return allNodes[i];
        }

        return null;
    }

    void ConnectAllSequenceNodes(List<DynamicEditorSequenceNode> sequenceNodes, List<BaseEditorNode> allNodes) {
        for (int i = 0; i < sequenceNodes.Count; i++) {
            var sn = sequenceNodes[i];

            // Link Entries
            if (sn.UnderlyingRuntimeNode.GUID == CurrentEditingAsset.EntrySequence) {
                TractatoriEditorUtility.LinkPort(_graphView, _graphView.EntryNode.Q<TractatoriStandardPort>("output-port"), sn.Q<TractatoriStandardPort>(DynamicEditorNode.PreviousPortName));
            }

            // Link the flow inputs
            var properties = TractatoriEditorUtility.GetAllFlowInputs(sn.UnderlyingRuntimeNode.NodeType);

            foreach (var property in properties) {
                var flow = (FlowInput)property.Property.GetValue(sn.UnderlyingSequenceNode);

                if (!string.IsNullOrEmpty(flow.GUID)) {
                    var find = FindEditorNode(allNodes, flow.GUID);

                    if (find != null) {
                        TractatoriEditorUtility.LinkPort(_graphView, find.Query<TractatoriStandardPort>().Where(x => x.direction == Direction.Output && x.OutputIndex == flow.OutputIndex).First(), sn.Q<TractatoriStandardPort>(property.Property.Name));
                    } else {
                        property.SetValue(sn.UnderlyingRuntimeNode, FlowInput.Null);
                    }
                }
            }

            if (sn.UnderlyingSequenceNode.IsFinal) {
                continue;
            }

            // Link the next sequence node
            for (int j = 0; j < sequenceNodes.Count; j++) {
                if (sn.UnderlyingSequenceNode.Next.GUID == sequenceNodes[j].UnderlyingSequenceNode.GUID) {
                    TractatoriEditorUtility.LinkSequenceNodes(_graphView, sequenceNodes[i], sequenceNodes[j]);
                    break;
                }
            }
        }
    }

    private void OnDisable() {
        rootVisualElement.Remove(_graphView);
    }
}
