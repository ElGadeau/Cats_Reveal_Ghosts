using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private bool _isPlaying = false;
    
    [Header("Control Settings")]
    [SerializeField] private float speed = 5.0f;

    [SerializeField] private Camera camera = null;
    
    // Start is called before the first frame update
    private void Start()
    {
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
    }

    private void PlayerControl()
    {
        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        input = Vector2.ClampMagnitude(input, 1);

        //calculate camera forward
        var camRot = camera.transform.rotation.eulerAngles;


        var angle = Quaternion.Euler(0, camRot.y, 0);
        
        Vector3 camF = angle * Vector3.forward;
        // Vector3 camF = camera.transform.forward;
        Vector3 camR = camera.transform.right;

        Vector3 mov = (camF * input.y + camR * input.x) * Time.deltaTime * speed;
        
        
        if (mov.x != 0.0f || mov.z != 0.0f)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(mov.normalized), 0.2f);
        
        
        gameObject.GetComponent<Rigidbody>().MovePosition(transform.position + mov);

        /*
        float ver = Input.GetAxis("Vertical");
        float hor = Input.GetAxis("Horizontal");
        
        Vector3 mov = new Vector3(hor, 0, ver);

        // Vector3 mov = forward;
        if (mov.x == 1.0f || mov.z == 1.0f)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(mov.normalized), 0.2f);
        
        if (mov.x == 1.0f || mov.z == 1.0f)
            mov.Normalize();

        mov.Scale(camera.transform.forward);
        mov *= speed;
        mov *= Time.deltaTime;
        gameObject.GetComponent<Rigidbody>().MovePosition(transform.position + mov);
        */

    }
}
