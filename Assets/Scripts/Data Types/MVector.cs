using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct MVector : ITractatoriConvertible {
    public static MVector Zero1 = new MVector(0);
    public static MVector Zero2 = new MVector(0, 0);
    public static MVector Zero3 = new MVector(0, 0, 0);
    public static MVector Zero4 = new MVector(0, 0, 0, 0);

    public static MVector QuaternionIdentity = new MVector(0, 0, 0, 1);

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
                return new MVector(lhs.X + rhs.X);

            case 2: return new MVector(lhs.X + rhs.X, lhs.Y + rhs.Y);
            case 3: return new MVector(lhs.X + rhs.X, lhs.Y + rhs.Y, lhs.Z + rhs.Z);
            case 4: return new MVector(lhs.Vector + rhs.Vector);
        }
    }

    public static MVector operator -(MVector lhs, MVector rhs) {
        int axis = Math.Max(lhs.Axis, lhs.Axis);

        switch (axis) {
            case 1:
            default:
                return new MVector(lhs.X - rhs.X);

            case 2: return new MVector(lhs.X - rhs.X, lhs.Y - rhs.Y);
            case 3: return new MVector(lhs.X - rhs.X, lhs.Y - rhs.Y, lhs.Z - rhs.Z);
            case 4: return new MVector(lhs.Vector - rhs.Vector);
        }
    }

    public static MVector operator*(MVector lhs, float scale) {
        switch (lhs.Axis) {
            case 1:
            default:
                return new MVector(lhs.X * scale);

            case 2: return new MVector(lhs.X * scale, lhs.Y * scale);
            case 3: return new MVector(lhs.X * scale, lhs.Y * scale, lhs.Z * scale);
            case 4: return new MVector(lhs.Vector * scale);
        }
    }

    public static MVector operator /(MVector lhs, float scale) {
        switch (lhs.Axis) {
            case 1:
            default:
                return new MVector(lhs.X / scale);

            case 2: return new MVector(lhs.X / scale, lhs.Y / scale);
            case 3: return new MVector(lhs.X / scale, lhs.Y / scale, lhs.Z / scale);
            case 4: return new MVector(lhs.Vector / scale);
        }
    }

    public static explicit operator MVector(MVectorInt v) {
        switch (v.Axis) {
            case 1:
            default:
                return new MVector(v.X);

            case 2: return new MVector(v.X, v.Y);
            case 3: return new MVector(v.X, v.Y, v.Z);
            case 4: return new MVector(v.X, v.Y, v.Z, v.W);
        }
    }

    public static implicit operator Vector4(MVector v) {
        return v.Vector;
    }

    public static explicit operator MVector(Vector4 v) {
        return new MVector(v);
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

    public bool Equals(MVector other) {
        return Vector == other.Vector;
    }

    MVector ITractatoriConvertible.ToMVector() {
        return this;
    }

    MVectorInt ITractatoriConvertible.ToMVectorInt() {
        return (MVectorInt)this;
    }

    public float this[int i] {
        get {
            switch (i) {
                default: case 0: return X;
                case 1: return Y;
                case 2: return Z;
                case 3: return W;
            }
        }

        set {
            switch (i) {
                default: case 0: X = value; break;
                case 1: Y = value; break;
                case 2: Z = value; break;
                case 3: W = value; break;
            }
        }
    }

    public static Boolean4 operator<(MVector lhs, MVector rhs) {
        return new Boolean4(
            lhs.Vector.x < rhs.Vector.x,
            lhs.Vector.y < rhs.Vector.y,
            lhs.Vector.z < rhs.Vector.z,
            lhs.Vector.w < rhs.Vector.w);
    }

    public static Boolean4 operator >(MVector lhs, MVector rhs) {
        return new Boolean4(
            lhs.Vector.x > rhs.Vector.x,
            lhs.Vector.y > rhs.Vector.y,
            lhs.Vector.z > rhs.Vector.z,
            lhs.Vector.w > rhs.Vector.w);
    }

    public static Boolean4 operator <=(MVector lhs, MVector rhs) {
        return new Boolean4(
            lhs.Vector.x <= rhs.Vector.x,
            lhs.Vector.y <= rhs.Vector.y,
            lhs.Vector.z <= rhs.Vector.z,
            lhs.Vector.w <= rhs.Vector.w);
    }

    public static Boolean4 operator >=(MVector lhs, MVector rhs) {
        return new Boolean4(
            lhs.Vector.x >= rhs.Vector.x,
            lhs.Vector.y >= rhs.Vector.y,
            lhs.Vector.z >= rhs.Vector.z,
            lhs.Vector.w >= rhs.Vector.w);
    }

    public static Boolean4 operator !=(MVector lhs, MVector rhs) {
        return new Boolean4(
            lhs.Vector.x != rhs.Vector.x,
            lhs.Vector.y != rhs.Vector.y,
            lhs.Vector.z != rhs.Vector.z,
            lhs.Vector.w != rhs.Vector.w);
    }

    public static Boolean4 operator ==(MVector lhs, MVector rhs) {
        return new Boolean4(
            lhs.Vector.x == rhs.Vector.x,
            lhs.Vector.y == rhs.Vector.y,
            lhs.Vector.z == rhs.Vector.z,
            lhs.Vector.w == rhs.Vector.w);
    }

    public override bool Equals(object obj) {
        if (obj is MVector v) {
            return (v == this).All();
        }

        return false;
    }

    public override int GetHashCode() {
        return Vector.GetHashCode() | (Axis << 5);
    }
}