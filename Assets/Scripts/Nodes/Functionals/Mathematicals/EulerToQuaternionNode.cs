using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomizeSearchPath("Functionals/Mathematicals/Euler To Quaternion Node")]
public class EulerToQuaternionNode : BaseRuntimeNode
{
    [field: ExpectedInputType(typeof(MVector), typeof(MVectorInt)), SerializeField] public FlowInput Euler { get; set; } = FlowInput.Null;

    private void Evaluate(NodeEvaluationInfo info, [OutputLayoutIndex(0)] out MVector rotation) {
        if (Euler.IsNull()) {
            rotation = MVector.QuaternionIdentity;
            return;
        }

        MVector euler = info.Container.EvaluateInput<ITractatoriConvertible>(Euler).ToMVector();
        var q = Quaternion.Euler(euler.Vector);

        rotation = new MVector(q.x, q.y, q.z, q.w);
    }
}
