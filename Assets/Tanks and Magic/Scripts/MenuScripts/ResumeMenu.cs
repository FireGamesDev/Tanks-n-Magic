using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Settings
{
    public class ResumeMenu : MonoBehaviour
    {
        public static bool GameIsPaused = false;
        public GameObject pauseMenuUI;
        public AudioMixer masterMixer;
        public TankScripts.TankShooting tankShootingScript;

        private void Update()
        {
            if (Chat.ChatInputField.GetComponent<UnityEngine.UI.Mask>().showMaskGraphic)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (GameIsPaused)
                {
                    Resume();
                }
                else
                {
                    Pause();
                }
            }
        }

        public void Resume()
        {
            masterMixer.SetFloat("MasterVolume", PlayerPrefs.GetFloat("MasterVolume"));
            pauseMenuUI.SetActive(false);
            GameIsPaused = false;
            if (tankShootingScript != null)
            {
                tankShootingScript.canShoot = true; // if the game is resumed can shoot
            }
        }

        public void Pause()
        {
            masterMixer.SetFloat("MasterVolume", PlayerPrefs.GetFloat("MasterVolume") -10f);
            pauseMenuUI.SetActive(true);
            GameIsPaused = true;
            if (tankShootingScript != null)
            {
                tankShootingScript.canShoot = false; // if the game is paused can't shoot
            }
        }
    }
}
