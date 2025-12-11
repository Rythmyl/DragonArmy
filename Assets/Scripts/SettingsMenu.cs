using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private Slider masterVolumeSlider;
    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
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
