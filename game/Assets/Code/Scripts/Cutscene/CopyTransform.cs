using Sirenix.OdinInspector;
using UnityEngine;

namespace FlameArthur.Editor
{
    public class CopyTransform : MonoBehaviour
    {
        [SerializeField] private Transform sourceModel;
        [SerializeField] private Transform targetModel;
        [SerializeField] private bool humanoidMapping;

        private Transform _sourceRootBoneTran;
        private Transform _targetRootBoneTran;
        
        [Button]
        private void Copy()
        {
            targetModel.position = sourceModel.position;
            targetModel.rotation = sourceModel.rotation;
        }
    }
}