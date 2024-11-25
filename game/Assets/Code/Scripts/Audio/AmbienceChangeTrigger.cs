using Sirenix.OdinInspector;
using UnityEngine;

namespace Code.Scripts.Audio
{
    public class AmbienceChangeTrigger : MonoBehaviour
    {
        [Header("Parameter Change")] [SerializeField]
        private string parameterName;

        [SerializeField] private float parameterValue;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                AudioManager.Instance.SetAmbienceParameter(parameterName, parameterValue);
            }
        }

        [Button]
        private void Invoke()
        {
            AudioManager.Instance.SetAmbienceParameter(parameterName, parameterValue);
        }
    }
}