using UnityEngine;

namespace ScriptableEvents
{
    [CreateAssetMenu(fileName = "FloatEvent", menuName = "Global/Events/Float Event")]
    public sealed class FloatGameEvent : DynamicGameEvent<float> { }
}