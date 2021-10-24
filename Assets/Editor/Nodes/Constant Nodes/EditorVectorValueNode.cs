using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;

[DrawerForConstantNode(typeof(VectorValueNode))]
public class EditorVectorValueNode : BaseEditorConstantNode {
    public override void Initialize() {
        title = "Vector Constant Node";

        base.Initialize();
    }

    public override void InitializeFields(VisualElement contents) {
        EnumField enumField = new EnumField("Axis Mode", VectorAxis.XYZW);
		enumField.name = "Axis Mode";

        enumField.SetValueWithoutNotify((VectorAxis)(((VectorValueNode)UnderlyingRuntimeNode).Value.Axis - 1));

        var style = enumField.Q(className: "unity-enum-field__input").style;
        style.position = Position.Absolute;
        style.left = new Length(50, LengthUnit.Percent);
        style.right = new Length(0, LengthUnit.Percent);

        enumField.RegisterValueChangedCallback((evt) => {
			var value = (VectorAxis)evt.newValue;
			var oldVector = ((VectorValueNode)UnderlyingRuntimeNode).Value;

            Debug.Log("Regenerate Axis Fields");

            ClearYZWFields();

            switch (value) {
				default:
                case VectorAxis.X:
					((VectorValueNode)UnderlyingRuntimeNode).Value = new MVector(oldVector.X);
					break;

				case VectorAxis.XY:
                    contents.Add(CreateYField());

					((VectorValueNode)UnderlyingRuntimeNode).Value = new MVector(oldVector.X, oldVector.Y);
					break;

				case VectorAxis.XYZ:
                    contents.Add(CreateYField());
                    contents.Add(CreateZField());

					((VectorValueNode)UnderlyingRuntimeNode).Value = new MVector(oldVector.X, oldVector.Y, oldVector.Z);
					break;

				case VectorAxis.XYZW:
                    contents.Add(CreateYField());
                    contents.Add(CreateZField());
                    contents.Add(CreateWField());

					((VectorValueNode)UnderlyingRuntimeNode).Value = new MVector(oldVector.X, oldVector.Y, oldVector.Z, oldVector.W);
					break;
			}
		});

		contents.Add(enumField);

        FloatField floatField = new FloatField("X");
        floatField.name = "X Field";
        floatField.SetValueWithoutNotify(((VectorValueNode)UnderlyingRuntimeNode).Value.X);

        floatField.RegisterValueChangedCallback((evt) => {
            ((VectorValueNode)UnderlyingRuntimeNode).Value = new MVector(evt.newValue);
        });
        floatField.style.height = new Length(EditorGUIUtility.singleLineHeight, LengthUnit.Pixel);

        var input = floatField.Q("unity-text-input");
        input.style.position = Position.Absolute;
        input.style.left = new Length(50, LengthUnit.Percent);
        input.style.right = new Length(0, LengthUnit.Percent);

        contents.Add(floatField);

        var enumValue = (VectorAxis)enumField.value;

        if (enumValue >= VectorAxis.XY) {
            contents.Add(CreateYField());
        }

        if (enumValue >= VectorAxis.XYZ) {
            contents.Add(CreateZField());
        }

        if (enumValue >= VectorAxis.XYZW) {
            contents.Add(CreateWField());
        }
    }

    private void ClearYZWFields() {
        var contents = contentContainer.Q("contents");

        var y = contents.Q("Y Field");
        var z = contents.Q("Z Field");
        var w = contents.Q("W Field");

        if (y != null) contents.Remove(y);
        if (z != null) contents.Remove(z);
        if (w != null) contents.Remove(w);
    }

    private FloatField CreateYField() {
        var floatField = new FloatField("Y");
        floatField.name = "Y Field";
        floatField.SetValueWithoutNotify(((VectorValueNode)UnderlyingRuntimeNode).Value.Y);

        floatField.RegisterValueChangedCallback((evt) => {
            var v = ((VectorValueNode)UnderlyingRuntimeNode).Value;
            v.Y = evt.newValue;
            ((VectorValueNode)UnderlyingRuntimeNode).Value = v;
        });
        floatField.style.height = new Length(EditorGUIUtility.singleLineHeight, LengthUnit.Pixel);

        var input = floatField.Q("unity-text-input");
        input.style.position = Position.Absolute;
        input.style.left = new Length(50, LengthUnit.Percent);
        input.style.right = new Length(0, LengthUnit.Percent);

        return floatField;
    }

    private FloatField CreateZField() {
        var floatField = new FloatField("Z");
        floatField.name = "Z Field";
        floatField.SetValueWithoutNotify(((VectorValueNode)UnderlyingRuntimeNode).Value.Z);

        floatField.RegisterValueChangedCallback((evt) => {
            var v = ((VectorValueNode)UnderlyingRuntimeNode).Value;
            v.Z = evt.newValue;
            ((VectorValueNode)UnderlyingRuntimeNode).Value = v;
        });
        floatField.style.height = new Length(EditorGUIUtility.singleLineHeight, LengthUnit.Pixel);

        var input = floatField.Q("unity-text-input");
        input.style.position = Position.Absolute;
        input.style.left = new Length(50, LengthUnit.Percent);
        input.style.right = new Length(0, LengthUnit.Percent);

        return floatField;
    }

    private FloatField CreateWField() {
        var floatField = new FloatField("W");
        floatField.name = "W Field";
        floatField.SetValueWithoutNotify(((VectorValueNode)UnderlyingRuntimeNode).Value.W);

        floatField.RegisterValueChangedCallback((evt) => {
            var v = ((VectorValueNode)UnderlyingRuntimeNode).Value;
            v.W = evt.newValue;
            ((VectorValueNode)UnderlyingRuntimeNode).Value = v;
        });
        floatField.style.height = new Length(EditorGUIUtility.singleLineHeight, LengthUnit.Pixel);

        var input = floatField.Q("unity-text-input");
        input.style.position = Position.Absolute;
        input.style.left = new Length(50, LengthUnit.Percent);
        input.style.right = new Length(0, LengthUnit.Percent);

        return floatField;
    }
}

[Flags]
public enum VectorAxis {
    X = 0,
    XY = 1,
	XYZ = 2,
	XYZW = 3,
}