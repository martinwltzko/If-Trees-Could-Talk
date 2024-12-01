using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class FootstepSounds : MonoBehaviour
{
    [SerializeField] private BoneAnchor _leftFoot;
    [SerializeField] private BoneAnchor _rightFoot;

    [SerializeField] private GroundTypeMapper _typeMapper;
    [SerializeField] private SoundRelay _soundRelay;
    [SerializeField, Range(0, 0.2f)] private float _stepHeightThreshold = .15f;
    [SerializeField, Range(0, 1f)] private float _minStepInterval = 0.25f;
    
    [SerializeField] private LayerMask _ignoreMask;
    
    private bool _leftFootGrounded;
    private bool _leftFootSoundPlayed;
    
    private bool _rightFootGrounded;
    private bool _rightFootSoundPlayed;

    private int _frameStepWasPlayed;
    private int _fixedFrame;
    // Update is called once per frame
    void Update()
    {
        var leftHit = CastRay(_leftFoot.position, Vector3.down, _stepHeightThreshold);
        var rightHit = CastRay(_rightFoot.position, Vector3.down, _stepHeightThreshold);
        
        _leftFootGrounded = leftHit.collider != null;
        _rightFootGrounded = rightHit.collider != null;
        
        if(!_leftFootGrounded) _leftFootSoundPlayed = false;
        if(!_rightFootGrounded) _rightFootSoundPlayed = false;
        var center = (_leftFoot.position + _rightFoot.position) / 2f;
        if ((!_leftFootSoundPlayed && _leftFootGrounded))
        {
            _leftFootSoundPlayed = true;
            PlayFootstepSound(center);
        }
        
        if ((!_rightFootSoundPlayed && _rightFootGrounded))
        {
            _rightFootSoundPlayed = true;
            PlayFootstepSound(center);
        }
    }

    //TODO: Implement in future
    private void CalculateFootVelocity()
    {
        
    }

    //TODO: Implement properly
    private TerrainLayer CheckTerrainAtPosition(Terrain terrain, Vector3 position)
    {
        var terrainPosition = position - terrain.transform.position;
        var terrainData = terrain.terrainData;

        var mapPosition =
            new Vector3(terrainPosition.x / terrainData.size.x, 0, terrainPosition.z / terrainData.size.z);

        var xCoord = mapPosition.x * terrainData.alphamapWidth;
        var zCoord = mapPosition.z * terrainData.alphamapHeight;
        var posX = (int)xCoord;
        var posZ = (int)zCoord;

        var mapData = terrain.terrainData.GetAlphamaps(posX, posZ, 1, 1);
        var layerCollections = new float[mapData.GetUpperBound(2) + 1];

        for (int i = 0; i < layerCollections.Length; i++)
        {
            layerCollections[i] = mapData[0, 0, i];
        }

        var highest = 0f;
        var maxIndex = 0;
        for (int i = 0; i < layerCollections.Length; i++)
        {
            if (!(layerCollections[i] > highest)) continue;

            maxIndex = i;
            highest = layerCollections[i];
        }

        return terrain.terrainData.terrainLayers[maxIndex];
    }

    private void FixedUpdate()
    {
        _fixedFrame++;
    }

    private RaycastHit CastRay(Vector3 position, Vector3 direction, float distance)
    {
        RaycastHit hit;
        Physics.Raycast(position, direction, out hit, distance, ~_ignoreMask);
        return hit;
    }
    
    private bool CanPlayFootstep => _fixedFrame > _frameStepWasPlayed + _minStepInterval/Time.fixedDeltaTime;
    private void PlayFootstepSound(Vector3 position)
    {
        if (!CanPlayFootstep) return;

        var hit = CastRay(position + Vector3.up, Vector3.down, Mathf.Infinity);
        if (hit.collider == null) return;

        GroundType groundType = GroundType.Grass;
        if (hit.transform.TryGetComponent<Terrain>(out var terrain)) {
            var layer = CheckTerrainAtPosition(terrain, position);
            groundType = _typeMapper.Match(layer).Type;
        }
        else if (hit.transform.TryGetComponent<GroundTypeRelay>(out var groundTypeRelay)) {
            groundType = groundTypeRelay.type;
        }
        
        _frameStepWasPlayed = _fixedFrame;
        _soundRelay.PlaySoundWithParameter("Footstep", "ground_type", (float)groundType);
    }

    private void OnDrawGizmos()
    {
        if(_leftFoot != null)
            Gizmos.DrawRay(_leftFoot.position, Vector3.down * _stepHeightThreshold);
        if(_rightFoot != null)
            Gizmos.DrawRay(_rightFoot.position, Vector3.down * _stepHeightThreshold);
    }
}
