using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LoadingScreenManager : MonoBehaviour
{
    public static LoadingScreenManager Instance; // �������� ��� ������� �� ����� �����

    public GameObject loadingScreen; // ������ � UI ������������ ������ (��������, Canvas � ���������)
    public Slider progressBar;       // UI-������� ��� ����������� ���������

    private void Awake()
    {
        // ���� ���������� ��� ���, ������������� ��� � �� ���������� ��� �������� ����� �����
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

    /// <summary>
    /// ��������� ����� ��� �������� ����� ����� � ������������ ������������ ������
    /// </summary>
    /// <param name="sceneName">��� �����, ������� ����� ���������</param>
    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    /// <summary>
    /// ����������� �������� ����� � ����������� UI ��������
    /// </summary>
    private IEnumerator LoadSceneAsync(string sceneName)
    {
        // ���������� ����������� �����
        loadingScreen.SetActive(true);

        // �������� ����������� �������� �����
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        // ���� �����, ����� ��������� �������������� ��������� �����:
        // operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            // AsyncOperation.progress ���������� �������� �� 0 �� 0.9,
            // ������� ����������� ��� �� ��������� 0-1
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            progressBar.value = progress;

            // ���� �� ����������� allowSceneActivation = false,
            // ����� �������� ��������, ����� �������� ����� ���������
            // if (operation.progress >= 0.9f)
            // {
            //     // ����� �����, ��������, ������� ������� ������ "������ ����" � �����:
            //     // operation.allowSceneActivation = true;
            // }

            yield return null;
        }

        // ������������ ����������� ����� ����� ���������� ��������
        loadingScreen.SetActive(false);
    }
}
