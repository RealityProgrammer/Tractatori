using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[DrawerForNode(typeof(BooleanSampleNode))]
public class BooleanSampleNode_Creation : BaseEditorFunctionalNode {
    private BooleanSampleNode Node {
        get => UnderlyingRuntimeNode as BooleanSampleNode;
    }

    public override void Initialize() {
        base.Initialize();

        CreateDefaultInputPorts();
        CreateDefaultOutputPorts();

        EnumField axisField = new EnumField("Axis", Node.Axis);
        axisField.style.flexGrow = 1;
        axisField.SetValueWithoutNotify(Node.Axis);
        axisField.RegisterValueChangedCallback((evt) => {
            Node.Axis = (BooleanSampleNode.SampleAxis)evt.newValue;
        });

        var label = axisField.Q(className: "unity-label").style;
        label.minWidth = StyleKeyword.Auto;
        label.flexGrow = 1;

        FieldContainer.Add(axisField);

        RefreshExpandedState();
        RefreshPorts();
    }
}
