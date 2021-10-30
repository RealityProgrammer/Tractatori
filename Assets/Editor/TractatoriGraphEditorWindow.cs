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
using System.Collections.ObjectModel;

public class TractatoriGraphEditorWindow : EditorWindow
{
    public const string ObjectBindablePropertyUserData = "Object";
    public const string VectorBindablePropertyUserData = "Vector";

    public TractatoriGraphView GraphView { get; private set; }

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
        ConstructSubWindow();
    }

    private void Update() {
        if (CurrentEditingAsset == null) {
            Close();
            return;
        }
        GraphView.Update();
    }

    private void ConstructGraphView() {
        GraphView = new TractatoriGraphView() {
            name = "Graph View",
        };
        GraphView.Initialize(this);

        GraphView.StretchToParentSize();
        rootVisualElement.Add(GraphView);
    }

    private void ConstructToolbar() {
        var toolbar = new Toolbar();

        var saveButton = new Button(SaveGraph);
        saveButton.text = "Save";

        toolbar.Add(saveButton);

        rootVisualElement.Add(toolbar);
    }

    private TractatoriToolPanelElement _toolPanel;
    private void ConstructSubWindow() {
        _toolPanel = new TractatoriToolPanelElement(rootVisualElement);
        rootVisualElement.Add(_toolPanel);
    }

    public void CreateObjectBindableProperty(ObjectBindableProperty property) {
        CurrentEditingAsset.ObjectBindableProperties.Add(property);

        //CreateFieldForBindableProperty(property).Field.userData = ObjectBindablePropertyUserData;
    }

    public void CreateVectorBindableProperty(MVectorBindableProperty property) {
        CurrentEditingAsset.VectorBindableProperties.Add(property);

        //CreateFieldForBindableProperty(property).Field.userData = VectorBindablePropertyUserData;
    }

    private void SaveGraph() {
        List<Node> nodes = new List<Node>();
        GraphView.nodes.ToList(nodes);

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

        var container = CurrentEditingAsset.NodeContainer;
        for (int i = 0; i < container.Count; i++) {
            var runtimeNode = container[i];

            switch (runtimeNode) {
                case IConstantValueNode cn:
                    GraphView.NodeEmitter.EnqueueRequest(new ConstantNodeCreationRequest() {
                        ExistInstance = Instantiate(runtimeNode),
                        Position = runtimeNode.NodePosition,
                    });
                    break;

                case BaseSequenceNode sn:
                    GraphView.NodeEmitter.EnqueueRequest(new SequenceNodeCreationRequest() {
                        ExistInstance = Instantiate(runtimeNode),
                        Position = runtimeNode.NodePosition,
                    });
                    break;

                default:
                    if (runtimeNode == null) break;

                    GraphView.NodeEmitter.EnqueueRequest(new FunctionalNodeCreationRequest() {
                        ExistInstance = Instantiate(runtimeNode),
                        Position = runtimeNode.NodePosition,
                    });
                    break;
            }
        }

        while (GraphView.HandleNodeEmitRequests()) { }

        GraphView.BeginLoadGraph();

        if (CurrentEditingAsset.EntrySequence.GuidValid()) {
            if (GraphView.TrySearchEditorNode<BaseEditorNode>(CurrentEditingAsset.EntrySequence, out var entryNode)) {
                TractatoriEditorUtility.LinkPort(GraphView, GraphView.EntryNode.Q<TractatoriStandardPort>("output-port"), entryNode.Q<TractatoriStandardPort>(BaseEditorSequenceNode.PreviousPortName));
            }
        }

        var info = new GraphLoadInformation(CurrentEditingAsset, GraphView);

        GraphView.nodes.ForEach(baseNode => {
            BaseEditorNode editorNode = baseNode as BaseEditorNode;

            if (editorNode.IsEntryPoint) return;

            editorNode.HandleGraphLoad(info);
        });

        GraphView.FinishLoadGraph();

        _toolPanel.InitializeAllBindablePropertyFields();

        Debug.Log("Finish load graph");
    }

    void ConnectAllSequenceNodes(List<DynamicEditorSequenceNode> sequenceNodes, List<BaseEditorNode> allNodes) {
        for (int i = 0; i < sequenceNodes.Count; i++) {
            var sn = sequenceNodes[i];

            // Link Entries
            if (sn.UnderlyingRuntimeNode.GUID == CurrentEditingAsset.EntrySequence) {
                TractatoriEditorUtility.LinkPort(GraphView, GraphView.EntryNode.Q<TractatoriStandardPort>("output-port"), sn.Q<TractatoriStandardPort>(DynamicEditorSequenceNode.PreviousPortName));
            }

            // Link the flow inputs
            var properties = TractatoriEditorUtility.GetAllFlowInputs(sn.UnderlyingRuntimeNode.NodeType);

            foreach (var property in properties) {
                var flow = (FlowInput)property.Property.GetValue(sn.UnderlyingSequenceNode);

                if (!string.IsNullOrEmpty(flow.GUID)) {
                    if (GraphView.TrySearchEditorNode<BaseEditorNode>(flow.GUID, out var find)) {
                        TractatoriEditorUtility.LinkPort(GraphView, find.Query<TractatoriStandardPort>().Where(x => x.direction == Direction.Output && x.OutputIndex == flow.OutputIndex).First(), sn.Q<TractatoriStandardPort>(property.Property.Name));
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
                    TractatoriEditorUtility.LinkSequenceNodes(GraphView, sequenceNodes[i], sequenceNodes[j]);
                    break;
                }
            }
        }
    }

    private void OnDisable() {
        rootVisualElement.Remove(GraphView);
    }
}
