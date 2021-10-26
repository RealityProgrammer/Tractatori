using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using UnityEditor.UIElements;

public class TractatoriGraphView : GraphView
{
    private NodeSearchWindow _searchWindow;
    private TractatoriGraphEditorWindow _window;

    public NodeEmitter NodeEmitter { get; private set; }

    public BaseEditorNode EntryNode { get; private set; }

    public TractatoriGraphView() {
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

        RegisterCallback<DragEnterEvent>(HandleDragEnter);
        RegisterCallback<DragUpdatedEvent>(HandleDragUpdated);
        RegisterCallback<DragPerformEvent>(HandleDragPerform);
        RegisterCallback<DragLeaveEvent>(HandleDragLeave);

        graphViewChanged = OnGraphChange;
    }

    private GraphViewChange OnGraphChange(GraphViewChange change) {
        if (change.elementsToRemove != null) {
            // If node get deleted, edge will be deleted first, then the node itself. So... we only need to process the edge

            foreach (GraphElement ge in change.elementsToRemove) {
                // 1 case here, if anything is added in the future, I don't have to reformat it
                switch (ge) {
                    case Edge edge:
                        var editorInputNode = edge.input.node as BaseEditorNode;
                        var editorOutputNode = edge.output.node as BaseEditorNode;

                        if (editorInputNode == null || editorOutputNode == null) break; // Guard check

                        var inputNode = editorInputNode.UnderlyingRuntimeNode;
                        var outputNode = editorOutputNode.UnderlyingRuntimeNode;

                        if (inputNode is BaseSequenceNode sequenceInput && outputNode is BaseSequenceNode sequenceOutput) {
                            sequenceOutput.Next = FlowInput.Null;
                            sequenceInput.Previous = FlowInput.Null;

                            break;
                        }

                        if (editorOutputNode.IsEntryPoint) {
                            TractatoriGraphEditorWindow.CurrentEditingAsset.EntrySequence = Guid.Empty.ToString();
                        } else {
                            var name = edge.input.name;

                            var property = TractatoriEditorUtility.GetAllFlowInputs(inputNode.NodeType).Where(x => x.Name == name).FirstOrDefault();

                            if (property != null) {
                                property.SetValue(inputNode, FlowInput.Null);
                            } else {
                                Debug.LogWarning("If you see this message. Report issue and send the container data.");
                            }
                        }
                        break;
                }
            }
        }

        return change;
    }

    private bool validate;
    private void HandleDragEnter(DragEnterEvent evt) {
        validate = DragAndDrop.objectReferences.Length == 1 && EditorUtility.IsPersistent(DragAndDrop.objectReferences[0]) && AssetDatabase.GetAssetPath(DragAndDrop.objectReferences[0]).StartsWith("Assets/Resources/");
    }
    private void HandleDragUpdated(DragUpdatedEvent evt) {
        if (validate) {
            DragAndDrop.visualMode = DragAndDropVisualMode.Move;
        } else {
            if (DragAndDrop.GetGenericData("DragSelection") is List<ISelectable> selection && (selection.OfType<BlackboardField>().Count() >= 0)) {
                DragAndDrop.visualMode = evt.actionKey ? DragAndDropVisualMode.Copy : DragAndDropVisualMode.Move;
            }
        }
    }
    private void HandleDragPerform(DragPerformEvent evt) {
        if (validate) {
            DragAndDrop.AcceptDrag();

            var req = new ConstantNodeCreationRequest();
            var un = ScriptableObject.CreateInstance<UnityObjectNode>();
            un.Value = DragAndDrop.objectReferences[0];
            req.ExistInstance = un;
            req.Position = viewTransform.matrix.inverse.MultiplyPoint(evt.localMousePosition);

            NodeEmitter.EnqueueRequest(req);
        } else {
            var dragSelection = DragAndDrop.GetGenericData("DragSelection");

            if (dragSelection != null) {
                var selectables = dragSelection as List<ISelectable>;
                var fields = selectables.OfType<BlackboardField>();

                int i = 0;
                foreach (BlackboardField field in fields) {
                    var req = new ConstantNodeCreationRequest();
                    BaseBindablePropertyNode nodeInstance = null;

                    switch ((string)field.userData) {
                        case TractatoriGraphEditorWindow.ObjectBindablePropertyUserData:
                            nodeInstance = ScriptableObject.CreateInstance<ObjectBindablePropertyNode>();
                            break;

                        case TractatoriGraphEditorWindow.VectorBindablePropertyUserData:
                            nodeInstance = ScriptableObject.CreateInstance<VectorBindablePropertyNode>();
                            break;
                    }

                    nodeInstance.Name = field.text;
                    req.ExistInstance = nodeInstance;
                    req.Position = viewTransform.matrix.inverse.MultiplyPoint(evt.localMousePosition) + i * 10 * Vector3.one;

                    NodeEmitter.EnqueueRequest(req);

                    i++;
                }
            }
        }
    }
    private void HandleDragLeave(DragLeaveEvent evt) {
        if (validate) {
            DragAndDrop.visualMode = DragAndDropVisualMode.None;
            validate = false;
        }
    }

    public void Initialize(TractatoriGraphEditorWindow window) {
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
            IsEntryPoint = true,
        };

        var port = node.GeneratePort(Direction.Output, Port.Capacity.Single, typeof(BaseSequenceNode)); // Float type as placeholder
        port.portName = "Initialization";
        port.name = "output-port";
        port.ConnectionCallback.OnDropCallback += (GraphView graphView, Edge edge) => {
            TractatoriGraphEditorWindow.CurrentEditingAsset.EntrySequence = ((BaseEditorNode)edge.input.node).UnderlyingRuntimeNode.GUID;
        };

        node.outputContainer.Add(port);

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

        if (startPort.direction == Direction.Output) {
            var outputPort = startPort as TractatoriStandardPort;

            ports.ForEach(port => {
                if (startPort == port || startPort.node == port.node || port.direction == Direction.Output) {
                    return;
                }

                if (port is TractatoriStandardPort inputPort) {
                    foreach (var type in inputPort.ExpectedTypes) {
                        if (type.IsAssignableFrom(outputPort.portType)) {
                            compatibles.Add(port);
                            break;
                        }
                    }
                }
            });
        } else {
            ports.ForEach(port => {
                if (startPort == port || startPort.node == port.node || port.direction == Direction.Output) {
                    return;
                }

                if (port is TractatoriStandardPort outputPort) {

                }
            });
            Debug.LogWarning("Connection from Input ports are currently being constructed");
        }

        return compatibles;
    }
}
