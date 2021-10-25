using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

public class NodeSearchWindow : ScriptableObject, ISearchWindowProvider {
    private TractatoriGraphEditorWindow _window;
    private TractatoriGraphView _graphView;

    private static readonly TypeCache.TypeCollection _allRuntimeNodes;
    private static readonly List<Type> _allConstantNodes;
    private static readonly List<Type> _allSequenceNodes;
    private static readonly List<Type> _remainNodes;

    private static Texture2D indent;

    private static readonly Type baseSequenceNodeType = typeof(BaseSequenceNode);
    private static readonly Type constantNodeType = typeof(IConstantValueNode);

    private readonly List<HashSet<string>> pathGroup = new List<HashSet<string>>();

    static NodeSearchWindow() {
        _allRuntimeNodes = TypeCache.GetTypesDerivedFrom<BaseRuntimeNode>();

        _allConstantNodes = new List<Type>();
        _allSequenceNodes = new List<Type>();
        _remainNodes = new List<Type>();

        foreach (var nt in _allRuntimeNodes) {
            if (nt.IsAbstract) continue;

            if (constantNodeType.IsAssignableFrom(nt)) {
                _allConstantNodes.Add(nt);
                continue;
            }

            if (nt.IsSubclassOf(baseSequenceNodeType)) {
                _allSequenceNodes.Add(nt);
                continue;
            }

            _remainNodes.Add(nt);
        }
    }

    void OnEnable() {
        if (indent == null) {
            indent = new Texture2D(1, 1);
            indent.SetPixel(0, 0, Color.clear);
            indent.Apply();
        }
    }

    public void Initialize(TractatoriGraphEditorWindow window, TractatoriGraphView gv) {
        _window = window;
        _graphView = gv;
    }

    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context) {
        var searchTree = new List<SearchTreeEntry>();

        searchTree.Add(new SearchTreeGroupEntry(new GUIContent("Sequence"), 1));
        foreach (var type in _allSequenceNodes) {
            var pathAttribute = type.GetCustomAttribute<CustomizeSearchPathAttribute>();
            string path = "Sequence/" + (pathAttribute == null ? type.FullName.Replace(".", "/") : pathAttribute.Path);

            AddSearchTreeEntryBasedOnPath(searchTree, path, 2, new SequenceNodeCreationRequest() {
                UnderlyingNodeType = type,
            });
        }

        foreach (var type in _remainNodes) {
            var pathAttribute = type.GetCustomAttribute<CustomizeSearchPathAttribute>();
            string path = pathAttribute == null ? "Uncategorized/" + type.FullName.Replace(".", "/") : pathAttribute.Path;

            AddSearchTreeEntryBasedOnPath(searchTree, path, 2, new FunctionalNodeCreationRequest() {
                UnderlyingNodeType = type,
            });
        }

        searchTree.Add(new SearchTreeGroupEntry(new GUIContent("Constants"), 2));

        searchTree.Add(new SearchTreeEntry(new GUIContent("Vector (Float)", indent)) {
            userData = new ConstantNodeCreationRequest() {
                UnderlyingNodeType = typeof(VectorValueNode),
            },
            level = 3,
        });

        searchTree.Add(new SearchTreeEntry(new GUIContent("String", indent)) {
            userData = new ConstantNodeCreationRequest() {
                UnderlyingNodeType = typeof(StringValueNode),
            },
            level = 3,
        });

        foreach (var hs in pathGroup) {
            hs.Clear();
        }

        return searchTree;
    }

    private static readonly char[] Seperators = new char[] { '/' };
    void AddSearchTreeEntryBasedOnPath(List<SearchTreeEntry> list, string path, int startLevel, object userData) {
        path = path.Trim('/', ' ');

        string[] splitted = path.Split(Seperators, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < splitted.Length - 1; i++) {
            while (pathGroup.Count - 1 < i) {
                pathGroup.Add(new HashSet<string>());
            }

            if (!pathGroup[i].Contains(splitted[i])) {
                pathGroup[i].Add(splitted[i]);
                list.Add(new SearchTreeGroupEntry(new GUIContent(splitted[i]), startLevel + i));
            }
        }

        list.Add(new SearchTreeEntry(new GUIContent(splitted[splitted.Length - 1], indent)) {
            level = startLevel + splitted.Length - 1,
            userData = userData,
        });
    }

    public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context) {
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
