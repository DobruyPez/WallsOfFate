using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using System.Collections;
using UnityEngine.UI;
using Quest;

public class LoadingScreenManager : MonoBehaviour
{
    public static LoadingScreenManager Instance;

    public bool IsLoading { get; private set; }
    public event Action LoadingStarted;
    public event Action LoadingFinished;


    [Header("UI-������")]
    public GameObject loadingScreen;      // ���� ������������ ������ ��������
    public TMP_Text loadingText;
    public Image loadingImage;            // ��� �������� ��� ���������� �������

    public GameObject panelEndOfDay;      // �����: ����� ������������ ����� ����
    public GameObject panelStartOfDay;    // �����: ����� ������� ����
    public float startDayDuration = 2f;   // ������� ��� ���������� ������ ���
    public float inputDelay = 0.05f;      // ����� ����� ���, ��� ������� ������ Continue

    public Sprite finalSprite;            // ��, ��� �������� �������� ����� Continue

    private string targetSceneName;
    private bool waitingForInput;
    private UISpriteAnimator spriteAnimator;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); }

        spriteAnimator = loadingImage.GetComponent<UISpriteAnimator>();
    }

    // === ������ ������ ���� � ����� UI ������ �������� ���� ����� ===
    public void ShowEndOfDayPanel()
    {
        // ������ ���������� ����� ����� ���
        panelEndOfDay.SetActive(true);
    }

    // === ������ ������������ ����� ���� �� panelEndOfDay ===
    public void OnConfirmEndOfDay()
    {
        QuestCollection.IncreaseCurrentDay();
        panelEndOfDay.SetActive(false);
        BeginLoadWithStartOfDay("StartDay");
    }

    // === ������ ������� �� panelEndOfDay ===
    public void OnCancelEndOfDay()
    {
        panelEndOfDay.SetActive(false);
    }

    // ����� ������ �������� (� ������� start-day �����)
    public void BeginLoadWithStartOfDay(string sceneName)
    {
        targetSceneName = sceneName;
        ShowLoadingUI();

        // ����������
        IsLoading = true;
        LoadingStarted?.Invoke();

        AudioManager.Instance.ActivateLoadingSnapshot();
        AudioManager.Instance.PlayLoadingMusic();

        StartCoroutine(LoadSceneAsync(sceneName, showStartDay: true));
    }

    // === ��� ������������ ����� LoadScene, �� ��� end-day ===
    public void LoadScene(string sceneName)
    {
        // ���� ��� ����� ������ ����� �������� ��� start-day
        targetSceneName = sceneName;
        ShowLoadingUI();

        IsLoading = true;
        LoadingStarted?.Invoke();

        AudioManager.Instance.ActivateLoadingSnapshot();
        AudioManager.Instance.PlayLoadingMusic();

        StartCoroutine(LoadSceneAsync(sceneName, showStartDay: false));
    }

    private void ShowLoadingUI()
    {
        Time.timeScale = 1f;
        loadingScreen.SetActive(true);
        panelEndOfDay.SetActive(false);
        // �� �������� panelStartOfDay �����!
        loadingText.text = "��������...";
        if (spriteAnimator != null) spriteAnimator.enabled = true;
    }

    private IEnumerator LoadSceneAsync(string sceneName, bool showStartDay)
    {
        var op = SceneManager.LoadSceneAsync(sceneName);
        while (!op.isDone)
        {
            if (op.progress >= 0.9f)
            {
                yield return new WaitForSeconds(inputDelay);

                if (spriteAnimator != null) spriteAnimator.enabled = false;
                if (finalSprite != null) loadingImage.sprite = finalSprite;
                loadingText.text = "����������";
                waitingForInput = true;

                // ����� ��� �� ������� ������� panelStartOfDay
                yield return StartCoroutine(WaitForUserInput(showStartDay));
                yield break;
            }
            yield return null;
        }
    }


    private IEnumerator WaitForUserInput(bool showStartDay)
    {
        // ��� �������
        while (!Input.anyKeyDown) yield return null;
        waitingForInput = false;

        // ���������� ����� � ��������
        AudioManager.Instance.ReloadVolumeSettings();
        AudioManager.Instance.ChangeMusicForScene(targetSceneName);
        if (spriteAnimator != null) spriteAnimator.enabled = true;

        // ����� ������ ������ ��� ��� ����� ��������
        if (showStartDay && panelStartOfDay != null)
        {
            panelStartOfDay.SetActive(true);
            // ����� loadingScreen ������� ��������, ����� ��������� ������ ��������
            yield return new WaitForSeconds(startDayDuration);
            panelStartOfDay.SetActive(false);
        }

        // ������ ����� ����� ��������� ��� loadingScreen
        loadingScreen.SetActive(false);

        // � ���� ������ � ������� �������� �����������
        IsLoading = false;
        LoadingFinished?.Invoke();
    }



    private IEnumerator FadeText()
    {
        float freq = 2f;
        while (waitingForInput)
        {
            float a = (Mathf.Sin(Time.time * freq) + 1f) / 2f;
            var c = loadingText.color; c.a = a;
            loadingText.color = c;
            yield return null;
        }
        var f = loadingText.color; f.a = 1f;
        loadingText.color = f;
    }

    private IEnumerator ShowStartDay()
    {
        panelStartOfDay.SetActive(true);
        yield return new WaitForSeconds(startDayDuration);
        panelStartOfDay.SetActive(false);
    }
}
