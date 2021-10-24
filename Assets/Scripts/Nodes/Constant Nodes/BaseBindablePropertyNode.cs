using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBindablePropertyNode : BaseRuntimeNode, IConstantValueNode
{
    [field: SerializeField]
    public string Name { get; set; }
}
