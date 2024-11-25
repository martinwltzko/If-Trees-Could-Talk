using UnityEngine;

[System.Serializable]
public class BoneAnchor
{
    public Vector3 position => anchor!=null ? anchor.position + anchor.rotation * anchorOffset : Vector3.zero;
    public Quaternion rotation => anchor!=null ? anchor.rotation * Quaternion.Euler(anchorRotation) : Quaternion.identity;
    public Transform transform => anchor;
    
    [SerializeField] private Transform anchor;
    [SerializeField] private Vector3 anchorOffset;
    [SerializeField] private Vector3 anchorRotation;

    public bool Debug = false;
}