using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Settings
{
    public class MainMenu : MonoBehaviour
    {
        public Animator transitionAnim;

        public void PlayGame()
        {
            StartCoroutine(SceneTransition());
        }

        private IEnumerator SceneTransition()
        {
            transitionAnim.SetTrigger("FadeIn");
            yield return new WaitForSeconds(1.5f);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        public void QuitGame()
        {
            Debug.Log("Quit!");
            Application.Quit();
        }

        public void BackToMenu()
        {
            ResumeMenu.GameIsPaused = false;
        }
    }
}
