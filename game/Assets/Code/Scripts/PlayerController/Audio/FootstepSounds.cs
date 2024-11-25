using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepSounds : MonoBehaviour
{
    [SerializeField] private BoneAnchor _leftFoot;
    [SerializeField] private BoneAnchor _rightFoot;
    
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
        var leftHit = CastFootRay(_leftFoot);
        var rightHit = CastFootRay(_rightFoot);
        
        _leftFootGrounded = leftHit.collider != null;
        _rightFootGrounded = rightHit.collider != null;
        
        if(!_leftFootGrounded) _leftFootSoundPlayed = false;
        if(!_rightFootGrounded) _rightFootSoundPlayed = false;
        
        if ((!_leftFootSoundPlayed && _leftFootGrounded))
        {
            _leftFootSoundPlayed = true;
            PlayFootstepSound();
        }
        
        if ((!_rightFootSoundPlayed && _rightFootGrounded))
        {
            _rightFootSoundPlayed = true;
            PlayFootstepSound();
        }
    }

    private void FixedUpdate()
    {
        _fixedFrame++;
    }

    private RaycastHit CastFootRay(BoneAnchor foot)
    {
        RaycastHit hit;
        Physics.Raycast(foot.position, Vector3.down, out hit, _stepHeightThreshold, ~_ignoreMask);
        return hit;
    }
    
    private bool CanPlayFootstep => _fixedFrame > _frameStepWasPlayed + _minStepInterval/Time.fixedDeltaTime;
    private void PlayFootstepSound()
    {
        if (!CanPlayFootstep) return;
        
        _frameStepWasPlayed = _fixedFrame;
        //Debug.Log("Playing footstep sound");
        _soundRelay.PlaySoundOneShot("Footstep");
    }

    private void OnDrawGizmos()
    {
        if(_leftFoot != null)
            Gizmos.DrawRay(_leftFoot.position, Vector3.down * _stepHeightThreshold);
        if(_rightFoot != null)
            Gizmos.DrawRay(_rightFoot.position, Vector3.down * _stepHeightThreshold);
    }
}
