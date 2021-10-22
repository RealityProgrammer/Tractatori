using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class BindingPropertyNode : BaseRuntimeNode, IConstantValueNode
{
    [field: SerializeField]
    public string Name { get; set; }

    private void Evaluate(NodeEvaluationInfo info, [OutputLayoutIndex(0)] out object output) {
        var p = info.Container.FindBindingProperty(Name);

        if (p == null) {
            output = null;
        } else {
            output = p.GetValue();
        }
    }
}
