using GameResources;
using Quest;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
using UnityEngine.UI;

public sealed class SaveLoadManager : MonoBehaviour
{
    private static Transform _playerTransform;

    // ������ ����� ����������� (������������ ��� �������� ���������� ����)
    private ISaveLoader[] saveLoaders;
    // ����� �����������, ����������� ��� ������������� ��� �������� ������� ������.
    private ISaveLoader[] requiredSaveLoaders;

    // ���� ��� ����������� ������ ����� ����
    private bool _startNewGame = false;

    [SerializeField] private string firstGameplayScene = "StartDay";

    // ������ ��������� ����� ����� ���������
    [SerializeField] private Transform spawnPoint;

    [Inject]
    private void Construct(PlayerMoveController controller)
    {
        _playerTransform = controller.transform;
    }

    private void Awake()
    {
        // ������ ����� ����������� � ������������, ��������, ��� ����������� ����.
        saveLoaders = new ISaveLoader[]
        {
            new QuestSaveLoader(),
            new ResourceSaveLoader(),
            new PlayerSaveLoader(_playerTransform),
            new CollectionSaveLoader(AssembledPickups.GetAllPickups()),
        };

        // ����� �����������, ������� �� ������ �������������� ������� ������.
        // ������� PlayerSaveLoader, ����� ������� ������ ���������� �����, ����� ��� ���������� �� �����.
        requiredSaveLoaders = new ISaveLoader[]
        {
            new QuestSaveLoader(),
            new ResourceSaveLoader(),
            // new PlayerSaveLoader(_playerTransform), // �� ��������� ������� ������
            new CollectionSaveLoader(AssembledPickups.GetAllPickups()),
            new InteractableItemSaveLoader()
        };

        // ���� ��� �� ����� ����, ��������� ����������� ������.
        // (��� ����� ���� ClearSavs() ������� _startNewGame = true, � ������� �� ����� ������������)
        if (!_startNewGame)
        {
            LoadRequiredData();
        }
    }

    private void Start()
    {
        // ���� ������ � ����� ��� �� ������ (�������� �� FindObject)
        Button loadBtn = GameObject.Find("Button_LoadGame")
                                   ?.GetComponent<Button>();
        if (loadBtn != null)
            loadBtn.interactable = CanLoad();   // �������, ������ ���� ���� ����
    }

    /// <summary>
    /// ������ �������� ����������� ��������� (� ��� ����� ������� ������).
    /// ����������, ��������, ��� ������ "����������".
    /// </summary>
    public void LoadGame()
    {
        Repository.LoadState();
        foreach (var saveLoader in saveLoaders)
        {
            saveLoader.LoadData();
        }
    }

    /// <summary>
    /// �������� ������������ ������, ��� �������� ������� ������.
    /// ���� ��� ������-���� ���������� ��� ���������� ������, ���������� LoadDefaultData().
    /// </summary>
    public void LoadRequiredData()
    {
        Repository.LoadState();
        foreach (var saveLoader in requiredSaveLoaders)
        {
            if (!saveLoader.LoadData())
            {
                saveLoader.LoadDefaultData();
            }
        }
    }

    /// <summary>
    /// ���������� ������������ ������ (��������, ��� ����� ����).
    /// </summary>
    public void SaveRequiredData()
    {
        if (!_startNewGame)
        {
            foreach (var saveLoader in requiredSaveLoaders)
            {
                if (saveLoader != null)
                    saveLoader.SaveData();
            }
            Repository.SaveState();
            _startNewGame = false;
        }
    }

    /// <summary>
    /// ������ ���������� �������� ��������� (� ��� ����� ������� ������).
    /// ����������, ��������, ��� ���������� ����� ������� ��� ��� �������� � ���� "����������".
    /// </summary>
    public void SaveGame()
    {
        foreach (var saveLoader in saveLoaders)
        {
            saveLoader.SaveData();
        }
        Repository.SetUserProgress(true);
        Repository.SaveState();
    }

    /// <summary>
    /// ���������, ���� �� ���������� ������� ��������.
    /// </summary>
    public bool CanLoad()
    {
        Repository.LoadState();
        return Repository.HasAnyData();
    }

    /// <summary>
    /// ������� ���������� ������ � ������������� ���� ����� ����.
    /// </summary>
    public void ClearSavs()
    {
        QuestCollection.ClearQuests();
        AssembledPickups.Clear();
        Repository.ClearSaveData();
        PlayerSpawnData.ClearData();
        _startNewGame = true;
    }

    private void OnEnable()
    {
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SaveRequiredData();
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneUnloaded(Scene scene)
    {
        SaveRequiredData();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // ���� ���������� ����� ����, ������� ������ ������������ � spawnPoint.
        if (_startNewGame)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null && spawnPoint != null)
            {
                player.transform.position = spawnPoint.position;
                // ���� ����� �������� � ��������, ����� ��������:
                // player.transform.rotation = spawnPoint.rotation;
            }
            // ����� ��������� ������������ ������ (��� ������� ������)
            LoadRequiredData();
            _startNewGame = false; // �������� ����, ����� � ���������� ������� �������� ���������� ��������
        }
        else
        {
            LoadRequiredData();
        }
    }

    public void OnNewGameButton()
    {
        ClearSavs();                            // �������� ��
        _startNewGame = true;                   // �������� OnSceneLoaded
        LoadingScreenManager.Instance.BeginLoadWithStartOfDay(firstGameplayScene);
    }

    /*-------------------------------------------------*/
    /*      ������ ���������� ���ӻ                    */
    /*-------------------------------------------------*/
    public void OnLoadGameButton()
    {
        // safety-check: ������ �� ������, ���� ���������� ���
        if (!CanLoad()) return;

        _startNewGame = false;                  // ����� �� ������������ �������
        LoadGame();                             // ���������� ������
    }

}
