using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private Slider sliderMusic;
    [SerializeField] private Slider sliderSFX;
    [SerializeField] private Slider sliderUI;

    private void Start()
    {
        // ��������� ���������� ���������
        sliderMusic.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sliderSFX.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
        sliderUI.value = PlayerPrefs.GetFloat("UIVolume", 1f);

        ApplySettings();
    }

    public void ApplySettings()
{
    // ��������� ��������� ���������
    AudioManager.Instance.SetVolume("Volume_Music", sliderMusic.value);
    AudioManager.Instance.SetVolume("Volume_SFX", sliderSFX.value);
    AudioManager.Instance.SetVolume("Volume_UI", sliderUI.value);

    // ��������� ���������
    PlayerPrefs.SetFloat("MusicVolume", sliderMusic.value);
    PlayerPrefs.SetFloat("SFXVolume", sliderSFX.value);
    PlayerPrefs.SetFloat("UIVolume", sliderUI.value);
    PlayerPrefs.Save();
}
}
