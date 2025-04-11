using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class LoadingScreenManager : MonoBehaviour
{
    public static LoadingScreenManager Instance; // ��������

    [Header("UI �������� ������������ ������")]
    public GameObject loadingScreen;
    public TMP_Text loadingText;

    // ����� �������� ����� ���, ��� ����� ������ ������� ����� �������� (� ��������)
    public float inputDelay = 0.05f;

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
        // ��������������� ���������� ������� �������
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
                // ��������� �������� ����� ���������� ����������� ��������
                yield return new WaitForSeconds(inputDelay);
                loadingText.text = "����������";
                waitingForInput = true;

                // ������ �������: �������� ����� ������������ � ������� ��������� �����-������ ������
                StartCoroutine(WaitForUserInput());
                StartCoroutine(FadeText());
                break;
            }
            yield return null;
        }
    }

    private IEnumerator WaitForUserInput()
    {
        
        // ����� �������� �������� ������� ������� ����� �������
        while (!Input.anyKeyDown)
        {
            yield return null;
        }
        waitingForInput = false;

        // ��������������� ����� �������� ���������������� ����������:
        AudioManager.Instance.ReloadVolumeSettings();
        // ����������� ������ �� ������ ������� �����
        AudioManager.Instance.ChangeMusicForScene(targetSceneName);
        // �������� ����������� �����
        loadingScreen.SetActive(false);
    }

    // ����� ����� ��� �������� ��������� �����-������ ������
    private IEnumerator FadeText()
    {
        // ������ ������� ��������� ������������ (��� ������ ��������, ��� ������� �������� �����)
        float frequency = 2f;

        while (waitingForInput)
        {
            // ��������� ����� �������� ����� � ������� ���������,
            // ������� ����� ������ ���������� �� 0 �� 1
            float alpha = (Mathf.Sin(Time.time * frequency) + 1f) / 2f;
            Color color = loadingText.color;
            color.a = alpha;
            loadingText.color = color;
            yield return null;
        }
        // ����� ������ �� ����� ���������� ����� � ��������� ������������ ���������
        Color finalColor = loadingText.color;
        finalColor.a = 1f;
        loadingText.color = finalColor;
    }
}
