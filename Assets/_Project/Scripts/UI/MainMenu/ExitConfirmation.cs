using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExitConfirmation : MonoBehaviour
{
    [SerializeField] private GameObject exitPanel;
    [SerializeField] private Button confirmButton;  // ������ ������������� ������

    private void Start()
    {
        exitPanel.SetActive(false);
    }

    private void Update()
    {
        // ��������� ������ �� Esc, ���� ��� ��� �������
        if (exitPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            HideExitPanel();
        }
    }

    public void ShowExitPanel()
    {
        exitPanel.SetActive(true);

        // ������� ������� ���������, ����� �������� ������ �������������
        EventSystem.current.SetSelectedGameObject(null);
        confirmButton.Select();
    }

    public void HideExitPanel()
    {
        exitPanel.SetActive(false);

        // ����� ������, ����� ��� �������� ������ ������ �� �������� ����������
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void QuitToMenuGame()
    {
        LoadingScreenManager.Instance.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
