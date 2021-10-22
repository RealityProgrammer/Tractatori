using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

public class NodeSearchWindow : ScriptableObject, ISearchWindowProvider {
    private STGraphEditorWindow _window;
    private STEditorGraphView _graphView;

    private static readonly TypeCache.TypeCollection _allRuntimeNodes;
    private static readonly List<Type> _allSequenceNodes;
    private static readonly List<Type> _allConstantNodes;

    private static readonly List<Type> _allFunctionalNode;

    private static Texture2D indent;

    private static readonly Type baseSequenceNodeType = typeof(BaseSequenceNode);
    private static readonly Type constantNodeType = typeof(IConstantValueNode);
    static NodeSearchWindow() {
        _allRuntimeNodes = TypeCache.GetTypesDerivedFrom<BaseRuntimeNode>();

        _allSequenceNodes = new List<Type>();
        _allConstantNodes = new List<Type>();
        _allFunctionalNode = new List<Type>();

        foreach (var nt in _allRuntimeNodes) {
            if (nt.IsAbstract) continue;

            if (nt.IsSubclassOf(baseSequenceNodeType)) {
                _allSequenceNodes.Add(nt);
                continue;
            }

            if (constantNodeType.IsAssignableFrom(nt)) {
                _allConstantNodes.Add(nt);
                continue;
            }

            _allFunctionalNode.Add(nt);
        }
    }

    void OnEnable() {
        if (indent == null) {
            indent = new Texture2D(1, 1);
            indent.SetPixel(0, 0, Color.clear);
            indent.Apply();
        }
    }

    public void Initialize(STGraphEditorWindow window, STEditorGraphView gv) {
        _window = window;
        _graphView = gv;
    }

    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context) {
        var tree = new List<SearchTreeEntry>(_allSequenceNodes.Count + _allFunctionalNode.Count + 16); // 16 as an placeholder value
        tree.Add(new SearchTreeGroupEntry(new GUIContent("Node Initialization"), 0));
        tree.Add(new SearchTreeGroupEntry(new GUIContent("Sequencing"), 1));

        foreach (var type in _allSequenceNodes) {
            var pathAttribute = type.GetCustomAttribute<SequenceNodeSearchPathAttribute>();
            string path = pathAttribute == null ? "" : pathAttribute.Path;

            AddSearchTreeEntryBasedOnPath(tree, path, 2, new GUIContent(ObjectNames.NicifyVariableName(type.Name), indent), new SequenceNodeCreationRequest() {
                UnderlyingNodeType = type,
            });
        }

        tree.Add(new SearchTreeGroupEntry(new GUIContent("Functional"), 1));
        foreach (var type in _allFunctionalNode) {
            tree.Add(new SearchTreeEntry(new GUIContent(ObjectNames.NicifyVariableName(type.Name), indent)) {
                userData = new FunctionalNodeCreationRequest() {
                    UnderlyingNodeType = type,
                },
                level = 2,
            });
        }

        tree.Add(new SearchTreeGroupEntry(new GUIContent("Constants"), 1));

        tree.Add(new SearchTreeEntry(new GUIContent("Vector (Float)", indent)) {
            userData = new ConstantNodeCreationRequest() {
                UnderlyingNodeType = typeof(VectorValueNode),
            },
            level = 2,
        });

        tree.Add(new SearchTreeEntry(new GUIContent("String", indent)) {
            userData = new ConstantNodeCreationRequest() {
                UnderlyingNodeType = typeof(StringValueNode),
            },
            level = 2,
        });

        return tree;
    }

    void AddSearchTreeEntryBasedOnPath(List<SearchTreeEntry> list, string path, int startLevel, GUIContent content, object userData) {
        var splitted = path.Split('/');
        int invalidCounter = 0;

        for (int i = 0; i < splitted.Length; i++) {
            if (string.IsNullOrEmpty(splitted[i]) || string.IsNullOrWhiteSpace(splitted[i])) {
                invalidCounter++;
                continue;
            }

            list.Add(new SearchTreeGroupEntry(new GUIContent(splitted[i]), startLevel + i - invalidCounter));
        }

        list.Add(new SearchTreeEntry(content) {
            level = startLevel + splitted.Length - invalidCounter,
            userData = userData,
        });
    }

    public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context) {
        Debug.Log("context.screenMousePosition: " + context.screenMousePosition);

        var worldMousePosition = _window.rootVisualElement.ChangeCoordinatesTo(_window.rootVisualElement.parent, context.screenMousePosition - _window.position.position);
        var localMousePosition = _graphView.contentViewContainer.WorldToLocal(worldMousePosition);

        switch (SearchTreeEntry.userData) {
            case NodeCreationRequest nodeCreation:
                nodeCreation.Position = localMousePosition;

                _graphView.NodeEmitter.EnqueueRequest(nodeCreation);
                return true;

            default: return false;
        }
    }
}
