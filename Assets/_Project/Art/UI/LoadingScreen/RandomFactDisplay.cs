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

    private static int currentIndex = 0;
    private static bool allFactsShown = false;

    private void OnEnable()
    {
        RefreshFact();
    }

    /// <summary>
    /// ���������� ��������� ���� �� �������, � ����� ��������� � ���������.
    /// </summary>
    public void RefreshFact()
    {
        if (factText != null && facts != null && facts.Length > 0)
        {
            string selectedFact;

            if (!allFactsShown)
            {
                selectedFact = facts[currentIndex];
                currentIndex++;

                if (currentIndex >= facts.Length)
                {
                    allFactsShown = true;
                }
            }
            else
            {
                int randomIndex = Random.Range(0, facts.Length);
                selectedFact = facts[randomIndex];
            }

            factText.text = selectedFact;
        }
    }
}
