using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSequenceNode : BaseRuntimeNode {
    [SerializeField, ExcludeInput] private FlowInput _previous = FlowInput.Null, _next = FlowInput.Null;

    public FlowInput Previous {
        get => _previous;
        set {
            _previous = value;

#if UNITY_EDITOR
            if (_previous.OutputIndex > 0) {
                Debug.LogWarning("Flow Input of Sequence Node cannot have it's Output Index modified");

                _previous.OutputIndex = 0;
            }
#endif
        }
    }

    public FlowInput Next {
        get => _next;
        set {
            _next = value;

#if UNITY_EDITOR
            if (_next.OutputIndex > 0) {
                Debug.LogWarning("Flow Input of Sequence Node cannot have it's Output Index modified");

                _next.OutputIndex = 0;
            }
#endif
        }
    }

    private void PreviousCallbackTest() {
        Debug.Log("Previous Callback");
    }

    private void NextCallbackTest() {
        Debug.Log("Next Callback");
    }

    public bool IsFinal => string.IsNullOrEmpty(Next.GUID) || Next.GUID == "00000000-0000-0000-0000-000000000000";
    public bool IsRoot => string.IsNullOrEmpty(Previous.GUID) || Previous.GUID == "00000000-0000-0000-0000-000000000000";
}