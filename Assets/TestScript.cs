using UnityEngine;
using System.Reflection;
using System;
using System.Collections;

public class TestScript : MonoBehaviour
{
    [SerializeField] ManipulationContainer container;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.C) && container != null) {
            StartCoroutine(container.StartEvaluate());

        }

        if (Input.GetKeyDown(KeyCode.V)) {
            var method = typeof(Transform).GetMethod(nameof(Transform.Translate), BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, new Type[1] { typeof(Vector3) }, null);

            method.Invoke(transform, new object[] { Vector3.left });
        }
    }
}
