using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[DrawerForNode(typeof(Boolean4ValueNode))]
public class EditorBoolean4ValueNode : BaseEditorConstantNode {
    public override void Initialize() {
        title = "Boolean Constant Node";

        base.Initialize();
    }

    public override void InitializeFields(VisualElement contents) {
        VisualElement toggleContainer = new VisualElement();
        toggleContainer.name = "toggle-container";
        toggleContainer.style.flexDirection = FlexDirection.Row;

        Label label = new Label("Value");
        label.style.flexGrow = 1;
        toggleContainer.Add(label);

        Toggle t1 = new Toggle();
        t1.style.flexGrow = 0;
        t1.SetValueWithoutNotify(((Boolean4ValueNode)UnderlyingRuntimeNode).Value.B1);
        t1.RegisterValueChangedCallback((evt) => {
            var n = (Boolean4ValueNode)UnderlyingRuntimeNode;

            var b = n.Value;
            b.B1 = evt.newValue;
            n.Value = b;
        });
        toggleContainer.Add(t1);

        Toggle t2 = new Toggle();
        t2.style.flexGrow = 0;
        t2.SetValueWithoutNotify(((Boolean4ValueNode)UnderlyingRuntimeNode).Value.B2);
        t2.RegisterValueChangedCallback((evt) => {
            var n = (Boolean4ValueNode)UnderlyingRuntimeNode;

            var b = n.Value;
            b.B2 = evt.newValue;
            n.Value = b;
        });
        toggleContainer.Add(t2);

        Toggle t3 = new Toggle();
        t3.style.flexGrow = 0;
        t3.SetValueWithoutNotify(((Boolean4ValueNode)UnderlyingRuntimeNode).Value.B3);
        t3.RegisterValueChangedCallback((evt) => {
            var n = (Boolean4ValueNode)UnderlyingRuntimeNode;

            var b = n.Value;
            b.B3 = evt.newValue;
            n.Value = b;
        });
        toggleContainer.Add(t3);

        Toggle t4 = new Toggle();
        t4.style.flexGrow = 0;
        t4.SetValueWithoutNotify(((Boolean4ValueNode)UnderlyingRuntimeNode).Value.B4);
        t4.RegisterValueChangedCallback((evt) => {
            var n = (Boolean4ValueNode)UnderlyingRuntimeNode;

            var b = n.Value;
            b.B4 = evt.newValue;
            n.Value = b;
        });
        toggleContainer.Add(t4);

        contents.Add(toggleContainer);
    }
}
