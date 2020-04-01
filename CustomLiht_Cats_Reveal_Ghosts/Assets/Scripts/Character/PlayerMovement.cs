using System.Collections;
using Data;
using Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Character
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Control Settings")]
        [SerializeField] private float speed = 5.0f;

        private Camera _camera = null;
        private UnityEvent _deathEvent = new UnityEvent();

        public bool isDead = false;

        private GameObject _timer = null;

        private void Awake()
        {
            _timer = GameObject.FindGameObjectWithTag("Timer");
            _camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            _deathEvent.AddListener(OnDeath);
            _deathEvent.AddListener(_camera.GetComponent<CameraControl>().OnDeath);
            _deathEvent.AddListener(GameObject.FindGameObjectWithTag("GameManager").GetComponent<DeathEvent>().OnDeath);
            _deathEvent.AddListener(_timer.GetComponent<TimeScore>().OnDeath);
        }
    
        private void Update()
        {
            if (isDead)
                return;
        
            PlayerControl();
            if (transform.position.y <= -50.0f)
                _deathEvent.Invoke();
        }

        //Kill the player when colliding with a ghost
        private void OnCollisionEnter(Collision other)
        {
            if (isDead)
                return;
        
            if (other.gameObject.CompareTag("Ghost"))
                _deathEvent.Invoke();

            if (other.gameObject.CompareTag("EndTile"))
            {
                _timer.GetComponent<TimeScore>().ShouldRun = false;
                _camera.GetComponent<CameraControl>().EndGame(_timer.GetComponent<TimeScore>().GetScore());
                isDead = true;
            }
        }
    
        //Move the player using the direction of the camera
        private void PlayerControl()
        {
            Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            input = Vector2.ClampMagnitude(input, 1);

            //calculate camera forward
            var camRot = _camera.transform.rotation.eulerAngles;
            var angle = Quaternion.Euler(0, camRot.y, 0);
        
            Vector3 camF = angle * Vector3.forward;
            Vector3 camR = _camera.transform.right;

            Vector3 mov = (camF * input.y + camR * input.x) * Time.deltaTime * speed;

            if (mov.x != 0.0f || mov.z != 0.0f)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(mov.normalized), 0.2f);

            gameObject.GetComponent<Rigidbody>().MovePosition(transform.position + mov);
        }

        private void OnDeath()
        {
            var nextRotation = Quaternion.AngleAxis(-90.0f, Vector3.right);
            StartCoroutine(Dying(nextRotation));
            isDead = true;
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
}
