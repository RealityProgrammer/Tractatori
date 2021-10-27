using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphLoadInformation
{
    public ManipulationContainer Container { get; private set; }
    public TractatoriGraphView GraphView { get; private set; }

    public GraphLoadInformation(ManipulationContainer container, TractatoriGraphView gv) {
        Container = container;
        GraphView = gv;
    }
}
