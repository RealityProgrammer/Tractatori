using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValidateObjectTypeNode : MonoBehaviour
{
    [field: SerializeField, ExpectedInputType(typeof(object))]
    public FlowInput Input { get; set; }

    // Wait... WE GOT NO CONDITIONAL NODE
}
