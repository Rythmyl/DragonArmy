
using UnityEngine;

public class audioManager : MonoBehaviour
{

    public static audioManager Instance;

    [Header("----- Volume Settings -----")]
    [Range(0f, 1f)]
    [SerializeField] float masterVolume = 1f;

    [Range(0f, 1f)]
    [SerializeField] float musicvolume = 0.7f;

    [Range(0f, 1f)]
    [SerializeField] float sfxVolume = 1f;

    [Header("----- Pause Duck Volume ------")]
    [Range(0f, 1f)]
    [SerializeField] float pausedMusicVolume = 0.25f;

    [Header("----- Music Sources -----")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("------ Music Clips -----")]
    public AudioClip menuMusic;
    public AudioClip gameMusic;

    [Header("----- UI SFX -----")]
    public AudioClip uiClick;
    public AudioClip uiHover;

    private float originalMusicVolume;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);

        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);

        originalMusicVolume = musicvolume;

        ApplyVolumes();
    }

    private void OnValidate()
    {
        ApplyVolumes();
    }

    private void ApplyVolumes()
    {
        if (musicSource != null)
            musicSource.volume = masterVolume * musicvolume;

        if (sfxSource != null)
            sfxSource.volume = masterVolume * sfxVolume;
    }

    public void SetMasterVolume(float value)
    {
        masterVolume = Mathf.Clamp01(value);

        if (Application.isPlaying)
        {
            UnityEditor.EditorUtility.SetDirty(this);
        }

        ApplyVolumes();
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.Save();
    }

    public float GetMasterVolume() => masterVolume;
   

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;

        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.loop = true;
        ApplyVolumes();
        musicSource.Play();
    }

    public void PlayMenuMusic()
    {
        PlayMusic(menuMusic);
    }

    public void PlayGameMusic()
    {
        PlayMusic(gameMusic);
    }

    public void DuckMusic()
    {
        musicvolume = pausedMusicVolume;
        ApplyVolumes();

    }

    public void UnduckMusic()
    {
        musicvolume = originalMusicVolume;
        ApplyVolumes();
    }
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, masterVolume * sfxVolume);
    }

    public void PlayUIClick()
    {
        PlaySFX(uiClick);
    }

    public void PlayUIHover()
    {
        PlaySFX(uiHover);
    }


}
