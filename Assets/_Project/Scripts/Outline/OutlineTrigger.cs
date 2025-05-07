using UnityEngine;
using cakeslice;

[RequireComponent(typeof(Collider))]
public class OutlineTrigger : MonoBehaviour
{
    [Header("Hover Settings")]
    [Tooltip("������������ ���������, �� ������� �� ��������� ��������� �������")]
    [SerializeField] private float hoverCheckDistance = 100f;
    [Tooltip("����(�) ��� �������� ���������")]
    [SerializeField] private LayerMask hoverLayerMask = ~0;

    private Outline[] outlines;
    private Collider[] colliders;
    private InteractableItem interactable;

    private bool isPlayerInTrigger;
    private bool isMouseOver;

    private void Start()
    {
        // �������� ��� Outline-� � Collider-� �� ������� � ��� �������� ��������
        outlines = GetComponentsInChildren<Outline>(true);
        colliders = GetComponentsInChildren<Collider>(true);
        interactable = GetComponent<InteractableItem>();

        // ��������� ��������� �� ���������
        foreach (var o in outlines)
            o.enabled = false;
    }

    private void Update()
    {
        // ��������� �������� ��������� �� ����� �� �����������
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool hitThis = false;

        foreach (var col in colliders)
        {
            // ��������� �� ����
            if (((1 << col.gameObject.layer) & hoverLayerMask) == 0) continue;

            // ��������� ��������� ���� � ���������� ���������
            if (col.Raycast(ray, out _, hoverCheckDistance))
            {
                hitThis = true;
                break;
            }
        }

        if (hitThis != isMouseOver)
        {
            isMouseOver = hitThis;
            UpdateOutlineState();
        }
    }

    private void UpdateOutlineState()
    {
        bool canHighlight = interactable == null || !interactable.HasBeenUsed;
        bool shouldBeOn = canHighlight && (isPlayerInTrigger || isMouseOver);

        foreach (var o in outlines)
            o.enabled = shouldBeOn;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = true;
            UpdateOutlineState();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false;
            UpdateOutlineState();
        }
    }
}
