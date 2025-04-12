using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractManager : MonoBehaviour
{
    // ������ ���� ����������� ������������� �������� (����������� ITriggerable)
    private List<ITriggerable> encounteredTriggers = new List<ITriggerable>();
    // ����� ��� ������������ ��� �������������� ���������
    private HashSet<ITriggerable> triggeredSet = new HashSet<ITriggerable>();
    // ������� �������� ������������� ������
    private ITriggerable currentTriggerable;
    // ������-��������� �������������� (��������, ���������)
    private GameObject interactionIndicator;
    // ����, ����� �������� ���������� �������������� �� ������ �� ����
    private bool hasInteracted = false;
    // ��������� �������� �������������� �������
    private Collider currentTriggerCollider;

    // ������ �� ��������� �������� ������, ������� �������� ������ PlayPickupFloor, PlayPickupBody � PlayOpenChest
    private PlayerAnimator playerAnimator;

    private void Awake()
    {
       
        playerAnimator = GetComponent<PlayerAnimator>();
        if (playerAnimator == null)
        {
            Debug.LogError("InteractManager: �� ������ ��������� PlayerAnimator!");
        }

    }

    private void OnTriggerEnter(Collider collider)
    {
        ITriggerable newTriggerable = collider.gameObject.GetComponent<ITriggerable>();
        if (newTriggerable != null)
        {
            // ��������� ������������� ������ � ������, ���� ��� ��� ��� ���
            if (!encounteredTriggers.Contains(newTriggerable))
            {
                encounteredTriggers.Add(newTriggerable);
            }

            // ������������� ������� �������� ������������� ������
            if (currentTriggerCollider != collider || !hasInteracted)
            {
                currentTriggerable = newTriggerable;
                currentTriggerCollider = collider;
                hasInteracted = false;

                // ���� �������� ������ � ����� "InteractionIndicator" � ���������� ���
                var indicators = collider.gameObject.GetComponentsInChildren<Transform>(true);
                foreach (var indicator in indicators)
                {
                    if (indicator.CompareTag("InteractionIndicator"))
                    {
                        interactionIndicator = indicator.gameObject;
                        interactionIndicator.SetActive(true);
                        break;
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        // ��� ������ �� ���� �������������� ������� ���������� ������
        if (collider == currentTriggerCollider)
        {
            if (interactionIndicator != null)
            {
                interactionIndicator.SetActive(false);
            }

            encounteredTriggers.Clear();
            triggeredSet.Clear();
            currentTriggerable = null;
            currentTriggerCollider = null;
            interactionIndicator = null;
            hasInteracted = false;
        }
    }

    private void Update()
    {
        // ���� ���� �������� ������������� ������ � ��� �� ���� ��������������
        if (currentTriggerable != null && !hasInteracted)
        {
            // �������� ������� ������ �������������� ����� ��� InputManager
            bool isInteract = InputManager.GetInstance().GetInteractPressed();
            if (isInteract)
            {
                Debug.Log("Interacting with: " + currentTriggerable);

                // �������� currentTriggerable � MonoBehaviour ��� ������� � GameObject (��������������, ��� ITriggerable � ���������)
                GameObject triggerObj = (currentTriggerable as MonoBehaviour).gameObject;
                // � ����������� �� ���� ��������� ��������������� ��������
                if (triggerObj.CompareTag("PickupFloor"))
                {
                    playerAnimator.PlayPickupFloor();
                    TriggerAllEncounteredOnce();
                }
                else if (triggerObj.CompareTag("PickupBody"))
                {
                    playerAnimator.PlayPickupBody();
                    TriggerAllEncounteredOnce();
                }
                else if (triggerObj.CompareTag("Chest"))
                {
                    playerAnimator.PlayOpenChest();
                    TriggerAllEncounteredOnce();
                }
                else
                {
                    // ���� ��� �� ���������, ���������� ��� ����������� �������� ����������
                    TriggerAllEncounteredOnce();
                }
                hasInteracted = true;

                if (interactionIndicator != null)
                {
                    interactionIndicator.SetActive(false);
                }
            }
        }
    }

    // �������� ������������ ���������� �������, ���� �� ��� �� ��� ����������� (���� ���� ��� Box)
    private void TryTrigger(ITriggerable trigger)
    {
        if (!triggeredSet.Contains(trigger) || trigger is Box)
        {
            trigger.Triggered();
            triggeredSet.Add(trigger);
        }
        else
        {
            Debug.Log($"Trigger {trigger} already activated, skipping");
        }
    }

    // ����� ��� ������������ ������� ���� ����������� ���������
    public void TriggerAllEncounteredOnce()
    {
        foreach (var trigger in encounteredTriggers)
        {
            if (trigger != null)
            {
                TryTrigger(trigger);
            }
        }
    }

    // ����� ��� ��������� ������ ���� ����������� ������������� ��������
    public List<ITriggerable> GetEncounteredTriggers()
    {
        return new List<ITriggerable>(encounteredTriggers);
    }

    // ����� ��� ��������, ��� �� ������� ��� �����������
    public bool HasTriggerBeenActivated(ITriggerable trigger)
    {
        return triggeredSet.Contains(trigger);
    }
}
