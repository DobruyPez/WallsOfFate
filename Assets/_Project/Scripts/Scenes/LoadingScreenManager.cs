using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class LoadingScreenManager : MonoBehaviour
{
    public static LoadingScreenManager Instance; // ��������

    [Header("UI �������� ������������ ������")]
    public GameObject loadingScreen;
    public Image loadingImage;
    public TMP_Text loadingText;

    [Header("��������� ���������� ����")]
    public Sprite finalSprite;

    private string targetSceneName;
    private bool waitingForInput = false;

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
        }
    }

    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1f;

        targetSceneName = sceneName;

        loadingScreen.SetActive(true);
        loadingText.text = "��������...";

        // ����������� ������ �� ����� �������� (��������� ����� �����)
        AudioManager.Instance.ActivateLoadingSnapshot();

        // ��������� ����������� ������ (���� �����)
        AudioManager.Instance.PlayLoadingMusic();

        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        while (!operation.isDone)
        {
            if (operation.progress >= 0.9f)
            {
                var spriteAnimator = loadingImage.GetComponent<UISpriteAnimator>();
                if (spriteAnimator != null)
                {
                    spriteAnimator.enabled = false;
                }
                if (finalSprite != null)
                {
                    loadingImage.sprite = finalSprite;
                }
                loadingText.text = "����������";
                waitingForInput = true;
                StartCoroutine(WaitForUserInput());
                break;
            }
            yield return null;
        }
    }

    private IEnumerator WaitForUserInput()
    {
        StartCoroutine(BlinkText());
        while (!Input.anyKeyDown)
        {
            yield return null;
        }
        waitingForInput = false;

        // ��������������� ����� �������� ���������������� ����������:
        AudioManager.Instance.ReloadVolumeSettings();
        // ���, ���� ������������ ActivateNormalSnapshot():
        // AudioManager.GetInstance().ActivateNormalSnapshot();

        // ����������� ������ �� ������ ������� �����
        AudioManager.Instance.ChangeMusicForScene(targetSceneName);
        // �������� ����������� �����
        loadingScreen.SetActive(false);
    }


    private IEnumerator BlinkText()
    {
        while (waitingForInput)
        {
            loadingText.enabled = !loadingText.enabled;
            yield return new WaitForSeconds(0.5f);
        }
        loadingText.enabled = true;
    }
}
