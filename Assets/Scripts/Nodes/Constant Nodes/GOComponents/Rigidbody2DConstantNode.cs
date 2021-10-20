using UnityEngine;

public class Rigidbody2DConstantNode : BaseRuntimeNode, IConstantValueNode
{
    public Rigidbody2D Value;

    private void Evaluate([OutputLayoutIndex(0)] out Rigidbody2D value) {
        value = Value;
    }
}
