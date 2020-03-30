using UnityEngine;
using UnityEngine.Events;
using WorldManagement;

namespace Events
{
    public class DeathEvent : MonoBehaviour
    {
        [Header("Respawn")]
        [SerializeField] private bool ghosts = false;
        [SerializeField] private bool cats = false;
        [SerializeField] private bool world = false;
    
        private UnityEvent Death = new UnityEvent();

        private void Awake()
        {
            if (world)
            {
                Death.AddListener(gameObject.GetComponent<LevelGenerator>().RegenerateWorld);
                ghosts = cats = true;
            }
        
            if (ghosts)
                Death.AddListener(gameObject.GetComponent<EntitySpawn>().RegenerateGhosts);

            if (cats)
                Death.AddListener(gameObject.GetComponent<EntitySpawn>().RegenerateCats);
        
            Death.AddListener(gameObject.GetComponent<EntitySpawn>().RegeneratePlayer);
        }

        public void OnDeath()
        {
            Invoke(nameof(FireEvent), 1.0f);
        }

        private void FireEvent()
        {
            Death.Invoke();
        }
    }
}
