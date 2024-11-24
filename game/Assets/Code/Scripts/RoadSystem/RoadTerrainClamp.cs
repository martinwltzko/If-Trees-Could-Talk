using System.Linq;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class RoadTerrainClamp : MonoBehaviour
{
    [SerializeField] private Terrain terrain;
    [SerializeField] private SplineContainer splineContainer;

    private float3[] _splinePositions;
    private int xRes, yRes;
    
    private float[,] _heights;
    [SerializeField, Range(0,1f)] private float height = 1f;
    
    [Button]
    void Test()
    {
        xRes = terrain.terrainData.heightmapResolution;
        yRes = terrain.terrainData.heightmapResolution;
        _splinePositions = splineContainer.Splines[0].Knots.Select(knot => knot.Position).ToArray();
        
        _heights = terrain.terrainData.GetHeights(0, 0, xRes, yRes);

        for(int x=0; x<xRes; x++)
        for(int y=0; y<yRes; y++)
        {
            _heights[x, y] = height;
        }
        
        terrain.terrainData.SetHeights(0, 0, _heights);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
