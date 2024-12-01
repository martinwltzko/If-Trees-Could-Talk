using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;


// FROM https://github.com/flame99999/CopyPose/blob/main/Assets/CopyPose/Scripts/Editor/CopyPoseWindow.cs
namespace FlameArthur.Editor
{
    public class CopyPoseAndTransform : MonoBehaviour
    {
        //[SerializeField] private Transform sourceModel;
        [SerializeField] private Transform targetTransform;
        [SerializeField] private Transform targetModel;
        [SerializeField] private bool humanoidMapping;

        private Transform _sourceRootBoneTran;
        private Transform _targetRootBoneTran;
        
        [Button]
        public void CopyPoseGeneral(Transform poseTransform)
        {
            if (!poseTransform.TryGetComponent<PoseWrapper>(out var pose)) {
                Debug.Log("No PoseWrapper found, can't copy pose");
                return;
            }

            if (!pose.Initialized) {
                Debug.Log("Pose not initialized, can't copy pose");
                return;
            }

            Debug.Log($"<color=green>Pose loaded with parameters (pos={pose.Position}, rot={pose.Rotation}), scale={pose.Scale}, boneRoot={pose.BoneRoot})</color>");
            
            var targetSkinnedMeshRenderer = targetModel.GetComponentInChildren<SkinnedMeshRenderer>();
            _targetRootBoneTran = targetSkinnedMeshRenderer.rootBone;
            foreach (Transform boneTran in pose.BoneRoot)
            {
                CopyTransform("", boneTran);
            }

            targetTransform.position = pose.Position;
            targetTransform.rotation = pose.Rotation;
            pose.SetDirty();
        }

        [Button]
        public void CopyPoseHumanoid(Transform sourceModel)
        {
            var sourceAnimator = sourceModel.GetComponentInChildren<Animator>();
            var targetAnimator = targetModel.GetComponentInChildren<Animator>();
            if (sourceAnimator == null || targetAnimator == null)
            {
                Debug.Log("No Animator found, can't copy pose");
                return;
            }
            var humanBoneTypes = Enum.GetValues(typeof(HumanBodyBones)) as HumanBodyBones[];
            for (var i = 0; i < humanBoneTypes.Length - 1; i++)
            {
                var sourceBone = sourceAnimator.GetBoneTransform(humanBoneTypes[i]);
                var targetBone = targetAnimator.GetBoneTransform(humanBoneTypes[i]);
                if (sourceBone == null || targetBone == null) continue;
                targetBone.localRotation = sourceBone.localRotation;
                targetBone.localPosition = sourceBone.localPosition;
            }
            
            targetTransform.position = sourceModel.position;
            targetTransform.rotation = sourceModel.rotation;
        }


        private void CopyTransform(string parentPath, Transform curTran)
        {
            var targetTran = _targetRootBoneTran.Find($"{parentPath}{curTran.name}");
            if (targetTran != null)
            {
                targetTran.localRotation = curTran.localRotation;
                targetTran.localPosition = curTran.localPosition;
            }

            foreach (Transform childBoneTran in curTran)
            {
                CopyTransform($"{parentPath}{curTran.name}/", childBoneTran);
            }
        }
    }
}