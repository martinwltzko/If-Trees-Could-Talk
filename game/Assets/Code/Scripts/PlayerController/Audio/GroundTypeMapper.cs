using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GroundTypeMapper", fileName = "GroundTypeMapper")]
public class GroundTypeMapper : ScriptableObject
{
    public List<GroundTerrainMapping> TerrainMappings;

    public GroundTerrainMapping Match(TerrainLayer layer)
    {
        var foundMap = TerrainMappings.Find(x => x.Layer == layer);

        if(foundMap == null) {
            Debug.LogError("No match found with layer: " + layer.name);
        }

        return foundMap;
    }
    
    [Serializable]
    public class GroundTerrainMapping
    {
        public TerrainLayer Layer;
        public GroundType Type;
    }
}