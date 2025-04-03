using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractManager : MonoBehaviour
{
    private List<ITriggerable> encounteredTriggers = new List<ITriggerable>();
    private HashSet<ITriggerable> triggeredSet = new HashSet<ITriggerable>(); // ��� ������������ ��� �������������� ���������
    private ITriggerable currentTriggerable;
    private GameObject interactionIndicator;
    private bool hasInteracted = false;
    private Collider currentTriggerCollider;

    private void OnTriggerEnter(Collider collider)
    {
        ITriggerable newTriggerable = collider.gameObject.GetComponent<ITriggerable>();
        if (newTriggerable != null)
        {
            // ��������� ������� � ������, ���� ��� ��� ��� ���
            if (!encounteredTriggers.Contains(newTriggerable))
            {
                encounteredTriggers.Add(newTriggerable);
            }

            // ������������� ������� �������� �������
            if (currentTriggerCollider != collider || !hasInteracted)
            {
                currentTriggerable = newTriggerable;
                currentTriggerCollider = collider;
                hasInteracted = false;

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
        //// ��������� �������� ��������
        //if (currentTriggerable != null && currentTriggerable is Box)
        //{
        //    bool isInteract = InputManager.GetInstance().GetInteractPressed();
        //    Debug.Log("Interacting with: " + currentTriggerable);
        //    TryTrigger(currentTriggerable);
        //    hasInteracted = true;
        //    if (interactionIndicator != null)
        //    {
        //        interactionIndicator.SetActive(false);
        //    }
        //}
        if (currentTriggerable != null && (currentTriggerable is Box || !hasInteracted))
        {
            bool isInteract = InputManager.GetInstance().GetInteractPressed();

            if (isInteract)
            {
                Debug.Log("Interacting with: " + currentTriggerable);
                TriggerAllEncounteredOnce(); // ��������������� �����
                //TryTrigger(currentTriggerable); // ���������� ����� �������
                hasInteracted = true;


                if (interactionIndicator != null)
                {
                    interactionIndicator.SetActive(false);
                }
            }
        }
    }

    // �������� ������������ �������, ���� ��� �� ������������
    private void TryTrigger(ITriggerable trigger)
    {
        if (!triggeredSet.Contains(trigger) || trigger is Box)
        {
            trigger.Trrigered();
            triggeredSet.Add(trigger);
        }
        else
        {
            Debug.Log($"Trigger {trigger} already activated, skipping");
        }
    }

    // ����� ��� ������� ���� ����������� ��������� �� ������ ����
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

    // ����� ��� ��������� ������ ���� ����������� ���������
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