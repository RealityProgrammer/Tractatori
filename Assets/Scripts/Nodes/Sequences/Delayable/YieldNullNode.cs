using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YieldNullNode : BaseSequenceNode
{
    private object Evaluate(NodeEvaluationInfo info) {
        return null;
    }
}
