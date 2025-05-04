using UnityEngine;
using cakeslice;

public class OutlineTrigger : MonoBehaviour
{
    private Outline[] outlines;
    private InteractableItem interactable;

    void Start()
    {
        // �������� ��� Outline ���������� � �������� ��������
        outlines = GetComponentsInChildren<Outline>();

        // ��������� ��� �� ���������
        foreach (var outline in outlines)
        {
            outline.enabled = false;
        }

        // ���� ������ �������������� (����� �������������!)
        interactable = GetComponent<InteractableItem>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // ��������: ���� ���� InteractableItem, �� ������������ ������ ���� �� �����������
            if (interactable == null || !interactable.HasBeenUsed)
            {
                foreach (var outline in outlines)
                {
                    outline.enabled = true;
                }
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
