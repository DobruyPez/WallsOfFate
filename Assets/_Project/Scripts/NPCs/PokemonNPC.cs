using Assets._Project.Scripts.TriggerOjects;
using System.Collections;
using UnityEngine;

public class PokemonNPC : MonoBehaviour
{
    [SerializeField] private GameObject _miniGame;
    [SerializeField] private GameObject _castle;
    [SerializeField] private GameObject _uiPannel;
    [SerializeField] private CameraSwitch _switch;
    [SerializeField] private GameProcess _miniGameProcessor;
    [SerializeField] private GameObject _menu;

    private GameObject _winPanel;
    private GameObject _losePanel;

    private bool _isPanelActive = false;

    private bool _isMiniGameActive = false; // �������� �� ����� ����-����
    private bool _isMiniGameFinished = false; // �������� �� ����� ����-����
    private bool _isWaitingForMiniGame = false; // ���� �������� ����� ������� ����-����

    private void Start()
    {
        _switch = GetComponentInChildren<CameraSwitch>();
        _miniGameProcessor.OnEndGame += EndMiniGame;
    }

    private void Update()
    {
        if (_isMiniGameActive || _isMiniGameFinished || _isWaitingForMiniGame)
            return; // �� ��������� ����-���� ��������

        DialogueManager dialogeManager = DialogueManager.GetInstance();

        bool powerCheckStart = ((Ink.Runtime.BoolValue)dialogeManager.GetVariablesState("PowerCheckStart")).value;

        if (!dialogeManager.DialogueIsPlaying)
        {
            if (powerCheckStart)
            {
                StartMiniGame();
            }
            if (_menu != null)
            {
                _winPanel = _menu.transform.Find("WinPanel")?.gameObject;
                _losePanel = _menu.transform.Find("LosePanel")?.gameObject;

                if (_winPanel == null || _losePanel == null)
                {
                    Debug.LogWarning("������ WinPanel ��� LosePanel �� ������� � ������� _menu.");
                }
                else
                {
                    // ������������ ������ ��� ������
                    _winPanel.SetActive(false);
                    _losePanel.SetActive(false);
                }
            }
            else
            {
                Debug.LogWarning("������ _menu �� ��������.");
            }

        }
    }

    private void ActivatePanel(GameObject panel)
    {
        if (panel != null)
        {
            panel.SetActive(!panel.activeSelf); 
        }
    }


    private void StartMiniGame()
    {
        _switch.SwitchCamera();
        _uiPannel.SetActive(false);
        _miniGame.SetActive(true);
        _castle.SetActive(false);
        _isMiniGameActive = true;
    }

    public void EndMiniGame(string winnerName, string loserName)
    {
        if (_isMiniGameFinished) return; // �� �������� ��������

        // ��������� �������� ��� ��������� ����� ����
        StartCoroutine(EndMiniGameCoroutine(winnerName, loserName));
    }

    private IEnumerator EndMiniGameCoroutine(string winnerName, string loserName)
    {
        _miniGame.SetActive(false);

        GameObject currentPannel = null;
        // ���������� ��������������� ������
        if (winnerName == "Player")
        {
            currentPannel = _winPanel;
        }
        else
        {
            currentPannel = _losePanel;
        }
        
        ActivatePanel(currentPannel);
        yield return new WaitForSeconds(2f);
        ActivatePanel(currentPannel);


        // ��������� ���������� ������ ����� ����
        _switch.SwitchCamera();
        _uiPannel.SetActive(true);
        _castle.SetActive(true);
        _isMiniGameFinished = true;
        DataBetweenLocations.ForgePerfom = true;


        Debug.LogWarning("����-���� ���������. ����������: " + winnerName);
    }

}
