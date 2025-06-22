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

    public GameObject panelGameOver;   // ����� ���������
    public GameObject panelVictory;


    private InventoryLogicEnd _inventoryLogicEnd;

    private bool _startupIntroShown = false;

    private Coroutine _fadeCoroutine;


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

    [Header("Intro Screen (New Game)")]
    public GameObject panelNewGameIntro;      // ��� ����� ����� ����� ��� ������
    //public float introDuration = 10f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); }

        _inventoryLogicEnd = FindObjectOfType<InventoryLogicEnd>();
        if (_inventoryLogicEnd == null)
            Debug.LogWarning("LoadingScreenManager: �� ������� ����� InventoryLogicEnd �� �����!");

        spriteAnimator = loadingImage.GetComponent<UISpriteAnimator>();
    }

    private void Start()
    {
        // ������ ���� ��� ��� ������ ������
        if (!_startupIntroShown && panelNewGameIntro != null)
        {
            _startupIntroShown = true;
            StartCoroutine(ShowStartupIntro());
        }
    }

    private void Update()
    {
        // ���� ��� ��� �������� ��� ��������� ����� ������ � ������ �� ������
        if (IsLoading || panelGameOver.activeSelf || panelVictory.activeSelf) return;

        if (IsAnyResourceZero())
        {
            ShowGameOver();
        }
        else if (IsAnyResourceHundrid())
        {
            ShowWin();
        }
    }

    private bool IsAnyResourceZero()
    {
        return GameResources.GameResources.Gold <= 0 ||
                GameResources.GameResources.Food <= 0 ||
                GameResources.GameResources.PeopleSatisfaction <= 0 ||
                GameResources.GameResources.CastleStrength <= 0;
    }
    private bool IsAnyResourceHundrid()
    {
        return GameResources.GameResources.Gold >= 1000 ||
                GameResources.GameResources.Food >= 1000 ||
                GameResources.GameResources.PeopleSatisfaction >= 1000 ||
                GameResources.GameResources.CastleStrength >= 1000;
    }

    private void ShowGameOver()
    {
        Time.timeScale = 0f;               // ������ ���� �� �����
        Transform[] children = panelGameOver.GetComponentsInChildren<Transform>(true);

        // ��������� �������� ��������
        UpdateResourceText(children, "Gold", GameResources.GameResources.Gold.ToString());
        UpdateResourceText(children, "Food", GameResources.GameResources.Food.ToString());
        UpdateResourceText(children, "Satisfaction", GameResources.GameResources.PeopleSatisfaction.ToString());
        UpdateResourceText(children, "Staraight", GameResources.GameResources.CastleStrength.ToString());
        panelGameOver.SetActive(true);
    }
    private void ShowWin()
    {
        Time.timeScale = 0f;               // ������ ���� �� �����
        Transform[] children = panelVictory.GetComponentsInChildren<Transform>(true);

        // ��������� �������� ��������
        UpdateResourceText(children, "Gold", GameResources.GameResources.Gold.ToString());
        UpdateResourceText(children, "Food", GameResources.GameResources.Food.ToString());
        UpdateResourceText(children, "Satisfaction", GameResources.GameResources.PeopleSatisfaction.ToString());
        UpdateResourceText(children, "Staraight", GameResources.GameResources.CastleStrength.ToString());
        panelVictory.SetActive(true);
    }

    // === ������ ������ ���� � ����� UI ������ �������� ���� ����� ===
    public void ShowEndOfDayPanel()
    {
        // ���������� ����� ����� ���
        panelEndOfDay.SetActive(true);

        // ������� ��� �������� ��������
        Transform[] children = panelEndOfDay.GetComponentsInChildren<Transform>(true);

        // ��������� �������� ��������
        UpdateResourceText(children, "Gold", GameResources.GameResources.Gold.ToString());
        UpdateResourceText(children, "Food", GameResources.GameResources.Food.ToString());
        UpdateResourceText(children, "Satisfaction", GameResources.GameResources.PeopleSatisfaction.ToString());
        UpdateResourceText(children, "Staraight", GameResources.GameResources.CastleStrength.ToString());
    }

    private void UpdateResourceText(Transform[] children, string parentObjectName, string value)
    {
        // ���� ������ ������������ ������ � ��������� ������
        Transform parentObject = System.Array.Find(children, child => child.name == parentObjectName);

        if (parentObject != null)
        {
            // ���� �������� ������, ��� �������� ���������� � "Amount%"
            Transform amountChild = null;
            foreach (Transform child in parentObject)
            {
                if (child.name.StartsWith("Amount"))
                {
                    amountChild = child;
                    break;
                }
            }

            if (amountChild != null)
            {
                // �������� ��������� TextMeshProUGUI
                TextMeshProUGUI textComponent = amountChild.GetComponent<TextMeshProUGUI>();

                if (textComponent != null)
                {
                    textComponent.text = value;
                }
                else
                {
                    Debug.LogWarning($"������ Amount% � {parentObjectName} �� �������� ��������� TextMeshProUGUI");
                }
            }
            else
            {
                Debug.LogWarning($"�� ������ �������� ������ Amount% � {parentObjectName}");
            }
        }
        else
        {
            Debug.LogWarning($"�� ������ ������������ ������ � ������ {parentObjectName}");
        }
    }

    // === ������ ������������ ����� ���� �� panelEndOfDay ===
    public void OnConfirmEndOfDay()
    {
        PlayerSpawnData.ClearData();

        if (_inventoryLogicEnd != null)
            _inventoryLogicEnd.RefreshPanel();

        QuestCollection.IncreaseCurrentDay();   // ���� +1

        /* --- ��������� ������ --- */
        if (QuestCollection.CurrentDayNumber > 3)
        {
            StartCoroutine(ShowVictoryAfterLoad());
            return;                             // ��������� ������� ���� ��������
        }

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

    private IEnumerator ShowVictoryAfterLoad()
    {
        // ���������� ������� �������, ����� ��� ������ ����
        BeginLoadWithStartOfDay("StartDay");

        // ��� ���� �� ����������
        while (IsLoading) yield return null;

        Time.timeScale = 0f;
        Transform[] children = panelVictory.GetComponentsInChildren<Transform>(true);

        // ��������� �������� ��������
        UpdateResourceText(children, "Gold", GameResources.GameResources.Gold.ToString());
        UpdateResourceText(children, "Food", GameResources.GameResources.Food.ToString());
        UpdateResourceText(children, "Satisfaction", GameResources.GameResources.PeopleSatisfaction.ToString());
        UpdateResourceText(children, "Staraight", GameResources.GameResources.CastleStrength.ToString());
        panelVictory.SetActive(true);
    }

    private void ShowLoadingUI()
    {
        Time.timeScale = 1f;
        loadingScreen.SetActive(true);
        panelEndOfDay.SetActive(false);

        // �������� ��������� ������
        loadingText.text = "��������...";

        if (spriteAnimator != null) spriteAnimator.enabled = true;
    }

    private void StartTextFade()
    {
        // ���� ��� ������� � �������������
        if (_fadeCoroutine != null)
            StopCoroutine(_fadeCoroutine);
        _fadeCoroutine = StartCoroutine(FadeText());
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

                // ������ �����, �� ��������� ��� ��������
                loadingText.text = "����������";
                StartTextFade();
                waitingForInput = true;

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

        if (_fadeCoroutine != null)
            StopCoroutine(_fadeCoroutine);
        var c = loadingText.color;
        loadingText.color = new Color(c.r, c.g, c.b, 1f);

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

    private IEnumerator ShowStartupIntro()
    {
        // ��������� �� ���������
        Time.timeScale = 0f;
        panelNewGameIntro.SetActive(true);

        float timer = 0f;
        // ��� ���� introDuration, ���� ������� �������
        while (/*timer < introDuration*/true)
        {
            if (Input.anyKeyDown)
                break;
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        panelNewGameIntro.SetActive(false);
        Time.timeScale = 1f;
    }

    private IEnumerator FadeText()
    {
        float freq = 4f;                  // �������� ���������
        Color baseColor = loadingText.color;
        while (true)
        {
            // ���������: [0�1]
            float a = (Mathf.Sin(Time.time * freq) + 1f) / 2f;
            loadingText.color = new Color(baseColor.r, baseColor.g, baseColor.b, a);
            yield return null;
        }
    }

    private IEnumerator ShowStartDay()
    {
        panelStartOfDay.SetActive(true);
        yield return new WaitForSeconds(startDayDuration);
        panelStartOfDay.SetActive(false);
    }
}
