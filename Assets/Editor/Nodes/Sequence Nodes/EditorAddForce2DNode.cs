using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;

[DrawerForNode(typeof(AddForce2DNode))]
public class EditorAddForce2DNode : BaseEditorSequenceNode {
    public AddForce2DNode Node {
        get => UnderlyingRuntimeNode as AddForce2DNode;
    }

    public override void Initialize() {
        title = ObjectNames.NicifyVariableName(UnderlyingRuntimeNode.NodeType.Name);

        GenerateSequencePorts();
        CreateDefaultInputPorts();

        EnumField forceTypeField = new EnumField("Force Mode", Node.ForceMode);
        forceTypeField.RegisterValueChangedCallback((evt) => {
            Node.ForceMode = (ForceMode2D)evt.newValue;
        });

        var enumLabel = forceTypeField.Q<Label>(className: "unity-label").style;
        enumLabel.width = StyleKeyword.Auto;
        enumLabel.minWidth = StyleKeyword.Auto;
        enumLabel.flexBasis = StyleKeyword.Auto;
        enumLabel.flexGrow = 1;

        var enumInput = forceTypeField.Q(className: "unity-base-field__input").style;
        enumInput.width = StyleKeyword.Auto;
        enumInput.flexBasis = StyleKeyword.Auto;
        enumInput.flexGrow = 1;

        FieldContainer.Add(forceTypeField);

        base.Initialize();
    }
}
