using UnityEngine;

public class CursorController : MonoBehaviour
{
    // ������ �� ������ ����������, ��� ������� ������ ���� ������� ������
    [SerializeField] private GameObject[] uiPanels;

    private void Start()
    {
        // ���������� ������ ��������: ����� � ������������
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        bool anyPanelActive = false;
        foreach (GameObject panel in uiPanels)
        {
            // ���� ���� �� ���� ������ �������, ��������� ����
            if (panel.activeSelf)
            {
                anyPanelActive = true;
                break;
            }
        }

        if (anyPanelActive)
        {
            // ���� ���� �� ���� �� ��������� ������� ������� � ������ ������ ������� � ����������������
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            // ���� �� ���� �� ������� �� ������� � �������� ������ � ��������� ���
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
