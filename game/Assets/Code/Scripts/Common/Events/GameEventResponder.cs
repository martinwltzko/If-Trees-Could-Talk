using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventResponder : MonoBehaviour
{

    [SerializeField] private List<GameEventListener> gameEventListeners;
    void OnEnable()
    {
        foreach(GameEventListener eventListener in gameEventListeners)
        {
            eventListener.OnEnable();
        }

    }

    private void OnDisable()
    {
        foreach (GameEventListener eventListener in gameEventListeners)
        {
            eventListener.OnDisable();
        }
    }
}
