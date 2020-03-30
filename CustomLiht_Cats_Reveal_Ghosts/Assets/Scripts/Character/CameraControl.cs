using System;
using UnityEngine;
using UnityEngine.UI;

namespace Character
{
    public class CameraControl : MonoBehaviour
    {
        [SerializeField] private Vector3 offset;
        [SerializeField] private float speed = 1;
        [SerializeField] private Image blackScreen = null;
        [SerializeField] private float fadeTime = 1.0f;
    
        private Transform _player = null;
        private bool _isLerping = false;
        private bool _isAxisInUse = false;
        private float _targetRotation = 0;
    
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

        public void OnDeath()
        {
            FadeToBlack();
        }

        private void FadeToBlack()
        {
            blackScreen.color = Color.black;
            blackScreen.canvasRenderer.SetAlpha(0.0f);
            blackScreen.CrossFadeAlpha(1.0f, fadeTime, false);
        }

        public void FadeFromBlack()
        {
            blackScreen.color = Color.black;
            blackScreen.canvasRenderer.SetAlpha(1.0f);
            blackScreen.CrossFadeAlpha(0.0f, fadeTime, false);
        }
    }
}