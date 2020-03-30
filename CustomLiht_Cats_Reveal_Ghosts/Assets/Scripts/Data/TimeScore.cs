using UnityEngine;

namespace Data
{
    public class TimeScore : MonoBehaviour
    {
        private float _timerScore = 0;
        private bool _shouldRun = true;
        private int _zeroDeathScore = 100;
        private int _numberOfDeath = 0;
    
        void Update()
        {
            if (_shouldRun)
                _timerScore += Time.deltaTime;
        }

        public void OnDeath()
        {
            ++_numberOfDeath;
        }

        public int GetScore()
        {
            float tmpScore = 0.0f;

            tmpScore = 1000.0f / _timerScore;
            if (_numberOfDeath == 0)
                tmpScore += _zeroDeathScore;
        
            return (int)tmpScore;
        }
        public float TimerScore => _timerScore;

        public bool ShouldRun
        {
            set => _shouldRun = value;
        }
    }
}
