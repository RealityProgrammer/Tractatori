using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[DrawerForNode(typeof(StringValueNode))]
public class EditorStringValueNode : BaseEditorConstantNode {
    public override void Initialize() {
        title = "String Constant Node";

        base.Initialize();
    }

    public override void InitializeFields(VisualElement contents) {
        TextField textField = new TextField();
        textField.multiline = true;
        textField.SetValueWithoutNotify(((StringValueNode)UnderlyingRuntimeNode).Value);
        textField.RegisterValueChangedCallback((evt) => {
            ((StringValueNode)UnderlyingRuntimeNode).Value = evt.newValue;
        });

        textField.style.maxWidth = new Length(200, LengthUnit.Pixel);
        textField.style.width = StyleKeyword.Auto;

        var textInput = textField.Q("unity-text-input").style;
        textInput.unityTextAlign = TextAnchor.UpperLeft;
        textInput.whiteSpace = WhiteSpace.Normal;

        contents.Add(textField);

        contents.style.height = StyleKeyword.Auto;
    }
}
