using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using System.Collections;
using UnityEngine.UI;

public class LoadingScreenManager : MonoBehaviour
{
    public static LoadingScreenManager Instance;

    /* ---------- ����� ---------- */
    public bool IsLoading { get; private set; }
    public event Action LoadingStarted;
    public event Action LoadingFinished;
    /* --------------------------- */

    [Header("UI")]
    public GameObject loadingScreen;
    public Image loadingImage;
    public TMP_Text loadingText;

    private UISpriteAnimator spriteAnimator;

    [Header("��������� ���������� ����")]
    public Sprite finalSprite;

    public float inputDelay = 0.05f;

    private string targetSceneName;
    private bool waitingForInput;

    /* ---------- SINGLETON ---------- */
    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
        spriteAnimator = loadingImage.GetComponent<UISpriteAnimator>();
    }

    /* ---------- ������ �������� ---------- */
    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1f;

        targetSceneName = sceneName;
        loadingScreen.SetActive(true);
        loadingText.text = "��������...";

        /* --- ���������� ��������� ��� --- */
        IsLoading = true;
        LoadingStarted?.Invoke();
        /* -------------------------------- */

        AudioManager.Instance.ActivateLoadingSnapshot();
        AudioManager.Instance.PlayLoadingMusic();

        StartCoroutine(LoadSceneAsync(sceneName));
    }

    /* ---------- �������� �������� ---------- */
    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);

        while (!op.isDone)
        {
            if (op.progress >= 0.9f)
            {
           
                yield return new WaitForSeconds(inputDelay);

                
                if (spriteAnimator != null)
                {
                    spriteAnimator.enabled = false;
                }
                // ����������� ��������� ������, ���� �� �����.
                if (finalSprite != null)
                {
                    loadingImage.sprite = finalSprite;
                }

                loadingText.text = "����������";
                waitingForInput = true;

                StartCoroutine(WaitForUserInput());
                StartCoroutine(FadeText());
                break;
            }
            yield return null;
        }
    }

    /* ---------- ������� ���� ������������ ---------- */
    private IEnumerator WaitForUserInput()
    {
        while (!Input.anyKeyDown) yield return null;

        waitingForInput = false;

        AudioManager.Instance.ReloadVolumeSettings();
        AudioManager.Instance.ChangeMusicForScene(targetSceneName);

        spriteAnimator.enabled = true;

        loadingScreen.SetActive(false);

        /* --- �������� ��������� ------------- */
        IsLoading = false;
        LoadingFinished?.Invoke();
        /* ------------------------------------ */
    }

    /* ---------- ������������ ����� ---------- */
    private IEnumerator FadeText()
    {
        float frequency = 2f;

        while (waitingForInput)
        {
            float a = (Mathf.Sin(Time.time * frequency) + 1f) / 2f;
            Color c = loadingText.color; c.a = a;
            loadingText.color = c;
            yield return null;
        }

        Color fin = loadingText.color; fin.a = 1f;
        loadingText.color = fin;
    }
}
