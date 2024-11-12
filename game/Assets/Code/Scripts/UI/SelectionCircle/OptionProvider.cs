using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Code.Scripts.UI
{
    public class OptionProvider : MonoBehaviour
    {
        public List<Option> Options = new List<Option>();

        [System.Serializable]
        public class Option
        {
            public string OptionName;
            public UnityEvent<MonoBehaviour> Interact;
        }
    }
}