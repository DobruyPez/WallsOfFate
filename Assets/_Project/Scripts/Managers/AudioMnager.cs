using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource uiSource;

    [SerializeField] private AudioClip defaultMusic; // ������ �� ��������� ��� ������ �����
    [SerializeField] private AudioClip loadingMusic;   // ������ ������������ ������

    [Header("Mixer Snapshots")]
    [SerializeField] private AudioMixerSnapshot normalSnapshot;   // ��������� ����������� �����
    [SerializeField] private AudioMixerSnapshot loadingSnapshot;  // Snapshot, ��� ���� ����� ��������
    [SerializeField] private float snapshotTransitionTime = 0.5f;   // ����� ��������

    [Header("Mini-game")]
    [SerializeField] private AudioClip miniGameMusic;   // ������ ��� ����-����

    private AudioClip _previousMusic;                   // ���������, ��� ������ ������
    private bool _isInMiniGame;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        LoadVolumeSettings();

        if (defaultMusic != null)
        {
            PlayMusic(defaultMusic);
        }
    }

    public static AudioManager GetInstance()
    {
        return Instance;
    }

    public void PlayMusic(AudioClip clip)
    {
        if (musicSource.isPlaying && musicSource.clip == clip) return;
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    /// <summary>
    /// ����������� ������ ������������ ������.
    /// </summary>
    public void PlayLoadingMusic()
    {
        if (loadingMusic != null)
        {
            PlayMusic(loadingMusic);
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void PlayUI(AudioClip clip)
    {
        uiSource.PlayOneShot(clip);
    }

    public void SetVolume(string parameter, float volume)
    {
        if (volume == 0f)
        {
            audioMixer.SetFloat(parameter, -80f);
        }
        else
        {
            audioMixer.SetFloat(parameter, Mathf.Log10(volume) * 20);
        }
    }

    private void LoadVolumeSettings()
    {
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        float uiVolume = PlayerPrefs.GetFloat("UIVolume", 1f);

        SetVolume("Volume_Music", musicVolume);
        SetVolume("Volume_SFX", sfxVolume);
        SetVolume("Volume_UI", uiVolume);
    }

    /// <summary>
    /// ����� ������ � ����������� �� �������� �����.
    /// ���� ����� ���������� �� ������������ ������ ����� ��� ��������.
    /// </summary>
    public void ChangeMusicForScene(string sceneName)
    {
        AudioClip newMusic = null;

        switch (sceneName)
        {
            case "MainMenu":
                newMusic = Resources.Load<AudioClip>("Music/MainMenuMusic");
                break;
            case "StartDay":
                newMusic = Resources.Load<AudioClip>("Music/StartDayMusic");
                break;
            case "MainRoom":
                newMusic = Resources.Load<AudioClip>("Music/MainRoomMusic");
                break;
            case "Forge":
                newMusic = Resources.Load<AudioClip>("Music/ForgeMusic");
                break;
            case "Storage":
                newMusic = Resources.Load<AudioClip>("Music/StorageMusic");
                break;
        }

        if (newMusic != null)
        {
            PlayMusic(newMusic);
        }
    }

    /// <summary>
    /// ����������� mixer �� snapshot ��� ������������ ������, ��� ����� ����� ���������.
    /// </summary>
    public void ActivateLoadingSnapshot()
    {
        // ������������� ������������� ��������� ��� SFX � UI �� -80 �� ���������� �� ���������������� ��������
        audioMixer.SetFloat("Volume_SFX", -80f);
        audioMixer.SetFloat("Volume_UI", -80f);

        // ����� ��������� �� snapshot ������������ ������
        if (loadingSnapshot != null)
        {
            loadingSnapshot.TransitionTo(snapshotTransitionTime);
        }
    }

    /// <summary>
    /// ���������� mixer � ���������� ���������.
    /// </summary>
    public void ActivateNormalSnapshot()
    {
        // ��������� ��������� ����� �� PlayerPrefs
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        float uiVolume = PlayerPrefs.GetFloat("UIVolume", 1f);
        SetVolume("Volume_SFX", sfxVolume);
        SetVolume("Volume_UI", uiVolume);

        if (normalSnapshot != null)
        {
            normalSnapshot.TransitionTo(snapshotTransitionTime);
        }
    }

    public void StartMiniGameMusic()
    {
        if (_isInMiniGame || miniGameMusic == null) return;

        _previousMusic = musicSource.clip;  // ���������� ������� ����
        _isInMiniGame = true;
        PlayMusic(miniGameMusic);
    }

    public void StopMiniGameMusic()
    {
        if (!_isInMiniGame) return;

        _isInMiniGame = false;
        // ���� ���������� ���� ���, ����� ���; ����� ������ �� ������
        if (_previousMusic != null)
            PlayMusic(_previousMusic);
    }



    public void ReloadVolumeSettings()
    {
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        float uiVolume = PlayerPrefs.GetFloat("UIVolume", 1f);

        SetVolume("Volume_Music", musicVolume);
        SetVolume("Volume_SFX", sfxVolume);
        SetVolume("Volume_UI", uiVolume);
    }

}
