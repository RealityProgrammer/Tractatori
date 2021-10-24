using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManipulationInitializer : MonoBehaviour
{
    [SerializeField] private ManipulationContainer container;

    [SerializeField]

    public ManipulationContainer Container {
        get => container;
        set => container = value;
    }
}
