 using UnityEngine;

namespace FlameArthur.Editor
{
    public class PoseWrapper : MonoBehaviour
    {
        [SerializeField] private SkinnedMeshRenderer SkinnedMeshRenderer;
        [SerializeField] private Transform Transform;
        
        public Vector3 Position { get; private set; }
        public Quaternion Rotation { get; private set; }
        public Vector3 Scale { get; private set; }
        public Transform BoneRoot{ get; private set; }
        public bool Initialized { get; private set; } = false;
        
        public void GeneratePose()
        {
            Position = Transform.position;
            Rotation = Transform.rotation;
            Scale = Transform.localScale;
            BoneRoot = SkinnedMeshRenderer.rootBone;
            
            Initialized = true;
        }
        
        public void SetDirty()
        {
            Initialized = false;
        }
    }
}