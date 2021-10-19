﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddForceNode : BaseSequenceNode
{
    [field: SerializeField, ExpectedInputType(typeof(Rigidbody))] public FlowInput TargetRigidbody { get; set; }
    [field: SerializeField, ExpectedInputType(typeof(MVector))] public FlowInput Direction { get; set; }

    protected override void Evaluate(NodeEvaluationInfo info) {
        info.Container.EvaluateInput<Rigidbody>(TargetRigidbody).AddForce(ToVector3(info.Container.EvaluateInput<MVector>(Direction)));
    }

    Vector3 ToVector3(MVector vector)
    {
        return new Vector3(vector.X, vector.Y, vector.Z);
    }
}
