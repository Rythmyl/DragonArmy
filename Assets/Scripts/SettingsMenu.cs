using System.Runtime.InteropServices;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private Slider masterVolumeSlider;
    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }

    [SerializeField] private TMP_Dropdown resolutionDropdown;
    
    Resolution[] resolutions;
    private void Start()
    {
       resolutions= Screen.resolutions;

        resolutionDropdown.ClearOptions();

        List<string>options= new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width==Screen.currentResolution.width&&
                resolutions[i].height==Screen.currentResolution.height)
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
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width,resolution.height,Screen.fullScreen);
    }
    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex); 
    }
    private void OnEnable()
    {
        if (masterVolumeSlider != null && audioManager.Instance != null)
        {
            masterVolumeSlider.value = audioManager.Instance.GetMasterVolume();
            masterVolumeSlider.onValueChanged.RemoveAllListeners();
            masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        }


    }

    private void OnDisable()
    {
        if (masterVolumeSlider != null)
            masterVolumeSlider.onValueChanged.RemoveAllListeners();
    }

    public void SetMasterVolume(float value)
    {
        if (audioManager.Instance != null) 
            audioManager.Instance.SetMasterVolume(value);
    }

}
