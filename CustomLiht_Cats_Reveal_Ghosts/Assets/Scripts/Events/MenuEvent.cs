using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Events
{
    public class MenuEvent : MonoBehaviour
    {
        public GameObject MainMenu, LevelSelection;
        
        private void Awake()
        {
            MainMenu.SetActive(true);
            LevelSelection.SetActive(false);
        }

        public void StartGame(string p_name)
        {
            SceneManager.LoadScene(p_name);
        }

        public void ChooseDifficulty()
        {
            MainMenu.SetActive(false);
            LevelSelection.SetActive(true);
        }

        public void Return()
        {
            LevelSelection.SetActive(false);
            MainMenu.SetActive(true);
        }
        
        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
