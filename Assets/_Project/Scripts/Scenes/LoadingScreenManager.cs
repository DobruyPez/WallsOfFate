using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class LoadingScreenManager : MonoBehaviour
{
    public static LoadingScreenManager Instance; // �������� ��� ������� �� ����� ����

    [Header("UI �������� ������������ ������")]
    public GameObject loadingScreen;   // ��������� (��������, ������ ��� Canvas) ������������ ������
    public Image loadingImage;         // �����������, ������� ������������ �� ����� ��������
    public TMP_Text loadingText;       // ��������� ������� (TextMeshPro) ��� ������ ���������

    [Header("��������� ���������� ����")]
    [Tooltip("������, ������� ������������� � loadingImage ����� �������� ����� �����.")]
    public Sprite finalSprite;         // ��������� ������

    // ����, �����������, ��� ���� ��� ������� �������
    private bool waitingForUserInput = false;

    private void Awake()
    {
        // ��������� ��������, ����� ������ �� ����������� ����� �������
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
    /// ��������� ����������� �������� ��������� �����.
    /// ����������� ����� ���������� �����, � ����� �������� ����� ����� ���� ������� �������,
    /// ���� ������������ �� ����� ����� �������.
    /// </summary>
    /// <param name="sceneName">��� ����� ��� ��������</param>
    public void LoadScene(string sceneName)
    {
        // ���������� ����������� ����� � ����������� ���������� "��������..."
        loadingScreen.SetActive(true);
        loadingText.text = "��������...";

        // ������������� �� ������� ���������� �������� �����
        SceneManager.sceneLoaded += OnSceneLoaded;

        // ��������� ����������� �������� ����� �����.
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    /// <summary>
    /// ���������� ��������� ��������� �����.
    /// allowSceneActivation ��������� true, ����� ����� ����� �������������� ����� ����� ��������.
    /// </summary>
    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        // ����� �������� ������������ ���������, ���� ���������.
        while (!operation.isDone)
        {
            yield return null;
        }
    }

    /// <summary>
    /// �����, ���������� �������� ����� �������� ����� �����.
    /// ����� ���������� ����� ��������� ��������� ������������ ������,
    /// � ���������� �������� ������� ����� ������� ��� �������� ����.
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // ������������, ����� �������� ������������� �������
        SceneManager.sceneLoaded -= OnSceneLoaded;

        // ���� �� ������� loadingImage ���� ��������� UISpriteAnimator, ��������� ���.
        UISpriteAnimator spriteAnimator = loadingImage.GetComponent<UISpriteAnimator>();
        if (spriteAnimator != null)
        {
            spriteAnimator.enabled = false;
        }

        // ����������� ��������� ������, ���� �� �����.
        if (finalSprite != null)
        {
            loadingImage.sprite = finalSprite;
        }

        // ������ ����� �� ��������� ���������.
        loadingText.text = "����������";
        waitingForUserInput = true;

        // ��������� �������� ��� �������� ����������������� ����� �, �����������, ������� ������� ������.
        StartCoroutine(WaitForUserInput());
    }

    /// <summary>
    /// ��������, ������� ���� ������� ����� �������.
    /// ���� �� ������ ����� �������, ����� �������� �������������� ������� (��������, ������� ������).
    /// </summary>
    private IEnumerator WaitForUserInput()
    {
        // ���� ������ �������� ������� ������, ����� ��������� ��������� ��������.
        StartCoroutine(BlinkText());

        // ����, ���� ������������ �� ������ ����� �������.
        while (!Input.anyKeyDown)
        {
            yield return null;
        }

        waitingForUserInput = false;
        // ����� ������� ������� �������� ����������� �����.
        loadingScreen.SetActive(false);
    }

    /// <summary>
    /// ������ �������� ������� ������� ������.
    /// ����� ����� ������������� ������ 0.5 �������, ���� ���� �����.
    /// </summary>
    private IEnumerator BlinkText()
    {
        while (waitingForUserInput)
        {
            loadingText.enabled = !loadingText.enabled;
            yield return new WaitForSeconds(0.5f);
        }
        // � ����� ����������� �������� �����
        loadingText.enabled = true;
    }
}
