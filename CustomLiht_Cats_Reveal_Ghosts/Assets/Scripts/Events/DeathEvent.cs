using UnityEngine;
using UnityEngine.Events;
using WorldManagement;

namespace Events
{
    public class DeathEvent : MonoBehaviour
    {
        [Header("Respawn")]
        [SerializeField] private bool entitys = false;
        [SerializeField] private bool world = false;
    
        private UnityEvent Death = new UnityEvent();

        private void Awake()
        {
            if (world)
            {
                Death.AddListener(gameObject.GetComponent<LevelGenerator>().RegenerateWorld);
                entitys = true;
            }
        
            if (entitys)
                Death.AddListener(gameObject.GetComponent<EntitySpawn>().RegenerateEntitys);
        
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
