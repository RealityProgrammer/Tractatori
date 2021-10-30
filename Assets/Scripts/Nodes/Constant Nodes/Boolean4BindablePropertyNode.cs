using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boolean4BindablePropertyNode : BaseBindablePropertyNode
{
    private void Evaluate(NodeEvaluationInfo info, [OutputLayoutIndex(0)] out Boolean4 output) {
        var p = info.Container.FindBooleanBindableProperty(Name);

        if (p == null) {
            output = Boolean4.AllFalse;
        } else {
            output = p.Value;
        }
    }
}
