using UnityEngine;

namespace ScriptableEvents
{
    [CreateAssetMenu(fileName = "BoolEvent", menuName = "Global/Events/Bool Event")]
    public sealed class BoolGameEvent : DynamicGameEvent<bool> { }
}

