using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private Transform player = null;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float speed = 1;
    private bool isLerping = false;
    private bool isAxisInUse = false;
    [SerializeField] private float targetRotation = 0;

    private void Awake()
    {
        // if (player == null)
        // {
        //     Debug.LogError("the player is not set in the camera");
        //     Debug.Break();
        //     #if UNITY_EDITOR
        //     UnityEditor.EditorApplication.isPlaying = false;
        //     #elif UNITY_WEBPLAYER
        //     Application.OpenURL(webplayerQuitURL);
        //     #else
        //     Application.Quit();
        //     #endif
        // }

        player = GameObject.FindGameObjectsWithTag("Player")[0].transform;
        transform.LookAt(player.transform.position);
    }

    private void LateUpdate()
    {
        CheckInput();
        ApplyRotation();
    }

    private void CheckInput()
    {
        if (Input.GetAxisRaw("Look") != 0.0f && !isAxisInUse)
        {
            isLerping = true;
            isAxisInUse = true;
            float angle = 90 * Input.GetAxisRaw("Look");
            offset = Quaternion.AngleAxis(angle, Vector3.up) * offset;
            
            targetRotation += angle;
            
            if (targetRotation > 360)
                targetRotation -= 360.0f;
            if (targetRotation < 0)
                targetRotation += 360.0f;
        }
        else if (Input.GetAxisRaw("Look") == 0.0f)
        {
            isAxisInUse = false;
        }
    }

    private void ApplyRotation()
    {
        Vector3 position = player.transform.position;
        if (isLerping)
        {
            if (Math.Abs(transform.rotation.eulerAngles.y - targetRotation) < 0.5f)
            {
                isLerping = false;
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

    public Vector3 Offset
    {
        set => offset = value;
    }
}























