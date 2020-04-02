using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Character
{
    public class CameraControl : MonoBehaviour
    {
        [SerializeField] private Vector3 offset;
        [SerializeField] private float speed = 1;
        [SerializeField] private Image blackScreen = null;
        [SerializeField] private GameObject Score = null;

        private Transform _player = null;
        private bool _isLerping = false;
        private bool _isAxisInUse = false;
        private bool _gameFinished = false;
        private float _targetRotation = 0;

        private void Start()
        {
            Score.SetActive(false);
            
            var texts = Score.GetComponentsInChildren<Text>();

            foreach (Text text in texts)
            {
                text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
            }
        }

        private void Update()
        {
            if (_gameFinished && Input.anyKeyDown)
                SceneManager.LoadScene("MainMenu");
        }

        //using late update to make sure the player position is updated
        private void LateUpdate()
        {
            if (_player == null)
                FindPlayer();
            else
            {
                CheckInput();
                ApplyRotation();
            }
        }

        private void FindPlayer()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player)
            {
                _player = player.transform;
                ApplyRotation();
            }
        }

        //Check for the input and use it on press only
        private void CheckInput()
        {
            if (Math.Abs(Input.GetAxisRaw("Look")) != 0.0f && !_isAxisInUse)
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
            else if (Math.Abs(Input.GetAxisRaw("Look")) == 0.0f)
            {
                _isAxisInUse = false;
            }
        }

        //rotate the camera around the player by 90 degree
        private void ApplyRotation()
        {
            Vector3 position = _player.position;
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
            {
                transform.position = position + offset;
                transform.LookAt(position);
            }
        }

        public void OnDeath(float p_time)
        {
            FadeToBlack(p_time / 2);
        }

        private void FadeToBlack(float p_time)
        {
            blackScreen.color = Color.black;
            blackScreen.canvasRenderer.SetAlpha(0.0f);
            blackScreen.CrossFadeAlpha(1.0f, p_time, false);
        }

        public void FadeFromBlack(float p_time)
        {
            blackScreen.color = Color.black;
            blackScreen.canvasRenderer.SetAlpha(1.0f);
            blackScreen.CrossFadeAlpha(0.0f, p_time, false);
        }

        public void EndGame(int p_score)
        {
            blackScreen.CrossFadeAlpha(1.0f, 0.5f, false);
            var texts = Score.GetComponentsInChildren<Text>();

            foreach (Text text in texts)
            {
                if (text.name == "data")
                {
                    text.text = p_score.ToString();
                }
                StartCoroutine(FadeTextIn(0.5f, text));
            }

        }

        private IEnumerator FadeTextIn(float p_time, Text p_text)
        {
            p_text.color = new Color(p_text.color.r, p_text.color.g, p_text.color.b, 0);
            while (p_text.color.a < 1.0f)
            {
                p_text.color = new Color(p_text.color.r, p_text.color.g, p_text.color.b, p_text.color.a + (Time.deltaTime / p_time));
                yield return null;
            }

            _gameFinished = true;
        }
        
    }
}