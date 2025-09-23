using UnityEngine;

namespace ScriptableEvents
{
    [CreateAssetMenu(fileName = "TransformEvent", menuName = "Global/Events/Transform Event")]
    public sealed class TransformGameEvent : DynamicGameEvent<Transform> { }
}