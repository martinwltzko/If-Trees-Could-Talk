using UnityEngine;

namespace ScriptableEvents
{
    [CreateAssetMenu(fileName = "ComponentEvent", menuName = "Global/Events/Component Event")]
    public sealed class ComponentGameEvent : DynamicGameEvent<Component> { }
}