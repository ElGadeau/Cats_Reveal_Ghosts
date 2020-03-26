using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private float speed = 1;
    
    private Transform _player = null;
    private bool _isLerping = false;
    private bool _isAxisInUse = false;
    private float _targetRotation = 0;

    private void Awake()
    {
        _player = GameObject.FindGameObjectsWithTag("Player")[0].transform;
        transform.LookAt(_player.transform.position);
    }

    //using late update to make sure the player position is updated
    private void LateUpdate()
    {
        CheckInput();
        ApplyRotation();
    }

    //Check for the input and use it on press only
    private void CheckInput()
    {
        if (Input.GetAxisRaw("Look") != 0.0f && !_isAxisInUse)
        {
            _isLerping = true;
            _isAxisInUse = true;
            float angle = 90 * Input.GetAxisRaw("Look");
            offset = Quaternion.AngleAxis(angle, Vector3.up) * offset;
            
            _targetRotation += angle;
            
            if (_targetRotation > 360)
                _targetRotation -= 360.0f;
            if (_targetRotation < 0)
                _targetRotation += 360.0f;
        }
        else if (Input.GetAxisRaw("Look") == 0.0f)
        {
            _isAxisInUse = false;
        }
    }

    //rotate the camera around the player by 90 degree
    private void ApplyRotation()
    {
        Vector3 position = _player.transform.position;
        if (_isLerping)
        {
            if (Math.Abs(transform.rotation.eulerAngles.y - _targetRotation) < 0.5f)
            {
                _isLerping = false;
                transform.position = position + offset;
                transform.LookAt(position);
            }
            else
            {
                transform.position = Vector3.Slerp(transform.position, position + offset, Time.deltaTime * speed);
                transform.LookAt(position);
            }
        }
        else
            transform.position = position + offset;
    }
}