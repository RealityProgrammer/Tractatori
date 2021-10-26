using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOrPropertyInfo {
    public enum InfoType {
        Null, Field, Property
    }

    public FieldInfo Field { get; private set; }
    public PropertyInfo Property { get; private set; }

    public bool IsUsingBackingField { get; private set; }

    public InfoType UsingType { get; private set; } = InfoType.Null;

    public FieldOrPropertyInfo(FieldInfo field) {
        if (field == null) {
            UsingType = InfoType.Null;
            return;
        }

        Field = field;
        UsingType = InfoType.Field;
    }

    public FieldOrPropertyInfo(PropertyInfo property) {
        if (property == null) {
            UsingType = InfoType.Null;
            return;
        }

        Property = property;
        UsingType = InfoType.Property;
    }

    public FieldOrPropertyInfo(FieldInfo field, PropertyInfo property) {
        if (property != null) {
            Property = property;
            UsingType = InfoType.Property;

            if (field != null) {
                if (field.Name == "<" + property.Name + ">k__BackingField") {
                    IsUsingBackingField = true;
                    Field = field;
                }
            }

            return;
        }

        if (field != null) {
            Field = field;
            UsingType = InfoType.Field;
            return;
        }

        UsingType = InfoType.Null;
    }

    public bool TryAssignField(FieldInfo field) {
        if (UsingType == InfoType.Null && field != null) {
            Field = field;
            UsingType = InfoType.Field;
            return true;
        }

        return false;
    }

    public bool TryAssignProperty(PropertyInfo property) {
        if (UsingType == InfoType.Null && property != null) {
            Property = property;
            UsingType = InfoType.Property;
            return true;
        }

        return false;
    }

    public object GetValue(object obj) {
        switch (UsingType) {
            case InfoType.Field: return Field.GetValue(obj);
            case InfoType.Property: return Property.GetValue(obj);
            case InfoType.Null:
            default:
                throw new NullReferenceException("Cannot get Field or Property Info value because both FieldInfo and PropertyInfo is null");
        }
    }

    public void SetValue(object obj, object value) {
        switch (UsingType) {
            case InfoType.Field: Field.SetValue(obj, value); break;
            case InfoType.Property: Property.SetValue(obj, value); break;
            case InfoType.Null:
            default:
                throw new NullReferenceException("Cannot set Field or Property Info value because both FieldInfo and PropertyInfo is null");
        }
    }

    public Type Type {
        get {
            switch (UsingType) {
                case InfoType.Field: return Field.FieldType;
                case InfoType.Property: return Property.PropertyType;
                case InfoType.Null:
                default:
                    throw new NullReferenceException("Cannot get type of Field or Property Info because both FieldInfo and PropertyInfo is null");
            }
        }
    }

    public string Name {
        get {
            switch (UsingType) {
                case InfoType.Field: return Field.Name;
                case InfoType.Property: return Property.Name;
                case InfoType.Null:
                default:
                    throw new NullReferenceException("Cannot get name of Field or Property Info because both FieldInfo and PropertyInfo is null");
            }
        }
    }

    public T GetAttribute<T>() where T : Attribute {
        switch (UsingType) {
            case InfoType.Field: return Field.GetCustomAttribute<T>();
            case InfoType.Property:
                var attr = Property.GetCustomAttribute<T>();

                return attr ?? Field.GetCustomAttribute<T>();

            case InfoType.Null:
            default:
                throw new NullReferenceException("Cannot get attribute of Field or Property Info because both FieldInfo and PropertyInfo is null");
        }
    }
}
