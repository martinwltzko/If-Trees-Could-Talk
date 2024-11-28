using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EventHandling
{
    public static class EventBus<T> where T : IEvent
    {
        private static readonly HashSet<IEventBinding<T>> bindings = new();
        
        public static void Register(EventBinding<T> binding) => bindings.Add(binding);
        public static void Unregister(EventBinding<T> binding) => bindings.Remove(binding);

        public static void Raise(T @event)
        {
            foreach (var binding in bindings)
            {
                binding.OnEvent.Invoke(@event);
                binding.OnEventNoArgs.Invoke();
            }
        }

        public static async void Ping(T @event, float timeOutInMs=10000)
        {
            while(timeOutInMs > 0)
            {
                if (bindings.Count > 0) {
                    Raise(@event);
                    break;
                }
                
                await Task.Delay(100);
                timeOutInMs -= 100;
            }
            if(timeOutInMs <= 0)
                Debug.LogWarning($"EventBus<{typeof(T).Name}>: Ping timed out.");
        }
        
        public static async void Repeat(T @event, float intervalInMs=250, int repeatCount=10)
        {
            HashSet<IEventBinding<T>> calledBindings = new();
            while(repeatCount > 0)
            {
                if (bindings.Count > 0) {
                    foreach (var binding in bindings) {
                        if (!calledBindings.Add(binding)) continue;
                        
                        // Call new events
                        binding.OnEvent.Invoke(@event);
                        binding.OnEventNoArgs.Invoke();
                    }
                }
                await Task.Delay((int)intervalInMs);
                repeatCount--;
            }
        }

        static void Clear()
        {
            Debug.Log($"Clearing EventBus<{typeof(T).Name}>...");
            bindings.Clear();
        }
    }
}