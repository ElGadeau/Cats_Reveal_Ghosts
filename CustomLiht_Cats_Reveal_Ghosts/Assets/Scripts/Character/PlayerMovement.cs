using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private bool _isPlaying = false;
    
    [Header("Control Settings")]
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float mouseSensitivity = 1.0f;
    
    [Header("Controlled Objects")]
    [SerializeField] private Camera playerCamera = null;
    
    // Start is called before the first frame update
    private void Start()
    {
        if (playerCamera == null)
        {
            Debug.Log("Camera is not set");
            Debug.Break();
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #elif UNITY_WEBPLAYER
            Application.OpenURL(webplayerQuitURL);
            #else
            Application.Quit();
            #endif
        }

        if (_isPlaying)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        PlayerControl();
        CameraControl();
    }

    private void PlayerControl()
    {
        float ver = Input.GetAxis("Vertical");
        float hor = Input.GetAxis("Horizontal");
        
        Vector3 forward = transform.forward * ver;
        Vector3 right = transform.right * hor;
        
        Vector3 mov = forward + right;

        mov *= speed;
        mov *= Time.deltaTime;
        gameObject.GetComponent<Rigidbody>().MovePosition(transform.position + mov);
    }

    private void CameraControl()
    {
        float rotX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float rotY = -Input.GetAxis("Mouse Y") * mouseSensitivity;

        playerCamera.transform.Rotate(rotY, 0, 0);
        transform.Rotate(0, rotX, 0); 
    }
}
