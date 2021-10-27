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
    private static List<Type> _allConstantNodes;
    private static List<SequenceNodeInfoCache> _allSequenceNodes;

    private class SequenceNodeInfoCache {
        public Type Type { get; private set; }
        public string Path { get; private set; }

        public SequenceNodeInfoCache(Type type) {
            Type = type;

            var pathAttribute = Type.GetCustomAttribute<CustomizeSearchPathAttribute>();
            Path = "Sequence/" + (pathAttribute == null ? (Type.Namespace ?? "").Replace(".", "/") + ObjectNames.NicifyVariableName(Type.Name) : pathAttribute.Path);
        }
    }
    private class NodeInfoCache {
        public Type Type { get; private set; }
        public string Path { get; private set; }

        public NodeInfoCache(Type type) {
            Type = type;

            var pathAttribute = Type.GetCustomAttribute<CustomizeSearchPathAttribute>();
            Path = pathAttribute == null ? "Uncategorized/" + (Type.Namespace ?? "").Replace(".", "/") + ObjectNames.NicifyVariableName(Type.Name) : pathAttribute.Path;
        }
    }

    private static List<NodeInfoCache> _remainNodes;

    private static Texture2D indent;

    private static readonly Type baseSequenceNodeType = typeof(BaseSequenceNode);
    private static readonly Type constantNodeType = typeof(IConstantValueNode);

    private readonly List<HashSet<string>> pathGroup = new List<HashSet<string>>();

    static bool initialized = false;
    static NodeSearchWindow() {
        _allRuntimeNodes = TypeCache.GetTypesDerivedFrom<BaseRuntimeNode>();
    }

    void OnEnable() {
        if (indent == null) {
            indent = new Texture2D(1, 1);
            indent.SetPixel(0, 0, Color.clear);
            indent.Apply();
        }

        if (!initialized) {
            initialized = true;

            _allConstantNodes = new List<Type>();
            _allSequenceNodes = new List<SequenceNodeInfoCache>();
            _remainNodes = new List<NodeInfoCache>();

            foreach (var nt in _allRuntimeNodes) {
                if (nt.IsAbstract) continue;

                if (constantNodeType.IsAssignableFrom(nt)) {
                    _allConstantNodes.Add(nt);
                    continue;
                }

                if (nt.IsSubclassOf(baseSequenceNodeType)) {
                    _allSequenceNodes.Add(new SequenceNodeInfoCache(nt));
                    continue;
                }

                _remainNodes.Add(new NodeInfoCache(nt));
            }

            _allSequenceNodes = _allSequenceNodes.OrderBy(x => x.Path).ToList();
            _remainNodes = _remainNodes.OrderBy(x => x.Path).ToList();
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
            AddSearchTreeEntryBasedOnPath(searchTree, type.Path, 2, new SequenceNodeCreationRequest() {
                UnderlyingNodeType = type.Type,
            });
        }

        foreach (var type in _remainNodes) {
            AddSearchTreeEntryBasedOnPath(searchTree, type.Path, 2, new FunctionalNodeCreationRequest() {
                UnderlyingNodeType = type.Type,
            });
        }

        searchTree.Add(new SearchTreeGroupEntry(new GUIContent("Constants"), 2));

        searchTree.Add(new SearchTreeEntry(new GUIContent("Vector (Float)", indent)) {
            userData = new ConstantNodeCreationRequest() {
                UnderlyingNodeType = typeof(VectorValueNode),
            },
            level = 3,
        });

        searchTree.Add(new SearchTreeEntry(new GUIContent("Vector (Integer)", indent)) {
            userData = new ConstantNodeCreationRequest() {
                UnderlyingNodeType = typeof(VectorIntValueNode),
            },
            level = 3,
        });

        searchTree.Add(new SearchTreeEntry(new GUIContent("String", indent)) {
            userData = new ConstantNodeCreationRequest() {
                UnderlyingNodeType = typeof(StringValueNode),
            },
            level = 3,
        });

        searchTree.Add(new SearchTreeEntry(new GUIContent("Boolean", indent)) {
            userData = new ConstantNodeCreationRequest() {
                UnderlyingNodeType = typeof(Boolean4ValueNode),
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
