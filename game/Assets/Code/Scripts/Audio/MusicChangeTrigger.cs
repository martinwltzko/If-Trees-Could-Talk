using Sirenix.OdinInspector;
using UnityEngine;

namespace Code.Scripts.Audio
{
    public class MusicChangeTrigger : MonoBehaviour
    {
        [SerializeField] private MusicArea area;
        
        [Button]
        private void Invoke()
        {
            AudioManager.Instance.SetMusicArea(area);
        }
    }
}