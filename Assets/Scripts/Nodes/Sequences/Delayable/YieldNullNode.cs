using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomizeSearchPath("Delayable/Yield Null Node")]
public class YieldNullNode : BaseSequenceNode
{
    private object Evaluate(NodeEvaluationInfo info) {
        return null;
    }
}
