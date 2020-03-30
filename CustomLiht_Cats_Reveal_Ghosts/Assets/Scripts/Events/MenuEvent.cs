using UnityEngine;
using UnityEngine.SceneManagement;

namespace Events
{
    public class MenuEvent : MonoBehaviour
    {
        // [SerializeField] private Scene NextScene;
        
        public void StartGame()
        {
            SceneManager.LoadScene("Lilian-Testing");
        }

        public void QuitGame()
        {
            Application.Quit();
        }
        
    }
}
