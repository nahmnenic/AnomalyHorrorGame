using UnityEngine;

namespace UI
{
    public class MainMenu : MonoBehaviour
    {
        private void Start()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
