using UnityEngine;

public class RigidbodyConstantNode : BaseRuntimeNode, IConstantValueNode
{
    public Rigidbody Value;

    private void Evaluate([OutputLayoutIndex(0)] out Rigidbody value) {
        value = Value;
    }
}
