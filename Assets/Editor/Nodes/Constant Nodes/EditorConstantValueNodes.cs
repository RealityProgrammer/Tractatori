using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;

public class EditorVectorValueNode : BaseEditorConstantNode {
    public EditorVectorValueNode(VectorValueNode node) : base() {
        UnderlyingRuntimeNode = node;
    }

    public override void Initialize() {
        title = "Vector Constant Node";

        base.Initialize();
    }

    public override void InitializeFields(VisualElement contents) {
        EnumField enumField = new EnumField("Axis Mode", VectorAxis.XYZW);
		enumField.name = "Axis Mode";
		enumField.RegisterValueChangedCallback((evt) => {
			var value = (VectorAxis)evt.newValue;
			var oldVector = ((VectorValueNode)UnderlyingRuntimeNode).Value;
			
			int axisCount = 1;
			switch (value) {
				default:
					((VectorValueNode)UnderlyingRuntimeNode).Value = new MVector(oldVector.X);
					break;
				case VectorAxis.XY:
					((VectorValueNode)UnderlyingRuntimeNode).Value = new MVector(oldVector.X, oldVector.Y);
					break;
				case VectorAxis.XYZ:
					((VectorValueNode)UnderlyingRuntimeNode).Value = new MVector(oldVector.X, oldVector.Y, oldVector.Z);
					break;
				case VectorAxis.XYZW:
					((VectorValueNode)UnderlyingRuntimeNode).Value = new MVector(oldVector.X, oldVector.Y, oldVector.Z, oldVector.W);
					break;
			}
		});
		
		//enumField.style.position = Position.Absolute;
        //enumField.style.left = new Length(50, LengthUnit.Percent);
        //enumField.style.right = new Length(0, LengthUnit.Percent);
		
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
		
        //floatField = new FloatField("Y");
        //floatField.name = "Y Field";
        //floatField.SetValueWithoutNotify(((VectorValueNode)UnderlyingRuntimeNode).Value.y);

        //floatField.RegisterValueChangedCallback((evt) => {
        //    ((VectorValueNode)UnderlyingRuntimeNode).Value.y = evt.newValue;
        //});
        //floatField.style.height = new Length(EditorGUIUtility.singleLineHeight, LengthUnit.Pixel);

        //input = floatField.Q("unity-text-input");
        //input.style.position = Position.Absolute;
        //input.style.left = new Length(50, LengthUnit.Percent);
        //input.style.right = new Length(0, LengthUnit.Percent);

        //contents.Add(floatField);

        //floatField = new FloatField("Z");
        //floatField.name = "Z Field";
        //floatField.SetValueWithoutNotify(((VectorValueNode)UnderlyingRuntimeNode).Value.z);

        //floatField.RegisterValueChangedCallback((evt) => {
        //    ((VectorValueNode)UnderlyingRuntimeNode).Value.z = evt.newValue;
        //});
        //floatField.style.height = new Length(EditorGUIUtility.singleLineHeight, LengthUnit.Pixel);

        //input = floatField.Q("unity-text-input");
        //input.style.position = Position.Absolute;
        //input.style.left = new Length(50, LengthUnit.Percent);
        //input.style.right = new Length(0, LengthUnit.Percent);

        //contents.Add(floatField);

        //floatField = new FloatField("W");
        //floatField.name = "W Field";
        //floatField.SetValueWithoutNotify(((VectorValueNode)UnderlyingRuntimeNode).Value.w);

        //floatField.RegisterValueChangedCallback((evt) => {
        //    ((VectorValueNode)UnderlyingRuntimeNode).Value.w = evt.newValue;
        //});
        //floatField.style.height = new Length(EditorGUIUtility.singleLineHeight, LengthUnit.Pixel);

        //input = floatField.Q("unity-text-input");
        //input.style.position = Position.Absolute;
        //input.style.left = new Length(50, LengthUnit.Percent);
        //input.style.right = new Length(0, LengthUnit.Percent);

        //contents.Add(floatField);
    }
}

[Flags]
public enum VectorAxis {
    XY = 0,
	XYZ = 1,
	XYZW = 2,
}