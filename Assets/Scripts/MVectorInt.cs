using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct MVectorInt : IEquatable<MVectorInt>, ITractatoriConvertible {
    public static MVectorInt Zero1 = new MVectorInt(0);
    public static MVectorInt Zero2 = new MVectorInt(0, 0);
    public static MVectorInt Zero3 = new MVectorInt(0, 0, 0);
    public static MVectorInt Zero4 = new MVectorInt(0, 0, 0, 0);

    public static MVectorInt QuaternionIdentity = new MVectorInt(0, 0, 0, 1);

    [field: SerializeField] public int X { get; set; }
    [field: SerializeField] public int Y { get; set; }
    [field: SerializeField] public int Z { get; set; }
    [field: SerializeField] public int W { get; set; }

    [field: SerializeField] public int Axis { get; private set; }

    public MVectorInt(int x) {
        X = x;
        Y = Z = W = 0;

        Axis = 1;
    }

    public MVectorInt(Vector2Int v) {
        X = v.x;
        Y = v.y;
        Z = W = 0;

        Axis = 2;
    }

    public MVectorInt(int x, int y) {
        X = x;
        Y = y;
        Z = W = 0;

        Axis = 2;
    }

    public MVectorInt(Vector3Int v) {
        X = v.x;
        Y = v.y;
        Z = v.z;
        W = 0;

        Axis = 3;
    }

    public MVectorInt(int x, int y, int z) {
        X = x;
        Y = y;
        Z = z;
        W = 0;
        Axis = 3;
    }

    public MVectorInt(int x, int y, int z, int w) {
        X = x;
        Y = y;
        Z = z;
        W = w;

        Axis = 4;
    }

    public static MVectorInt operator +(MVectorInt v) => v;
    public static MVectorInt operator -(MVectorInt v) => new MVectorInt(-v.X, -v.Y, -v.Z, -v.W);
    public static MVectorInt operator +(MVectorInt lhs, MVectorInt rhs) {
        int axis = Math.Max(lhs.Axis, lhs.Axis);

        switch (axis) {
            case 1:
            default:
                return new MVectorInt(lhs.X + rhs.X);

            case 2: return new MVectorInt(lhs.X + rhs.X, lhs.Y + rhs.Y);
            case 3: return new MVectorInt(lhs.X + rhs.X, lhs.Y + rhs.Y, lhs.Z + rhs.Z);
            case 4: return new MVectorInt(lhs.X + rhs.X, lhs.Y + rhs.Y, lhs.Z + rhs.Z, lhs.W + rhs.W);
        }
    }

    public static MVectorInt operator -(MVectorInt lhs, MVectorInt rhs) {
        int axis = Math.Max(lhs.Axis, lhs.Axis);

        switch (axis) {
            case 1:
            default:
                return new MVectorInt(lhs.X - rhs.X);

            case 2: return new MVectorInt(lhs.X - rhs.X, lhs.Y - rhs.Y);
            case 3: return new MVectorInt(lhs.X - rhs.X, lhs.Y - rhs.Y, lhs.Z - rhs.Z);
            case 4: return new MVectorInt(lhs.X - rhs.X, lhs.Y - rhs.Y, lhs.Z - rhs.Z, lhs.W - rhs.W);
        }
    }

    public static MVectorInt operator *(MVectorInt lhs, int scale) {
        switch (lhs.Axis) {
            case 1:
            default:
                return new MVectorInt(lhs.X * scale);

            case 2: return new MVectorInt(lhs.X * scale, lhs.Y * scale);
            case 3: return new MVectorInt(lhs.X * scale, lhs.Y * scale, lhs.Z * scale);
            case 4: return new MVectorInt(lhs.X * scale, lhs.Y * scale, lhs.Z * scale, lhs.W * scale);
        }
    }

    public static MVectorInt operator /(MVectorInt lhs, int scale) {
        switch (lhs.Axis) {
            case 1:
            default:
                return new MVectorInt(lhs.X / scale);

            case 2: return new MVectorInt(lhs.X / scale, lhs.Y / scale);
            case 3: return new MVectorInt(lhs.X / scale, lhs.Y / scale, lhs.Z / scale);
            case 4: return new MVectorInt(lhs.X / scale, lhs.Y / scale, lhs.Z / scale, lhs.W / scale);
        }
    }

    public static explicit operator MVectorInt(MVector v) {
        return new MVectorInt((int)v.X, (int)v.Y, (int)v.Z, (int)v.W);
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

    public bool Equals(MVectorInt other) {
        return X == other.X && Y == other.Y && Z == other.Z && W == other.W;
    }

    MVector ITractatoriConvertible.ToMVector() {
        return (MVector)this;
    }

    MVectorInt ITractatoriConvertible.ToMVectorInt() {
        return this;
    }

    public int this[int i] {
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
}
