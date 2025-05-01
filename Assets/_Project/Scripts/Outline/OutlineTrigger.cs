using UnityEngine;
using cakeslice;

public class OutlineTrigger : MonoBehaviour
{
    private Outline[] outlines;

    void Start()
    {
        // �������� ��� Outline ���������� � �������� ��������
        outlines = GetComponentsInChildren<Outline>();

        // ��������� ��� �� ���������
        foreach (var outline in outlines)
        {
            outline.enabled = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (var outline in outlines)
            {
                outline.enabled = true;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (var outline in outlines)
            {
                outline.enabled = false;
            }
        }
    }
}
