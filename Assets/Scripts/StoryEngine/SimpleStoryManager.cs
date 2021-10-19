using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StablerCharacter {
    public class SimpleStoryManager : MonoBehaviour
    {
        [SerializeField] ManipulationContainer container;
        [SerializeField] bool startStoryOnAwake;
        [SerializeField] TMPro.TextMeshProUGUI storyText;

        // Start is called before the first frame update
        void Start()
        {
            if(startStoryOnAwake) InvokeStory();
        }

        public void InvokeStory()
        {
            // Dictionary<string, object> args = {
            //     {"StoryText", storyText}
            // };
            // container.Evaluate(args);
        }
    }
}
