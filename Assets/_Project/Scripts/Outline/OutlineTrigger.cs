using UnityEngine;
using cakeslice;

[RequireComponent(typeof(Collider))]  // ���������, ��� �� ��� �� ������� ���� Collider (�� ����������� Trigger)
public class OutlineTrigger : MonoBehaviour
{
    private Outline[] outlines;
    private InteractableItem interactable;

    // ���������
    private bool isPlayerInTrigger = false;
    private bool isMouseOver = false;

    void Start()
    {
        // ���� ��� Outline � �����
        outlines = GetComponentsInChildren<Outline>(true);

        // ���������
        foreach (var o in outlines) o.enabled = false;

        // ��� ������ �������������� (����� ���� null)
        interactable = GetComponent<InteractableItem>();
    }

    private void UpdateOutlineState()
    {
        // ����� �� ������������?
        bool canHighlight = (interactable == null || !interactable.HasBeenUsed);
        bool shouldBeOn = canHighlight && (isPlayerInTrigger || isMouseOver);

        foreach (var o in outlines)
            o.enabled = shouldBeOn;
    }

    // ��� ������� ������ ���
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = true;
            UpdateOutlineState();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false;
            UpdateOutlineState();
        }
    }

    // ��� ���� ��� �������� ���
    // ��� ������� ����� ����������, ���� � ����� �� GameObject ���� Collider (�� ����������� isTrigger)
    void OnMouseEnter()
    {
        isMouseOver = true;
        UpdateOutlineState();
    }

    void OnMouseExit()
    {
        isMouseOver = false;
        UpdateOutlineState();
    }
}
