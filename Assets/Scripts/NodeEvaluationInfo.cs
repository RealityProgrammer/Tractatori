using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeEvaluationInfo
{
    public ManipulationContainer Container { get; private set; }

    public NodeEvaluationInfo(ManipulationContainer container) {
        Container = container;
    }
}
