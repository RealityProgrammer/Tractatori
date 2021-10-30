using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Validate input object, and generate a port based on whether object type is correct or not. In other words, it doesn't matter in runtime
/// </summary>
[CustomizeSearchPath("Functionals/Advanced/Validate Object Type Node")]
public class ValidateObjectTypeNode : BaseRuntimeNode
{
    [field: SerializeField, ExpectedInputType(typeof(object))]
    public FlowInput InputObject { get; set; } = FlowInput.Null;

    [field: SerializeField]
    public string TypeString { get; set; }

    private void Evaluate(NodeEvaluationInfo info, [OutputLayoutIndex(0)] out Object output) {
        if (InputObject.IsNull()) {
            output = null;
            return;
        }

        output = info.Container.EvaluateInput<Object>(InputObject);
    }
}
