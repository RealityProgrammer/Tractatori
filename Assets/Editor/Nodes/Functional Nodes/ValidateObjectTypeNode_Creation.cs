using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Stopwatch = System.Diagnostics.Stopwatch;

using UnityAssembly = UnityEditor.Compilation.Assembly;
using SAssembly = System.Reflection.Assembly;

[DrawerForNode(typeof(ValidateObjectTypeNode))]
public class ValidateObjectTypeNode_Creation : BaseEditorFunctionalNode {
    static HashSet<SAssembly> allRuntimeAssemblies;

    static IEnumerable<Type> allValidatedTypes;

    static ValidateObjectTypeNode_Creation() {
        //var unityasms = UnityEditor.Compilation.CompilationPipeline.GetAssemblies(UnityEditor.Compilation.AssembliesType.Player);
        //var unityasms = UnityEditor.Compilation.CompilationPipeline.GetAssemblies(UnityEditor.Compilation.AssembliesType.PlayerWithoutTestAssemblies);

        //allRuntimeAssemblies = new HashSet<SAssembly>();

        //foreach (var asm in GetRuntimeAssemblies(unityasms)) {
        //    allRuntimeAssemblies.Add(asm);
        //}

        allValidatedTypes = TypeCache.GetTypesDerivedFrom<UnityEngine.Object>().Where(x => x.GetCustomAttribute<ObsoleteAttribute>() == null);
        //allValidatedTypes = Filter(allUnityObjectTypes);

        //Debug.Log(string.Join(", ", unityasms.Select(x => x.name)));
        //Debug.Log(AppDomain.CurrentDomain.GetAssemblies().Contains(typeof(Rigidbody).Assembly));
    }

    private ValidateObjectTypeNode Node {
        get => UnderlyingRuntimeNode as ValidateObjectTypeNode;
    }

    private Type currentType;
    private Button selectionButton;

    public override void Initialize() {
        base.Initialize();

        CreateDefaultInputPorts();

        var label = new Label("Type");
        label.style.marginLeft = 3;
        FieldContainer.Add(label);

        selectionButton = new Button(() => {
            GenericMenu menu = new GenericMenu();

            foreach (var type in allValidatedTypes) {
                var _type = type;
                menu.AddItem(new GUIContent(type.FullName.Replace('.', '/')), false, () => {
                    SelectType(_type);
                });
            }

            menu.ShowAsContext();
        });

        FieldContainer.Add(selectionButton);

        InitializeType();
        InitializeButtonText();

        GeneratePort();

        ApplyPortChange();
    }

    void InitializeType() {
        var nodeStr = Node.TypeString;

        if (!string.IsNullOrEmpty(nodeStr)) {
            currentType = Type.GetType(nodeStr);

            if (currentType == null) {
                Node.TypeString = string.Empty;
            }
        }
    }

    void InitializeButtonText() {
        selectionButton.text = currentType == null ? "<Undefined type>" : currentType.FullName;
    }

    void RemoveOldOutputPorts() {
        outputContainer.Query<TractatoriStandardPort>().ForEach(port => port.RemoveFromHierarchy());
    }

    void GeneratePort() {
        var evaluateCache = TractatoriRuntimeUtilities.GetEvaluateCache(UnderlyingRuntimeNode.NodeType);

        var nodeStr = Node.TypeString;
        if (!string.IsNullOrEmpty(nodeStr)) {
            var type = Type.GetType(nodeStr);

            if (type != null) {
                selectionButton.text = type.FullName;

                var port = GeneratePort(Direction.Output, Port.Capacity.Multi, currentType == null ? typeof(UnityEngine.Object) : currentType);
                port.portName = ObjectNames.NicifyVariableName(evaluateCache.Parameters[1].Name);
                port.name = evaluateCache.Parameters[1].Name;
                port.OutputIndex = evaluateCache.LayoutIndex[1];
                port.portColor = GetPortColor(port.portType);

                var callback = new NodeConnectionCallback() {
                    OnDropCallback = (graphView, edge) => {
                        var output = edge.output.node as BaseEditorNode;
                        var input = edge.input.node as BaseEditorNode;

                        var property = TractatoriEditorUtility.GetAllFlowInputs(input.UnderlyingRuntimeNode.NodeType).Where(x => x.Name == edge.input.name).FirstOrDefault();
                        if (property != null) {
                            property.SetValue(input.UnderlyingRuntimeNode, new FlowInput(output.UnderlyingRuntimeNode.GUID, port.OutputIndex));
                        } else {
                            Debug.LogWarning("Something went wrong while connecting Editor Node. Information: Tried to find Property " + edge.input.name + " of node type " + input.UnderlyingRuntimeNode.NodeType.AssemblyQualifiedName);
                        }
                    }
                };

                port.AddManipulator(new EdgeConnector<Edge>(callback));

                outputContainer.Add(port);
            }
        }
    }

    void ApplyPortChange() {
        RefreshExpandedState();
        RefreshPorts();
    }

    public static Type[] GetTypesFromAssembly(Type type) {
        SAssembly assem = SAssembly.GetAssembly(type);

        if (assem == null) {
            Debug.LogError("Cannot find assembly of " + type + "!");
        }

        return assem.GetTypes().Where(t => t.IsSubclassOf(type)).ToArray();
    }

    public static Type[] GetTypesFromAllAssemblies(Type type) {
        List<Type> types = new List<Type>();
        SAssembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

        if (assemblies == null) {
            Debug.LogError("Cannot find assembly of " + type + "!");
        }

        for (int i = 0; i < assemblies.Length; i++) {
            SAssembly assembly = assemblies[i];

            foreach (Type t in assembly.GetTypes()) {
                if (t.IsSubclassOf(type)) {
                    types.Add(t);
                }
            }
        }

        return types.ToArray();
    }

    void SelectType(Type type) {
        currentType = type;
        Node.TypeString = type.AssemblyQualifiedName;

        InitializeButtonText();
        RemoveOldOutputPorts();
        GeneratePort();

        ApplyPortChange();
    }

    static IEnumerable<Type> Filter(TypeCache.TypeCollection types) {
        foreach (var type in types) {
            if (allRuntimeAssemblies.Contains(type.Assembly)) {
                yield return type;
            }
        }
    }
    static IEnumerable<SAssembly> GetRuntimeAssemblies(UnityAssembly[] runtimeUnityAsm) {
        var domainAsms = AppDomain.CurrentDomain.GetAssemblies();
        var domainAsmPaths = new string[domainAsms.Length];

        for (int i = 0; i < domainAsms.Length; i++) {
            domainAsmPaths[i] = domainAsms[i].Location.Replace('\\', '/');
        }

        for (int i = 0; i < domainAsms.Length; i++) {
            var sasm = domainAsms[i];

            foreach (var uasm in runtimeUnityAsm) {
                if (domainAsmPaths[i].EndsWith(uasm.outputPath)) {
                    yield return sasm;
                }
            }
        }
    }
}
