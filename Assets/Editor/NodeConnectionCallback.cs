using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

public class NodeConnectionCallback : IEdgeConnectorListener {
    public Action<GraphView, Edge> OnDropCallback;
    public Action<Edge, Vector2> OnDropOutsidePortCallback;

    private GraphViewChange m_GraphViewChange;
    private List<Edge> m_EdgesToCreate;
    private List<GraphElement> m_EdgesToDelete;

    public NodeConnectionCallback() {
        m_EdgesToCreate = new List<Edge>();
        m_EdgesToDelete = new List<GraphElement>();

        m_GraphViewChange.edgesToCreate = m_EdgesToCreate;
    }

    public void OnDrop(GraphView graphView, Edge edge) {
        m_EdgesToCreate.Clear();
        m_EdgesToCreate.Add(edge);
        m_EdgesToDelete.Clear();

        if (edge.input.capacity == Port.Capacity.Single)
            foreach (Edge edgeToDelete in edge.input.connections)
                if (edgeToDelete != edge)
                    m_EdgesToDelete.Add(edgeToDelete);

        if (edge.output.capacity == Port.Capacity.Single)
            foreach (Edge edgeToDelete in edge.output.connections)
                if (edgeToDelete != edge)
                    m_EdgesToDelete.Add(edgeToDelete);

        if (m_EdgesToDelete.Count > 0)
            graphView.DeleteElements(m_EdgesToDelete);

        var edgesToCreate = m_EdgesToCreate;
        if (graphView.graphViewChanged != null) {
            edgesToCreate = graphView.graphViewChanged(m_GraphViewChange).edgesToCreate;
        }

        foreach (Edge e in edgesToCreate) {
            graphView.AddElement(e);
            edge.input.Connect(e);
            edge.output.Connect(e);
        }

        OnDropCallback?.Invoke(graphView, edge);
    }

    public void OnDropOutsidePort(Edge edge, Vector2 position) {
        OnDropOutsidePortCallback?.Invoke(edge, position);
    }
}
