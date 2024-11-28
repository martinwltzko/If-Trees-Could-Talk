using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Code.Scripts.Audio
{
    public class MusicTrackChangeTrigger : MonoBehaviour
    {
        [SerializeField] private MusicVocalTrack musicVocalTrack;
        
        [Button]
        private void Invoke()
        {
            AudioManager.Instance.SetMusicParameter("track", musicVocalTrack);
        }

        
    }
}