using UnityEngine;

namespace Code.Scripts.MessageSystem
{
    [System.Serializable]
    public class WebMessage
    {
        public int id;
        public string owner;
        public string message;
        public Vector3 position;
        public Vector3 normal;
    }
}