using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class BindablePropertyDrawerCollector
{
    private static Dictionary<Type, Type> _drawerDictionary;
    private static readonly Action<BaseBindablePropertyDrawer, BaseBindableProperty> PropertyAssign;

    static BindablePropertyDrawerCollector() {
        PropertyAssign = (Action<BaseBindablePropertyDrawer, BaseBindableProperty>)typeof(BaseBindablePropertyDrawer).GetProperty("Property", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetSetMethod(true).CreateDelegate(typeof(Action<BaseBindablePropertyDrawer, BaseBindableProperty>));

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

    public static BaseBindablePropertyDrawer CreateDrawer<T>(T property) where T : BaseBindableProperty {
        var type = typeof(T);

        if (_drawerDictionary.TryGetValue(type, out var drawer)) {
            BaseBindablePropertyDrawer drawerInstance = Activator.CreateInstance(drawer) as BaseBindablePropertyDrawer;

            PropertyAssign(drawerInstance, property);

            return drawerInstance;
        }

        return null;
    }
}
