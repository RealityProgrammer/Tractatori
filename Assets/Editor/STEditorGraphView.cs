using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;

public class STEditorGraphView : GraphView
{
    private NodeSearchWindow _searchWindow;
    private STGraphEditorWindow _window;

    public NodeEmitter NodeEmitter { get; private set; }

    public BaseEditorNode EntryNode { get; private set; }

    public STEditorGraphView() {
        styleSheets.Add(Resources.Load<StyleSheet>("GridBackgroundSheet"));

        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var grid = new GridBackground();
        Insert(0, grid);
        grid.StretchToParentSize();

        EntryNode = GenerateEntryNode();
        AddElement(EntryNode);

        NodeEmitter = new NodeEmitter();
    }

    public void Initialize(STGraphEditorWindow window) {
        _window = window;
        AddSearchWindow();
    }

    private static NodeCreationResult[] _nodeBuffer = new NodeCreationResult[4];
    public void Update() {
        HandleNodeEmitRequests();
    }

    public bool HandleNodeEmitRequests() {
        int len = NodeEmitter.HandleRequests(_nodeBuffer);

        for (int i = 0; i < len; i++) {
            var node = _nodeBuffer[i];

            if (node.Result) {
                node.Output.SetPosition(new Rect(node.Request.Position, Vector2.zero));
                node.Output.Initialize();

                AddElement(node.Output);
            }
        }

        return len > 0;
    }

    void AddSearchWindow() {
        _searchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
        _searchWindow.Initialize(_window, this);

        nodeCreationRequest = (context) => {
            SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), _searchWindow);
        };
    }

    BaseEditorNode GenerateEntryNode() {
        var node = new BaseEditorNode() {
            title = "Entry Node",
            EntryPoint = true,
        };

        var port = node.GeneratePort(Direction.Output, Port.Capacity.Single, typeof(BaseSequenceNode)); // Float type as placeholder
        port.portName = "Initialization";
        port.name = "output-port";
        port.ConnectionCallback.OnDropCallback += (GraphView graphView, Edge edge) => {
            STGraphEditorWindow.CurrentEditingAsset.EntrySequence = ((BaseEditorNode)edge.input.node).UnderlyingRuntimeNode.GUID;
        };
        node.outputContainer.Add(port);
        node.RegisterCallback<MouseOverEvent>((evt) => {
            Debug.Log("Mouse Over kek");
        });

        node.capabilities &= ~Capabilities.Collapsible;
        node.capabilities &= ~Capabilities.Deletable;
        node.capabilities &= ~Capabilities.Renamable;
        node.capabilities &= ~Capabilities.Resizable;

        node.RefreshExpandedState();
        node.RefreshPorts();

        node.SetPosition(new Rect(0, 0, 100, 80));

        return node;
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter) {
        var compatibles = new List<Port>();

        ports.ForEach(port => {
            if (startPort == port || startPort.node == port.node || port.direction == startPort.direction) {
                return;
            }

            // Temporary fix
            if (!port.portType.IsAssignableFrom(startPort.portType)) {
                return;
            }

            compatibles.Add(port);
        });

        return compatibles;
    }
}
