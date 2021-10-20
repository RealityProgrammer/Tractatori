using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct FlowInput
{
    public static FlowInput Null = new FlowInput("00000000-0000-0000-0000-000000000000", 0);

    [field: SerializeField] public string GUID { get; set; }
    [field: SerializeField] public int OutputIndex { get; set; }

    public FlowInput(string guid) {
        GUID = guid;
        OutputIndex = 0;
    }

    public FlowInput(string guid, int outputIndex) {
        GUID = guid;
        OutputIndex = outputIndex;
    }

    public override string ToString() {
        return "FlowInput(GUID: " + GUID + ", Output Index: " + OutputIndex + ")";
    }
}
