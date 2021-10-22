using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;

public class ObjectBindingDropManipulator : Manipulator {
    public static readonly string GenericName = "ObjectBindingDrop";
    private BaseBindableProperty _bindingProperty;
    
    public ObjectBindingDropManipulator(BaseBindableProperty bindingProperty) {
        _bindingProperty = bindingProperty;
    }

    public enum DraggingState {
        Rest, Ready, Dragging
    }

    #region Registrations
    protected override void RegisterCallbacksOnTarget() {
        target.RegisterCallback<MouseDownEvent>(HandleStartDrag);
        target.RegisterCallback<MouseMoveEvent>(HandleMoveEvent);
    }

    protected override void UnregisterCallbacksFromTarget() {
        target.UnregisterCallback<MouseDownEvent>(HandleStartDrag);
        target.UnregisterCallback<MouseMoveEvent>(HandleMoveEvent);
    }
    #endregion

    private DraggingState m_DragState = 0; // Rest, Ready, Dragging
    void HandleStartDrag(MouseDownEvent e) {
        if (e.currentTarget == target && e.button == 0) {
            m_DragState = DraggingState.Ready;
        }
    }

    void HandleMoveEvent(MouseMoveEvent e) {
        if (m_DragState == DraggingState.Ready) {
            Debug.Log("Start Drag");

            DragAndDrop.PrepareStartDrag();
            DragAndDrop.SetGenericData(GenericName, _bindingProperty);

            DragAndDrop.StartDrag("RealityProgrammer");

            m_DragState = DraggingState.Dragging;
        }
    }

    //void HandleDragUpdated(DragUpdatedEvent e) {
    //    if (m_DragState == DraggingState.Dragging) {
    //        DragAndDrop.visualMode = DragAndDropVisualMode.Move;
    //    }
    //}

    //void HandleDragPerform(DragPerformEvent e) {
    //    if ((m_DragState == DraggingState.Dragging || m_DragState == DraggingState.Ready) && e.button == 0) {
    //        DragAndDrop.visualMode = DragAndDropVisualMode.None;
    //        m_DragState = 0;
    //    }
    //}
}
