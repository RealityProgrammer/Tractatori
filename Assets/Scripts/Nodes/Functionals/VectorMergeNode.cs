using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomizeSearchPath("Functionals/Miscs/Vector Merge Node")]
public class VectorMergeNode : BaseRuntimeNode
{
    [field: SerializeField, ExpectedInputType(typeof(MVector), typeof(MVectorInt))]
    public FlowInput X { get; private set; } = FlowInput.Null;

    [field: SerializeField, ExpectedInputType(typeof(MVector), typeof(MVectorInt))]
    public FlowInput Y { get; private set; } = FlowInput.Null;

    [field: SerializeField, ExpectedInputType(typeof(MVector), typeof(MVectorInt))]
    public FlowInput Z { get; private set; } = FlowInput.Null;

    [field: SerializeField, ExpectedInputType(typeof(MVector), typeof(MVectorInt))]
    public FlowInput W { get; private set; } = FlowInput.Null;

    private void Evaluate(NodeEvaluationInfo info, [OutputLayoutIndex(0)] out MVector output) {
        // For now, this will return 4 axes vector
        output = new MVector(X.IsNull() ? 0 : info.Container.EvaluateInput<ITractatoriConvertible>(X).ToMVector().X,
                             Y.IsNull() ? 0 : info.Container.EvaluateInput<ITractatoriConvertible>(Y).ToMVector().Y,
                             Z.IsNull() ? 0 : info.Container.EvaluateInput<ITractatoriConvertible>(Z).ToMVector().Z,
                             W.IsNull() ? 0 : info.Container.EvaluateInput<ITractatoriConvertible>(W).ToMVector().W);
    }
}
