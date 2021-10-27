using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[DrawerForNode(typeof(VectorIntValueNode))]
public class EditorVectorIntValueNode : BaseEditorConstantNode
{
    public override void Initialize() {
        title = "VectorInt Constant Node";

        base.Initialize();
    }

    public override void InitializeFields(VisualElement contents) {
        EnumField enumField = new EnumField("Axis Mode", VectorAxis.XYZW);
        enumField.name = "Axis Mode";

        enumField.SetValueWithoutNotify((VectorAxis)(((VectorIntValueNode)UnderlyingRuntimeNode).Value.Axis - 1));

        var style = enumField.Q(className: "unity-enum-field__input").style;
        style.position = Position.Absolute;
        style.left = new Length(50, LengthUnit.Percent);
        style.right = new Length(0, LengthUnit.Percent);

        enumField.RegisterValueChangedCallback((evt) => {
            var value = (VectorAxis)evt.newValue;
            var oldVector = ((VectorIntValueNode)UnderlyingRuntimeNode).Value;

            ClearYZWFields();

            switch (value) {
                default:
                case VectorAxis.X:
                    ((VectorIntValueNode)UnderlyingRuntimeNode).Value = new MVectorInt(oldVector.X);
                    break;

                case VectorAxis.XY:
                    contents.Add(CreateYField());

                    ((VectorIntValueNode)UnderlyingRuntimeNode).Value = new MVectorInt(oldVector.X, oldVector.Y);
                    break;

                case VectorAxis.XYZ:
                    contents.Add(CreateYField());
                    contents.Add(CreateZField());

                    ((VectorIntValueNode)UnderlyingRuntimeNode).Value = new MVectorInt(oldVector.X, oldVector.Y, oldVector.Z);
                    break;

                case VectorAxis.XYZW:
                    contents.Add(CreateYField());
                    contents.Add(CreateZField());
                    contents.Add(CreateWField());

                    ((VectorIntValueNode)UnderlyingRuntimeNode).Value = new MVectorInt(oldVector.X, oldVector.Y, oldVector.Z, oldVector.W);
                    break;
            }
        });

        contents.Add(enumField);

        IntegerField floatField = new IntegerField("X");
        floatField.name = "X Field";
        floatField.SetValueWithoutNotify(((VectorIntValueNode)UnderlyingRuntimeNode).Value.X);

        floatField.RegisterValueChangedCallback((evt) => {
            ((VectorIntValueNode)UnderlyingRuntimeNode).Value = new MVectorInt(evt.newValue);
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

    private IntegerField CreateYField() {
        var field = new IntegerField("Y");
        field.name = "Y Field";
        field.SetValueWithoutNotify(((VectorIntValueNode)UnderlyingRuntimeNode).Value.Y);

        field.RegisterValueChangedCallback((evt) => {
            var v = ((VectorIntValueNode)UnderlyingRuntimeNode).Value;
            v.Y = evt.newValue;
            ((VectorIntValueNode)UnderlyingRuntimeNode).Value = v;
        });
        field.style.height = new Length(EditorGUIUtility.singleLineHeight, LengthUnit.Pixel);

        var input = field.Q("unity-text-input");
        input.style.position = Position.Absolute;
        input.style.left = new Length(50, LengthUnit.Percent);
        input.style.right = new Length(0, LengthUnit.Percent);

        return field;
    }

    private IntegerField CreateZField() {
        var field = new IntegerField("Z");
        field.name = "Z Field";
        field.SetValueWithoutNotify(((VectorIntValueNode)UnderlyingRuntimeNode).Value.Z);

        field.RegisterValueChangedCallback((evt) => {
            var v = ((VectorIntValueNode)UnderlyingRuntimeNode).Value;
            v.Z = evt.newValue;
            ((VectorIntValueNode)UnderlyingRuntimeNode).Value = v;
        });
        field.style.height = new Length(EditorGUIUtility.singleLineHeight, LengthUnit.Pixel);

        var input = field.Q("unity-text-input");
        input.style.position = Position.Absolute;
        input.style.left = new Length(50, LengthUnit.Percent);
        input.style.right = new Length(0, LengthUnit.Percent);

        return field;
    }

    private IntegerField CreateWField() {
        var field = new IntegerField("W");
        field.name = "W Field";
        field.SetValueWithoutNotify(((VectorIntValueNode)UnderlyingRuntimeNode).Value.W);

        field.RegisterValueChangedCallback((evt) => {
            var v = ((VectorIntValueNode)UnderlyingRuntimeNode).Value;
            v.W = evt.newValue;
            ((VectorIntValueNode)UnderlyingRuntimeNode).Value = v;
        });
        field.style.height = new Length(EditorGUIUtility.singleLineHeight, LengthUnit.Pixel);

        var input = field.Q("unity-text-input");
        input.style.position = Position.Absolute;
        input.style.left = new Length(50, LengthUnit.Percent);
        input.style.right = new Length(0, LengthUnit.Percent);

        return field;
    }
}
