using UnityEngine;
using System.Collections;

public class TestScript : MonoBehaviour
{
    [SerializeField] ManipulationContainer container;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.C) && container != null) {
            StartCoroutine(container.StartEvaluate());
        }
    }
}
