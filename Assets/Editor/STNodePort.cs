using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;

public class STNodePort : Port
{
    public Action<Edge> OnPortConnect;
    public NodeConnectionCallback ConnectionCallback { get; protected set; }

    public int OutputIndex { get; set; } = 0;

    public STNodePort(Orientation portOrientation, Direction portDirection, Capacity portCapacity, Type type) : base(portOrientation, portDirection, portCapacity, type) {
        ConnectionCallback = new NodeConnectionCallback();
        m_EdgeConnector = new EdgeConnector<Edge>(ConnectionCallback);

        this.AddManipulator(m_EdgeConnector);

        this.RegisterCallback<DetachFromPanelEvent>((evt) => {
        });
    }

    public override void Connect(Edge edge) {
        base.Connect(edge);

        OnPortConnect?.Invoke(edge);
    }
}