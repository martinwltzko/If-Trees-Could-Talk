using Sirenix.OdinInspector;
using UnityEngine;

public class Bottle : MonoBehaviour
{
    [SerializeField] private MeshRenderer bottleRenderer;
    [SerializeField] private MeshRenderer bottleStripRenderer;

    [SerializeField] private Material bottleMaterial;
    [SerializeField] private Material bottleStripMaterial;

    [Button]
    public void RipOffStrip()
    {
        bottleStripRenderer.material = bottleMaterial;
    }
    
    [Button]
    public void RestoreStrip()
    {
        bottleStripRenderer.material = bottleStripMaterial;
    }
}
