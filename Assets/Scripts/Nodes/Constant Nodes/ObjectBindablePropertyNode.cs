using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class ObjectBindablePropertyNode : BaseBindablePropertyNode
{
    private void Evaluate(NodeEvaluationInfo info, [OutputLayoutIndex(0)] out Object output) {
        var p = info.Container.FindObjectBindableProperty(Name);

        if (p == null) {
            output = null;
        } else {
            output = p.Value;
        }
    }
}
