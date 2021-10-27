using UnityEngine;
using System.Linq;
using System.Reflection;
using System;
using System.Collections.Generic;
using Stopwatch = System.Diagnostics.Stopwatch;

public class TestScript : MonoBehaviour
{
    [SerializeField] ManipulationInitializer initializer;

    public MVector floatVector;
    public MVectorInt intVector;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.C) && initializer != null) {
            initializer.StartEvaluate();
        }

        if (Input.GetKeyDown(KeyCode.V)) {
            var method = typeof(Transform).GetMethod(nameof(Transform.Translate), BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, new Type[1] { typeof(Vector3) }, null);

            method.Invoke(transform, new object[] { Vector3.left });
        }

        if (Input.GetKeyDown(KeyCode.B)) {
            Stopwatch sw = new Stopwatch();

            sw.Start();
            var types = UnityEditor.TypeCache.GetTypesDerivedFrom<UnityEngine.Object>();
            var assembly = AppDomain.CurrentDomain.GetAssemblies().Where(x => x.GetName().Name == "UnityEditor").FirstOrDefault();

            var filter = types.Where(x => ValidateType(x) && ValidateTypeAssembly(x));
            sw.Stop();

            Debug.Log(string.Join("\n", filter.Select(x => x.FullName)));
            Debug.Log("Stopwatch tick: " + sw.ElapsedTicks);
        }

        if (Input.GetKeyDown(KeyCode.N)) {
            Stopwatch sw = new Stopwatch();

            sw.Start();

            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies()) {
                foreach (var type in asm.GetTypes()) {

                }
            }

            sw.Stop();
            Debug.Log(sw.ElapsedTicks);
        }
    }

    private static bool ValidateType(Type t) {
        return !t.IsAbstract && !t.FullName.StartsWith("UnityEngine.TestTools") && !t.FullName.StartsWith("UnityEngine.TestRunner") && t.Assembly.GetName().Name != "UnityEditor" && t.GetCustomAttribute<ObsoleteAttribute>() == null;
    }

    private static bool ValidateTypeAssembly(Type t) {
        //return t.Assembly.GetName().Name != "UnityEditor" && !t.Assembly.GetReferencedAssemblies().Select(y => y.Name).Contains("UnityEditor");
        return t.Assembly.GetName().Name != "UnityEditor";
    }
}
