using System.Linq;
using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;

public class PropertyBindingField
{
    private static Dictionary<Type, Type> _drawerDictionary;
    private static readonly Action<BaseBindablePropertyDrawer, PropertyBindingField> ParentAssign;

    static PropertyBindingField() {
        ParentAssign = (Action<BaseBindablePropertyDrawer, PropertyBindingField>)typeof(BaseBindablePropertyDrawer).GetProperty("Parent", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetSetMethod(true).CreateDelegate(typeof(Action<BaseBindablePropertyDrawer, PropertyBindingField>));

        _drawerDictionary = new Dictionary<Type, Type>();

        var drawers = TypeCache.GetTypesDerivedFrom<BaseBindablePropertyDrawer>();
        foreach (var drawer in drawers) {
            var attribute = drawer.GetCustomAttribute<BindablePropertyDrawerOfAttribute>();

            if (attribute == null) {
                Debug.LogWarning("A drawer of Bindable Property without [BindablePropertyDrawerOf] attribute discovered: " + drawer.AssemblyQualifiedName);
            } else {
                if (_drawerDictionary.TryGetValue(attribute.Target, out var t)) {
                    Debug.LogWarning("A drawer for Bindable Property type \"" + attribute.Target.AssemblyQualifiedName + "\" is already exists.");
                } else {
                    _drawerDictionary.Add(attribute.Target, drawer);
                }
            }
        }
    }

    public static readonly string GenericName = "ObjectBindingDrop";

    public VisualElement Container { get; private set; }
    public BlackboardRow Row { get; private set; }
    public BlackboardField Field { get; private set; }
    public BaseBindableProperty Property { get; private set; }

    public PropertyBindingField(BaseBindableProperty property, Blackboard blackboard) {
        Property = property;

        Container = new VisualElement();

        Field = new BlackboardField {
            text = property.Name,
            // typeText = property.Value == null ? "Null" : property.Value.GetType().FullName,
        };
        Field.RegisterCallback<DetachFromPanelEvent>(DestroyCallback);

        RegisterCallbacks();

        Container.Add(Field);

        var dataContainer = new VisualElement();
        dataContainer.name = "property-container";

        var propertyType = Property.GetType();
        if (_drawerDictionary.TryGetValue(propertyType, out Type drawer)) {
            BaseBindablePropertyDrawer drawerInstance = Activator.CreateInstance(drawer) as BaseBindablePropertyDrawer;

            ParentAssign(drawerInstance, this);
            drawerInstance.Initialize(dataContainer);
        }

        var row = new BlackboardRow(Field, dataContainer);

        Field.Add(row);
        Container.Add(row);

        blackboard.Add(Container);

        Field.AddManipulator(new ContextualMenuManipulator(ContexualMenu));
    }

    void ContexualMenu(ContextualMenuPopulateEvent evt) {
        evt.menu.AppendAction("Delete", (action) => {
            switch (STGraphEditorWindow.WindowInstance.BlackboardWrapper.Displaying) {
                case TBlackboardWrapper.DisplayingSection.Object:
                    STGraphEditorWindow.CurrentEditingAsset.ObjectBindableProperties.Remove(Property as ObjectBindableProperty);
                    break;

                case TBlackboardWrapper.DisplayingSection.Vector:
                    STGraphEditorWindow.CurrentEditingAsset.VectorBindableProperties.Remove(Property as MVectorBindableProperty);
                    break;
            }

            Field.RemoveFromHierarchy();
        });
    }

    void RegisterCallbacks() {
        Field.RegisterCallback<KeyDownEvent>((evt) => {
            if (evt.keyCode == KeyCode.Delete) {
                Debug.Log("Aeugh");
            }
        });
    }

    void DestroyCallback(DetachFromPanelEvent evt) {
        Container.RemoveFromHierarchy();
    }
}