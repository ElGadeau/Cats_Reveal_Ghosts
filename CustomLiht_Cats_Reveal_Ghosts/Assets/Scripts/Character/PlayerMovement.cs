using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEditorInternal.VR;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMovement : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject CameraPrefabs = null;
    
    [Header("Control Settings")]
    [SerializeField] private float speed = 5.0f;

    private Camera camera = null;
    private UnityEvent deathEvent = new UnityEvent();

    public bool _isDead = false;

    public GameObject timer = null;

    private void Awake()
    {
        timer = new GameObject("Timer");
        timer.AddComponent<TimeScore>();
        camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        deathEvent.AddListener(OnDeath);
        deathEvent.AddListener(camera.GetComponent<CameraControl>().OnDeath);
        deathEvent.AddListener(GameObject.FindGameObjectWithTag("GameManager").GetComponent<DeathEvent>().OnDeath);
        deathEvent.AddListener(timer.GetComponent<TimeScore>().OnDeath);
    }
    
    private void Update()
    {
        if (_isDead)
            return;
        
        PlayerControl();
        if (transform.position.y <= -50.0f)
            deathEvent.Invoke();
    }

    //Kill the player when colliding with a ghost
    private void OnCollisionEnter(Collision other)
    {
        if (_isDead)
            return;
        
        if (other.gameObject.CompareTag("Ghost"))
            deathEvent.Invoke();

        if (other.gameObject.CompareTag("EndTile"))
        {
            timer.GetComponent<TimeScore>().ShouldRun = false;
            Debug.Log("you won in : " + timer.GetComponent<TimeScore>().TimerScore + " seconds");
            Debug.Log("with a score of : " + timer.GetComponent<TimeScore>().GetScore() + " points");
        }
    }
    
    //Move the player using the direction of the camera
    private void PlayerControl()
    {
        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        input = Vector2.ClampMagnitude(input, 1);

        //calculate camera forward
        var camRot = camera.transform.rotation.eulerAngles;
        var angle = Quaternion.Euler(0, camRot.y, 0);
        
        Vector3 camF = angle * Vector3.forward;
        Vector3 camR = camera.transform.right;

        Vector3 mov = (camF * input.y + camR * input.x) * Time.deltaTime * speed;

        if (mov.x != 0.0f || mov.z != 0.0f)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(mov.normalized), 0.2f);

        gameObject.GetComponent<Rigidbody>().MovePosition(transform.position + mov);
    }

    private void OnDeath()
    {
        var nextRotation = Quaternion.AngleAxis(-90.0f, Vector3.right);
        StartCoroutine(Dying(nextRotation));
        _isDead = true;
    }

    IEnumerator Dying(Quaternion nextRot)
    {
        while (transform.rotation.eulerAngles.x > -90.0f)
        {
            // var currentRot = transform.rotation;

            transform.rotation = Quaternion.Slerp(transform.rotation, nextRot, Time.deltaTime * 6);
            yield return null;
        }
        
        yield return new WaitForEndOfFrame();
    }
}
