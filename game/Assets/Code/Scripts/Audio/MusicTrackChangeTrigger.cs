using Sirenix.OdinInspector;
using UnityEngine;

namespace Code.Scripts.Audio
{
    public class MusicTrackChangeTrigger : MonoBehaviour
    {
        [SerializeField] private MusicTrack musicTrack;
        
        [Button]
        private void Invoke()
        {
            AudioManager.Instance.SetMusicParameter("track", musicTrack);
        }

        
    }
}