using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyNamespace
{
    public class SplashScreen : MonoBehaviour
    {
        [SerializeField] private GameObject help;

        public void ToggleHelp()
        {
            help.SetActive(!help.activeSelf);
        }

        public void StartGame()
        {
            SceneManager.LoadScene(1);
        }

        public void MainMenu()
        {
            SceneManager.LoadScene(0);
        }
    }
}