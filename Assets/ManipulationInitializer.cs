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


}
