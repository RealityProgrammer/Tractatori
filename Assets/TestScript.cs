using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    [SerializeField] ManipulationContainer container;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.C) && container != null) {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            container.StartEvaluate();

            UnityEngine.Debug.Log("Ticks: " + sw.ElapsedTicks);
        }
    }
}
