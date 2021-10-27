using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Boolean4
{
    public static Boolean4 AllTrue = new Boolean4(true, true, true, true);
    public static Boolean4 AllFalse = new Boolean4(false, false, false, false);

    [field: SerializeField] public bool B1 { get; set; }
    [field: SerializeField] public bool B2 { get; set; }
    [field: SerializeField] public bool B3 { get; set; }
    [field: SerializeField] public bool B4 { get; set; }

    public Boolean4(bool a) {
        B1 = a;
        B2 = B3 = B4 = false;
    }

    public Boolean4(bool a, bool b) {
        B1 = a;
        B2 = b;
        B3 = B4 = false;
    }

    public Boolean4(bool a, bool b, bool c) {
        B1 = a;
        B2 = b;
        B3 = c;
        B4 = false;
    }

    public Boolean4(bool a, bool b, bool c, bool d) {
        B1 = a;
        B2 = b;
        B3 = c;
        B4 = d;
    }

    public override bool Equals(object obj) {
        if (obj is Boolean4 other) {
            return B1 == other.B1 &&
                   B2 == other.B2 &&
                   B3 == other.B3 &&
                   B4 == other.B4;
        }

        return false;
    }

    public override int GetHashCode() {
        int h = 0;

        h |= (B1 ? 1 : 0) << 3;
        h |= (B2 ? 1 : 0) << 2;
        h |= (B3 ? 1 : 0) << 1;
        h |= B4 ? 1 : 0;

        return h;
    }

    public static Boolean4 operator !(Boolean4 lhs) {
        return new Boolean4(!lhs.B1, !lhs.B2, !lhs.B3, !lhs.B4);
    }

    public static Boolean4 operator &(Boolean4 lhs, Boolean4 rhs) {
        return new Boolean4(lhs.B1 & rhs.B1, lhs.B2 & rhs.B2, lhs.B3 & rhs.B3, lhs.B4 & rhs.B4);
    }

    public static Boolean4 operator |(Boolean4 lhs, Boolean4 rhs) {
        return new Boolean4(lhs.B1 | rhs.B1, lhs.B2 | rhs.B2, lhs.B3 | rhs.B3, lhs.B4 | rhs.B4);
    }

    public static Boolean4 operator ^(Boolean4 lhs, Boolean4 rhs) {
        return new Boolean4(lhs.B1 ^ rhs.B1, lhs.B2 ^ rhs.B2, lhs.B3 ^ rhs.B3, lhs.B4 ^ rhs.B4);
    }

    public bool All() {
        return B1 && B2 && B3 && B4;
    }

    public bool Any() {
        return B1 || B2 || B3 || B4;
    }

    public bool this[int i] {
        get {
            switch (i) {
                case 0:
                default:
                    return B1;

                case 1: return B2;
                case 2: return B3;
                case 3: return B4;
            }
        }

        set {
            switch (i) {
                case 0:
                default:
                    B1 = value;
                    break;

                case 1: B2 = value; break;
                case 2: B3 = value; break;
                case 3: B4 = value; break;
            }
        }
    }
}
