using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameDebugNode : BaseSequenceNode
{
    private void Evaluate(NodeEvaluationInfo info) {
        Debug.Log("Frame: " + Time.frameCount);
    }
}
