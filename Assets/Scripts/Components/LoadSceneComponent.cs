using UnityEngine;
using UnityEngine.SceneManagement;

namespace Components
{
    public class LoadSceneComponent : MonoBehaviour
    {
        [SerializeField] private string sceneName;

        public void LoadScene()
        {
            SceneManager.LoadScene(sceneName);
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
