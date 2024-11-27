using AdvancedController;
using UnityEngine;

public class WorldTransformCopy : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    
    public void PasteTransform(Transform t)
    {
        playerController.transform.SetPositionAndRotation(t.position, t.rotation);
    }
}
