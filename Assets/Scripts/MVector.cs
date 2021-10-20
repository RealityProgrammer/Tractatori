﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct MVector {
    [field: SerializeField] public Vector4 Vector { get; set; }
    public float X {
        get => Vector.x;
        set => Vector = new Vector4(value, Vector.y, Vector.z, Vector.w);
    }
    public float Y {
        get => Vector.y;
        set => Vector = new Vector4(Vector.x, value, Vector.z, Vector.w);
    }
    public float Z {
        get => Vector.z;
        set => Vector = new Vector4(Vector.x, Vector.y, value, Vector.w);
    }
    public float W {
        get => Vector.w;
        set => Vector = new Vector4(Vector.x, Vector.y, Vector.z, value);
    }

    [field: SerializeField] public int Axis { get; private set; }

    public MVector(float x) {
        Vector = new Vector4(x, 0);
        Axis = 1;
    }

    public MVector(Vector2 v) {
        Vector = new Vector4(v.x, v.y);
        Axis = 2;
    }

    public MVector(float x, float y) {
        Vector = new Vector4(x, y);
        Axis = 2;
    }

    public MVector(Vector3 v) {
        Vector = new Vector4(v.x, v.y, v.z);
        Axis = 3;
    }

    public MVector(float x, float y, float z) {
        Vector = new Vector4(x, y, z);
        Axis = 3;
    }

    public MVector(Vector4 v) {
        Vector = new Vector4(v.x, v.y, v.z, v.w);
        Axis = 4;
    }

    public MVector(float x, float y, float z, float w) {
        Vector = new Vector4(x, y, z, w);
        Axis = 4;
    }

    public static MVector operator +(MVector v) => v;
    public static MVector operator -(MVector v) => new MVector(-v.Vector);
    public static MVector operator +(MVector lhs, MVector rhs) {
        int axis = Math.Max(lhs.Axis, lhs.Axis);

        switch (axis) {
            case 1:
            default:
                return new MVector(lhs.Vector.x + rhs.Vector.x);

            case 2: return new MVector(lhs.Vector.x + rhs.Vector.x, lhs.Vector.y + rhs.Vector.y);
            case 3: return new MVector(lhs.Vector.x + rhs.Vector.x, lhs.Vector.y + rhs.Vector.y, lhs.Vector.z + rhs.Vector.z);
            case 4: return new MVector(lhs.Vector + rhs.Vector);
        }
    }

    public static MVector operator -(MVector lhs, MVector rhs) {
        int axis = Math.Max(lhs.Axis, lhs.Axis);

        switch (axis) {
            case 1:
            default:
                return new MVector(lhs.Vector.x + rhs.Vector.x);

            case 2: return new MVector(lhs.Vector.x + rhs.Vector.x, lhs.Vector.y + rhs.Vector.y);
            case 3: return new MVector(lhs.Vector.x + rhs.Vector.x, lhs.Vector.y + rhs.Vector.y, lhs.Vector.z + rhs.Vector.z);
            case 4: return new MVector(lhs.Vector + rhs.Vector);
        }
    }

    public static MVector operator*(MVector lhs, float scale) {
        return new MVector(lhs.Vector * scale);
    }

    public static MVector operator /(MVector lhs, float scale) {
        return new MVector(lhs.Vector / scale);
    }

    public static implicit operator Vector4(MVector v) {
        return v.Vector;
    }

    public static MVector ConvertFrom(Vector4 v, int axisCount) {
        switch (axisCount) {
            case 1:
            default:
                return new MVector(v.x);

            case 2: return new MVector(v.x, v.y);
            case 3: return new MVector(v.x, v.y, v.z);
            case 4: return new MVector(v.x, v.y, v.z, v.w);
        }
    }

    public override string ToString() {
        switch (Axis) {
            case 1:
            default:
                return "MVector(" + X + ")";

            case 2: return "MVector(" + X + ", " + Y + ")";
            case 3: return "MVector(" + X + ", " + Y + ", " + Z + ")";
            case 4: return "MVector(" + X + ", " + Y + ", " + Z + ", " + W + ")";
        }
    }
}