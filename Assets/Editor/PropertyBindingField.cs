using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;

public class PropertyBindingField
{
    public static readonly string GenericName = "ObjectBindingDrop";

    public VisualElement Container { get; private set; }
    public BlackboardRow Row { get; private set; }
    public BlackboardField Field { get; private set; }
    public ObjectBindableProperty Property { get; private set; }

    public enum DraggingState {
        Rest, Ready, Dragging
    }

    public PropertyBindingField(ObjectBindableProperty property, Blackboard blackboard) {
        Property = property;

        Container = new VisualElement();

        Field = new BlackboardField {
            text = property.Name,
            typeText = property.Value == null ? "Null" : property.Value.GetType().FullName,
        };
        Field.RegisterCallback<DetachFromPanelEvent>(DestroyCallback);

        RegisterCallbacks();

        Container.Add(Field);

        var dataContainer = new VisualElement();
        dataContainer.name = "property-container";

        var objectField = new ObjectField("Object");
        objectField.objectType = typeof(Object);
        objectField.allowSceneObjects = false;
        objectField.SetValueWithoutNotify(property.Value);

        objectField.RegisterValueChangedCallback((obj) => {
            Property.Value = obj.newValue;
            Field.typeText = obj.newValue == null ? "Null" : obj.newValue.GetType().FullName;
        });

        var label = objectField.Q<Label>(className: "unity-label").style;
        label.width = new Length(20, LengthUnit.Percent);
        label.minWidth = StyleKeyword.Auto;

        objectField.Q<Label>(className: "unity-object-field-display__label").style.width = 0;

        dataContainer.Add(objectField);

        var row = new BlackboardRow(Field, dataContainer);

        Field.Add(row);
        Container.Add(row);

        blackboard.Add(Container);
    }

    void RegisterCallbacks() {
    }

    void DestroyCallback(DetachFromPanelEvent evt) {
        Container.RemoveFromHierarchy();
    }
}