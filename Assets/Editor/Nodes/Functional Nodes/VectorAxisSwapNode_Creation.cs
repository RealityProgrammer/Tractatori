using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[DrawerForNode(typeof(VectorAxisSwapNode))]
public class VectorAxisSwapNode_Creation : BaseEditorFunctionalNode
{
    private VectorAxisSwapNode Node {
        get => UnderlyingRuntimeNode as VectorAxisSwapNode;
    }

    public override void Initialize() {
        base.Initialize();

        CreateDefaultInputPorts();
        CreateDefaultOutputPorts();

        var enumContainer = new VisualElement();
        enumContainer.style.flexDirection = FlexDirection.Row;

        EnumField fromField = new EnumField(Node.From);
        fromField.style.flexGrow = 1;
        fromField.RegisterValueChangedCallback((evt) => {
            Node.From = (VectorAxisSwapNode.AxisMode)evt.newValue;
        });

        EnumField toField = new EnumField(Node.To);
        toField.style.flexGrow = 1;
        toField.RegisterValueChangedCallback((evt) => {
            Node.To = (VectorAxisSwapNode.AxisMode)evt.newValue;
        });

        enumContainer.Add(fromField);
        enumContainer.Add(toField);

        enumContainer.style.marginTop = 3;
        enumContainer.style.marginBottom = 3;

        FieldContainer.Add(enumContainer);

        RefreshExpandedState();
        RefreshPorts();
    }
}
