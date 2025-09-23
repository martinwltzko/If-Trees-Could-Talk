using UnityEngine;

namespace ScriptableEvents
{
    [CreateAssetMenu(fileName = "IntEvent", menuName = "Global/Events/Int Event")]
    public sealed class IntGameEvent : DynamicGameEvent<int> { }
}