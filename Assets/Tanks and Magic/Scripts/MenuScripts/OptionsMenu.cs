using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System.Linq;
using UnityEngine.UI;

namespace Settings
{
    public class OptionsMenu : MonoBehaviour
    {
        public AudioMixer masterMixer;

        private const string resolutionWidthPlayerPrefKey = "ResolutionWidth";
        private const string resolutionHeightPlayerPrefKey = "ResolutionHeight";
        public TMPro.TMP_Dropdown resolutionDropdown;
        public TMPro.TMP_Dropdown graphicsDropdown;
        public Slider slider_Master;
        public Slider slider_Music;
        public Slider slider_SFX;
        public Toggle toggle_VSync;
        public AudioClip point;

        Resolution[] resolutions;
        Resolution selectedResolution;

        void Start()
        {
            resolutions = Screen.resolutions.Select(resolution => new Resolution { width = resolution.width, height = resolution.height }).Distinct().ToArray();
            LoadSettings();
            CreateResolutionDropdown();
        }

        private void LoadSettings() //load the saves
        {
            selectedResolution = new Resolution();
            selectedResolution.width = PlayerPrefs.GetInt(resolutionWidthPlayerPrefKey, Screen.currentResolution.width);
            selectedResolution.height = PlayerPrefs.GetInt(resolutionHeightPlayerPrefKey, Screen.currentResolution.height);
            SetQuality(PlayerPrefs.GetInt("Graphics", 0));
            SetMasterVolume(PlayerPrefs.GetFloat("MasterVolume", 0));
            SetMusicVolume(PlayerPrefs.GetFloat("MusicVolume", 0));
            SetSFXVolume(PlayerPrefs.GetFloat("SFXVolume", 0));

            if (!CheckMobile.Instance.CheckIfWebGLIsMobile())
            {
                Screen.SetResolution(
                selectedResolution.width,
                selectedResolution.height,
                true
            );
            }
        }

        private void CreateResolutionDropdown()
        {
            resolutionDropdown.ClearOptions();
            List<string> options = new List<string>();
            int currentResolutionIndex = 0;
            for (int i = 0; i < resolutions.Length; i++)
            {
                string option = resolutions[i].width + " x " + resolutions[i].height;
                options.Add(option);
                if (Mathf.Approximately(resolutions[i].width, selectedResolution.width) && Mathf.Approximately(resolutions[i].height, selectedResolution.height))
                {
                    currentResolutionIndex = i;
                }
            }
            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();
        }

        public void SetResolution(int resolutionIndex)
        {
            selectedResolution = resolutions[resolutionIndex];
            if (!CheckMobile.Instance.CheckIfWebGLIsMobile())
            {
                Screen.SetResolution(
                selectedResolution.width,
                selectedResolution.height,
                true
            );
            }
            PlayerPrefs.SetInt(resolutionWidthPlayerPrefKey, selectedResolution.width);
            PlayerPrefs.SetInt(resolutionHeightPlayerPrefKey, selectedResolution.height);
        }

        public void SetMasterVolume(float volume)
        {
            slider_Master.value = volume;
            masterMixer.SetFloat("MasterVolume", volume);
            PlayerPrefs.SetFloat("MasterVolume", volume);
        }

        public void SetMusicVolume(float volume)
        {
            slider_Music.value = volume;
            masterMixer.SetFloat("MusicVolume", volume);
            PlayerPrefs.SetFloat("MusicVolume", volume);
        }

        public void SetSFXVolume(float volume)
        {
            slider_SFX.value = volume;
            masterMixer.SetFloat("SFX", volume);
            PlayerPrefs.SetFloat("SFXVolume", volume);
        }

        public void SetFullscreen(bool isFullscreen)
        {
            Screen.fullScreen = isFullscreen;
        }

        public void Mute(bool isMute)
        {
            if (isMute)
            {
                SetMasterVolume(-80);
            }
            else SetMasterVolume(0);
        }

        public void MusicOff(bool isMute)
        {
            if (isMute)
            {
                SetMusicVolume(-80);
            }
            else SetMusicVolume(0);
        }

        public void SFXOff(bool isMute)
        {
            if (isMute)
            {
                SetSFXVolume(-80);
            }
            else SetSFXVolume(0);
        }

        public void SetQuality(int qualityIndex)
        {
            graphicsDropdown.value = qualityIndex;
            QualitySettings.SetQualityLevel(qualityIndex);
            PlayerPrefs.SetInt("Graphics", qualityIndex);
        }

        private void OnApplicationQuit()
        {
            PlayerPrefs.Save();
        }
    }
}