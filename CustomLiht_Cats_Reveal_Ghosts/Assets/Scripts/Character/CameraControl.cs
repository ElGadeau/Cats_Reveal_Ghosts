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
    private float targetRotation = 0;
    
    void Start()
    {
        if (player == null)
        {
            Debug.Log("the player is not set in the camera");
            Debug.Break();
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #elif UNITY_WEBPLAYER
            Application.OpenURL(webplayerQuitURL);
            #else
            Application.Quit();
            #endif
        }
        transform.LookAt(player.transform.position);
    }

    void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            isLerping = true;
            offset = Quaternion.AngleAxis(90.0f, Vector3.up) * offset;
            targetRotation = transform.rotation.eulerAngles.y + 90.0f;
            
            if (targetRotation > 360)
                targetRotation -= 360.0f;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            isLerping = true;
            offset = Quaternion.AngleAxis(-90.0f, Vector3.up) * offset;
            targetRotation = transform.rotation.eulerAngles.y - 90.0f;

            if (targetRotation < 0)
                targetRotation += 360.0f;
        }

        if (isLerping)
        {
            if (Math.Abs(transform.rotation.eulerAngles.y - targetRotation) < 1.0f)
            {
                isLerping = false;
                transform.position = player.transform.position + offset;
                transform.LookAt(player.transform.position);
            }
            else
            {
                transform.position = Vector3.Slerp(transform.position, player.transform.position + offset, Time.deltaTime * speed);
                transform.LookAt(player.transform.position);
            }
        }
        else
            transform.position = player.transform.position + offset;
        
    }
}























