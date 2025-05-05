using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonsController : MonoBehaviour
{
    [SerializeField] private GameObject firstButton;

    private bool canAcceptInput = true;

    private void OnEnable()
    {
        // ������� �������������� ��������� ������� ���������
        if (LoadingScreenManager.Instance != null)
        {
            LoadingScreenManager.Instance.LoadingStarted += OnLoadingStarted;
            LoadingScreenManager.Instance.LoadingFinished += OnLoadingFinished;
        }
    }

    private void OnDisable()
    {
        if (LoadingScreenManager.Instance != null)
        {
            LoadingScreenManager.Instance.LoadingStarted -= OnLoadingStarted;
            LoadingScreenManager.Instance.LoadingFinished -= OnLoadingFinished;
        }
    }

    private void OnLoadingStarted()
    {
        canAcceptInput = false;
    }

    private void OnLoadingFinished()
    {
        StartCoroutine(EnableInputWithDelay(1f));
    }

    private IEnumerator EnableInputWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        canAcceptInput = true;
        // �� ������������� ����� ������������� � �������� ��� ������ ������������� �����
    }

    private void Update()
    {
        // ��o������ ���� ����, ���� ��� �������� ��� �� ���� delay
        if (!canAcceptInput ||
            (LoadingScreenManager.Instance != null && LoadingScreenManager.Instance.IsLoading))
            return;

        // ����������� ����
        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            if (InputModeTracker.UsingKeyboard)
            {
                InputModeTracker.NotifyMouseInput();
                EventSystem.current.SetSelectedGameObject(null);
            }
        }

        // ����������� ���������� (������� ��� W/S)
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow)
            || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S))
        {
            if (!InputModeTracker.UsingKeyboard)
                InputModeTracker.NotifyKeyboardInput();

            ClearMouseHoverEffect();

            // ��� ������ ������������� ����� ��������� ����� �� ������ ������
            if (EventSystem.current.currentSelectedGameObject == null)
                SetSelected(firstButton);
        }

        // Enter, Space ��� E
        if (Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.E))
        {
            var selected = EventSystem.current.currentSelectedGameObject;
            if (selected != null)
            {
                var btn = selected.GetComponent<Button>();
                if (btn != null)
                    btn.onClick.Invoke();
            }
        }
    }

    private void ClearMouseHoverEffect()
    {
        var pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (var res in results)
        {
            var effect = res.gameObject.GetComponent<UIButtonEffects>();
            if (effect != null)
                effect.ForceExit();
        }
    }

    private void SetSelected(GameObject go)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(go);
    }
}
