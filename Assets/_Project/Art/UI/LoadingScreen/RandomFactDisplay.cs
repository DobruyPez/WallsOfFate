using UnityEngine;
using TMPro;

public class RandomFactDisplay : MonoBehaviour
{
    [Header("��������� ������")]
    [Tooltip("��������� TextMeshPro ��� ����������� �����.")]
    public TMP_Text factText;

    [Tooltip("������ ������, �� �������� ���������� ���� ���������.")]
    [TextArea(2, 5)]
    public string[] facts;

    private void OnEnable()
    {
        RefreshFact();
    }

    /// <summary>
    /// �������� ��������� ���� � ����������� ��� ���������� ����������.
    /// </summary>
    public void RefreshFact()
    {
        if (factText != null && facts != null && facts.Length > 0)
        {
            int index = Random.Range(0, facts.Length);
            factText.text = facts[index];
        }
    }
}
