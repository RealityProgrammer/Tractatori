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

public class STGraphEditorWindow : EditorWindow
{
    private STEditorGraphView _graphView;
    public static ManipulationContainer CurrentEditingAsset { get; private set; }

    [OnOpenAsset(1)]
    public static bool OnOpenAsset(int instanceID, int line) {
        if (Selection.activeObject is ManipulationContainer container) {
            if (!HasOpenInstances<STGraphEditorWindow>()) {
                var window = GetWindow<STGraphEditorWindow>("Story Tailor Graph");

                CurrentEditingAsset = container;
                window.LoadGraph();

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
        _graphView = new STEditorGraphView() {
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

    Blackboard blackboard;
    private void ConstructBlackboard() {
        blackboard = new Blackboard(_graphView);
        blackboard.title = "Properties";
        blackboard.subTitle = string.Empty;
        blackboard.scrollable = true;

        blackboard.addItemRequested += (bb) => {
            CreateBindingProperty(new ObjectBindableProperty() { Name = Guid.NewGuid().ToString().Replace('-', '\0').Substring(0, 8), Value = null });
        };
        blackboard.editTextRequested += (Blackboard bb, VisualElement ve, string newValue) => {
            var old = ((BlackboardField)ve).text;

            if (CurrentEditingAsset.BindingPropertyContainer.Any(x => x.Name == newValue)) {
                Debug.LogWarning("Binding Property with the name of \"" + newValue + "\" is already exist");
                EditorApplication.Beep();
                return;
            }

            var prop = CurrentEditingAsset.FindBindingProperty(old);
            if (prop != null) {
                prop.Name = newValue;
                ((BlackboardField)ve).text = newValue;
            } else {
                Debug.LogWarning("Binding Property with the name of \"" + newValue + "\" cannot be found. This is not supposed to happen normally.");
            }
        };

        blackboard.Add(new BlackboardSection());

        _graphView.Add(blackboard);
    }

    // For now, only Binding Property with UnityEngine.Object are allowed for prototyping purpose, other properties, such as Vector will be added later
    public void CreateBindingProperty(ObjectBindableProperty property) {
        CurrentEditingAsset.BindingPropertyContainer.Add(property);

        CreateFieldForBindingProperty(property);
    }

    void CreateFieldForBindingProperty(ObjectBindableProperty property) {
        new PropertyBindingField(property, blackboard);
    }

    void ChangeBindingPropertyName(string from, string to) {
        var p = CurrentEditingAsset.FindBindingProperty(from);

        if (p == null) {
            Debug.LogWarning("Cannot change binding property name from " + from + " to " + to);
        } else {
            p.Name = to;
        }
    }

    private void SaveGraph() {
        List<Node> nodes = new List<Node>();
        _graphView.nodes.ToList(nodes);

        string assetPath = AssetDatabase.GetAssetPath(CurrentEditingAsset);

        var oldAssets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
        foreach (var oldAsset in oldAssets) {
            if (oldAsset is BaseRuntimeNode) {
                AssetDatabase.RemoveObjectFromAsset(oldAsset);
            }
        }

        CurrentEditingAsset.NodeContainer.Clear();
        CurrentEditingAsset.NodeContainer.Capacity = nodes.Count - 1; // Minus one because one entry node exists

        foreach (var node in nodes) {
            if (node is BaseEditorNode editorNode) {
                if (!editorNode.EntryPoint) {
                    editorNode.UnderlyingRuntimeNode.NodePosition = editorNode.GetPosition().position;
                    editorNode.UnderlyingRuntimeNode.name = editorNode.UnderlyingRuntimeNode.GUID;
                    AssetDatabase.AddObjectToAsset(editorNode.UnderlyingRuntimeNode, CurrentEditingAsset);

                    CurrentEditingAsset.NodeContainer.Add(editorNode.UnderlyingRuntimeNode);
                }
            } else {
                Debug.LogWarning("Undefined node type of " + node.GetType().FullName + " which isn't the child of BaseEditorNode");
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void LoadGraph() {
        Debug.Log("Loading Graph...");

        foreach (var prop in CurrentEditingAsset.BindingPropertyContainer) {
            CreateFieldForBindingProperty(prop);
        }

        var container = CurrentEditingAsset.NodeContainer;
        for (int i = 0; i < container.Count; i++) {
            var runtimeNode = container[i];

            switch (runtimeNode) {
                case IConstantValueNode cn:
                    _graphView.NodeEmitter.EnqueueRequest(new ConstantNodeCreationRequest() {
                        ExistInstance = runtimeNode,
                        Position = runtimeNode.NodePosition,
                    });
                    break;

                case BaseSequenceNode sn:
                    _graphView.NodeEmitter.EnqueueRequest(new SequenceNodeCreationRequest() {
                        ExistInstance = runtimeNode,
                        Position = runtimeNode.NodePosition,
                    });
                    break;

                default:
                    _graphView.NodeEmitter.EnqueueRequest(new FunctionalNodeCreationRequest() {
                        ExistInstance = runtimeNode,
                        Position = runtimeNode.NodePosition,
                    });
                    break;
            }
        }

        while (_graphView.HandleNodeEmitRequests()) { }

        var nodes = _graphView.nodes.ToList().Cast<BaseEditorNode>().ToList(); // Shut up, I use Linq for prototyping
        var sequenceNodes = nodes.Where(x => x is DynamicEditorSequenceNode).Cast<DynamicEditorSequenceNode>().ToList();
        var nonSequenceNodes = nodes.Where(x => !x.EntryPoint && !(x is DynamicEditorSequenceNode)).ToList();

        foreach (var node in nonSequenceNodes) {
            if (node.EntryPoint) continue;
            if (node.UnderlyingRuntimeNode == null) {
                Debug.LogWarning("An editor node with null runtime node discovered. This shouldn't happened.");
                continue;
            }

            var fields = STEditorUtilities.GetAllFlowInputFields(node.UnderlyingRuntimeNode.NodeType);
            var properties = STEditorUtilities.GetAllFlowInputProperties(node.UnderlyingRuntimeNode.NodeType);

            foreach (var field in fields) {
                FlowInput flow = (FlowInput)field.GetValue(node.UnderlyingRuntimeNode);

                if (!string.IsNullOrEmpty(flow.GUID)) {
                    var findNode = FindEditorNode(nodes, flow.GUID);

                    if (findNode != null) {
                        var inputPort = node.Q<STNodePort>(field.Name);
                        var outputPort = findNode.Query<STNodePort>().Where(x => x.direction == Direction.Output && x.OutputIndex == flow.OutputIndex).First();

                        STEditorUtilities.LinkPort(_graphView, outputPort, inputPort);
                    }
                }
            }

            foreach (var property in properties) {
                FlowInput flow = (FlowInput)property.Property.GetValue(node.UnderlyingRuntimeNode);

                if (!string.IsNullOrEmpty(flow.GUID)) {
                    var findNode = FindEditorNode(nodes, flow.GUID);

                    if (findNode != null) {
                        var inputPort = node.Q<STNodePort>(property.Property.Name);
                        var outputPort = findNode.Query<STNodePort>().Where(x => x.direction == Direction.Output && x.OutputIndex == flow.OutputIndex).First();

                        STEditorUtilities.LinkPort(_graphView, outputPort, inputPort);
                    }
                }
            }
        }

        ConnectAllSequenceNodes(sequenceNodes, nodes);

        Debug.Log("Finish Loading Graph");
    }

    BaseEditorNode FindEditorNode<T>(List<T> allNodes, string guid) where T : BaseEditorNode {
        for (int i = 0; i < allNodes.Count; i++) {
            if (allNodes[i].EntryPoint) continue;

            if (allNodes[i].UnderlyingRuntimeNode.GUID == guid) return allNodes[i];
        }

        return null;
    }

    void ConnectAllSequenceNodes(List<DynamicEditorSequenceNode> sequenceNodes, List<BaseEditorNode> allNodes) {
        for (int i = 0; i < sequenceNodes.Count; i++) {
            var sn = sequenceNodes[i];

            // Link Entries
            if (sn.UnderlyingRuntimeNode.GUID == CurrentEditingAsset.EntrySequence) {
                STEditorUtilities.LinkPort(_graphView, _graphView.EntryNode.Q<STNodePort>("output-port"), sn.Q<STNodePort>(DynamicEditorNode.PreviousPortName));
            }

            // Link the flow inputs
            var fields = STEditorUtilities.GetAllFlowInputFields(sn.UnderlyingRuntimeNode.NodeType);
            var properties = STEditorUtilities.GetAllFlowInputProperties(sn.UnderlyingRuntimeNode.NodeType);

            foreach (var field in fields) {
                var flow = (FlowInput)field.GetValue(sn.UnderlyingSequenceNode);

                if (!string.IsNullOrEmpty(flow.GUID)) {
                    var find = FindEditorNode(allNodes, flow.GUID);

                    if (find != null) {
                        STEditorUtilities.LinkPort(_graphView, find.Query<STNodePort>().Where(x => x.direction == Direction.Output && x.OutputIndex == flow.OutputIndex).First(), sn.Q<STNodePort>(field.Name));
                    } else {
                        field.SetValue(sn.UnderlyingRuntimeNode, FlowInput.Null);
                    }
                }
            }

            foreach (var property in properties) {
                var flow = (FlowInput)property.Property.GetValue(sn.UnderlyingSequenceNode);

                if (!string.IsNullOrEmpty(flow.GUID)) {
                    var find = FindEditorNode(allNodes, flow.GUID);

                    if (find != null) {
                        STEditorUtilities.LinkPort(_graphView, find.Query<STNodePort>().Where(x => x.direction == Direction.Output && x.OutputIndex == flow.OutputIndex).First(), sn.Q<STNodePort>(property.Property.Name));
                    } else {
                        property.Property.SetValue(sn.UnderlyingRuntimeNode, FlowInput.Null);
                    }
                }
            }

            if (sn.UnderlyingSequenceNode.IsFinal) {
                continue;
            }

            // Link the next sequence node
            for (int j = 0; j < sequenceNodes.Count; j++) {
                if (sn.UnderlyingSequenceNode.Next.GUID == sequenceNodes[j].UnderlyingSequenceNode.GUID) {
                    STEditorUtilities.LinkSequenceNodes(_graphView, sequenceNodes[i], sequenceNodes[j]);
                    break;
                }
            }
        }
    }

    private void OnDisable() {
        rootVisualElement.Remove(_graphView);
    }
}
