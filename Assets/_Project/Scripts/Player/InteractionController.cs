
using UnityEngine;

public class InteractionController : MonoBehaviour
{
    private PlayerAnimator playerAnimator;

    // ��������� �������� ������������� ������, � ������� ��������� �����.
    private GameObject currentInteractable;

    private void Awake()
    {
        playerAnimator = GetComponent<PlayerAnimator>();
        if (playerAnimator == null)
        {
            Debug.LogError("InteractionController: �� ������ ��������� PlayerAnimator!");
        }
    }

    // ����������, ����� ������ ��������� ������ � �������-��������� ������.
    private void OnTriggerEnter(Collider other)
    {
        // ���������� ������������� ������� �� ����.
        if (other.CompareTag("PickupFloor") ||
            other.CompareTag("PickupBody") ||
            other.CompareTag("Chest"))
        {
            currentInteractable = other.gameObject;
        }
    }

    // ����������, ����� ������ ��������� ������� �� �������-���������� ������.
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == currentInteractable)
        {
            currentInteractable = null;
        }
    }

    private void Update()
    {
        // ���� ������ ������� E � ���� �������� ������������� ������
        if (Input.GetKeyDown(KeyCode.E) && currentInteractable != null)
        {
            if (currentInteractable.CompareTag("PickupFloor"))
            {
                playerAnimator.PlayPickupFloor();
            }
            else if (currentInteractable.CompareTag("PickupBody"))
            {
                playerAnimator.PlayPickupBody();
            }
            else if (currentInteractable.CompareTag("Chest"))
            {
                playerAnimator.PlayOpenChest();
            }
        }
    }
}
