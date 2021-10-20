using UnityEngine;

public class GameObjectConstantNode : BaseRuntimeNode, IConstantValueNode
{
    public GameObject Value;

    private void Evaluate([OutputLayoutIndex(0)] out GameObject value) {
        value = Value;
    }
}
