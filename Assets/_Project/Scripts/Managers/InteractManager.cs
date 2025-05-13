using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractManager : MonoBehaviour
{
    // ������ ���� ����������� ������������� �������� (����������� ITriggerable)
    private HashSet<ITriggerable> encounteredTriggers = new HashSet<ITriggerable>();
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

    [Tooltip("����������� ����� ����� ���������� ���������, ���.")]
    [SerializeField] private float interactCooldown = 0.4f;
    private float _nextTimeCanInteract = 0f;
    private bool _interactBuffered;
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
        // �������� ��� ���������� ITriggerable �� �������
        ITriggerable[] triggerables = collider.gameObject.GetComponents<ITriggerable>();

        if (triggerables.Length == 0) return;

        bool isNewCollider = currentTriggerCollider != collider;

        foreach (var triggerable in triggerables)
        {
            // ��������� ��� �������� � ������
            if (!encounteredTriggers.Contains(triggerable))
            {
                encounteredTriggers.Add(triggerable);
            }

            // ��������� ������� �������� ������� ������ ����:
            // - ��� ����� ���������
            // - ��� ��� �� ���� ��������������
            if (isNewCollider || !hasInteracted)
            {
                currentTriggerable = triggerable;
                currentTriggerCollider = collider;
                hasInteracted = false;
            }
        }

        // ����� ���������� �������������� (���� �� ���� ������)
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

    private void OnTriggerStay(Collider collider)
    {
        // ��������� currentTriggerable ������ FixedUpdate,
        // ���� �� �����-�� ������� �� ����� ���� null
        if (currentTriggerable == null)
        {
            OnTriggerEnter(collider);   // �������������� ��� ������� ������
            _nextTimeCanInteract = Time.time;   // �� ��������� ������ �������
        }

        // --- �������������, �� ������: ��������� ����� ������� ---
        if (!_interactBuffered && InputManager.GetInstance().GetInteractPressed())
            _interactBuffered = true;
    }

    private void OnTriggerExit(Collider collider)
    {
        // ��� ������ �� ���� �������������� ������� ���������� ������
        ClearData(collider);
    }

    private void ClearData(Collider collider = null, bool onDestroy = false)
    {
        if (interactionIndicator != null)
        {
            interactionIndicator.SetActive(false);
        }

        encounteredTriggers.Clear();
        triggeredSet.Clear();
        currentTriggerable = null;
        interactionIndicator = null;
        hasInteracted = false;
        if (collider == currentTriggerCollider || onDestroy)
        {
            currentTriggerCollider = null;
        }
    }


    private void Update()
    {
        if (currentTriggerable == null) return;
        if (Time.time < _nextTimeCanInteract) return;

        // ���� ������ � ���� �����, ���� ������� �������������� � OnTriggerStay
        if (_interactBuffered || InputManager.GetInstance().GetInteractPressed())
        {
            _interactBuffered = false;          // ����� ������
            InteractWith(currentTriggerable);
            _nextTimeCanInteract = Time.time + interactCooldown;
        }
    }

    // �������� ������������ ���������� �������, ���� �� ��� �� ��� ����������� (���� ���� ��� Box)
    private void TryTrigger(ITriggerable trigger)
    {
        // 1) ����� �� ������� �������� �����������?
        //    � ������ ��������� �Dialogue� � �Box�
        bool repeatable =
            trigger is Box ||
            (trigger as MonoBehaviour)?.CompareTag("Dialogue") == true;

        // 2) ��������� ����������� �� ������
        if (!triggeredSet.Contains(trigger) || repeatable)
        {
            if (trigger is InteractableItem item)
                item.Interact();
            else
                trigger.Triggered();

            // � ������� ������ ������� ������ �����������
            if (!repeatable)
                triggeredSet.Add(trigger);
        }
    }

    public void InteractWith(ITriggerable trigger)
    {
        if (trigger == null) return;

        // �������� MonoBehaviour, �� ������� ����� ��� ITriggerable-����������
        var mb = trigger as MonoBehaviour;
        if (!mb) return;                       // �� ������ ������

        foreach (var t in mb.GetComponents<ITriggerable>())
            TryTrigger(t);                     // �������� Interact() ��� Triggered()

        // �������� ������ �� ���� � ��� ����
        GameObject go = mb.gameObject;
        if (go.CompareTag("PickupFloor")) playerAnimator.PlayPickupFloor();
        else if (go.CompareTag("PickupBody")) playerAnimator.PlayPickupBody();
        else if (go.CompareTag("Chest")) playerAnimator.PlayOpenChest();
        else if (go.CompareTag("Box"))
        {
            var grabber = GetComponent<PlayerBoxGrabber>();
            if (grabber != null)
                grabber.ToggleGrab(go.transform);   // ������ ������
        }

        hasInteracted = true;
        if (interactionIndicator) interactionIndicator.SetActive(false);
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

    private void OnDisable()
    {
        ClearData(null, true);
    }
}
