using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManipulationInitializer : MonoBehaviour
{
    [SerializeField] private ManipulationContainer container;

    public ManipulationContainer Container {
        get => container;
        set => container = value;
    }

    [SerializeField] private List<ObjectBindableProperty> objectOverrideList;
    public List<ObjectBindableProperty> ObjectOverrideList => objectOverrideList;

    [SerializeField] private List<MVectorBindableProperty> vectorOverrideList;
    public List<MVectorBindableProperty> VectorOverrideList => vectorOverrideList;

    bool isEvaluating;
    public void StartEvaluate() {
        if (!isEvaluating && container != null) {
            isEvaluating = true;

            StartCoroutine(EvaluateCoroutine());
        }
    }

    private IEnumerator EvaluateCoroutine() {
        container.OverrideObjectBindableProperties = objectOverrideList;
        container.OverrideVectorBindableProperties = vectorOverrideList;

        yield return container.StartEvaluate();

        container.OverrideObjectBindableProperties = null;
        container.OverrideVectorBindableProperties = null;

        isEvaluating = false;
    }
}
