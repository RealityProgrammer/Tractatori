using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorBindablePropertyNode : BaseBindablePropertyNode {
    private void Evaluate(NodeEvaluationInfo info, [OutputLayoutIndex(0)] out MVector output) {
        var p = info.Container.FindVectorBindableProperty(Name);

        if (p == null) {
            output = MVector.Zero4;
        } else {
            output = p.Value;
        }
    }
}
