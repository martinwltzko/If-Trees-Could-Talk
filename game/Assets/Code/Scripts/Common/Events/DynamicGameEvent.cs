using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "DynamicGameEvent", menuName = "Global/Events/Dynamic Game Event")]
public class DynamicGameEvent : GameEvent
{
    public enum EventType
    {
        TOGGLE,
        INT,
        FLOAT,
        OBJECT
    }

    public EventType Type;
    public object Meta;

    [ReadOnly, ShowIf("Type", EventType.TOGGLE)] public bool boolValue;
    [ReadOnly, ShowIf("Type", EventType.INT)] public int intValue;
    [ReadOnly, ShowIf("Type", EventType.FLOAT)] public float floatValue;


    public void Raise(object sender)
    {
        Meta = sender;
        base.Raise(sender);
    }
    
    public void Raise(bool value)
    {
        base.Raise(value);
    }
    
    public override void Raise()
    {
        switch (Type)
        {
            case EventType.INT:
                base.Raise<int>(intValue);
                break;
            case EventType.FLOAT:
                base.Raise<float>(floatValue);
                break;
        }
    }
}
