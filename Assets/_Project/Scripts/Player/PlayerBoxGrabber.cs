using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PlayerBoxGrabber : MonoBehaviour
{
    [Header("Grab Settings")]
    [Tooltip("������������ ���������� ��� ������� �����")]
    [SerializeField] private float grabRange = 2.0f;
    [Tooltip("������� ��� �������/������� �����")]
    [SerializeField] private KeyCode grabKey = KeyCode.E;
    [Tooltip("�������� ����������� ����� ��� ��������")]
    [SerializeField] private float boxMoveSpeed = 3.0f;

    private Transform cameraTransform;

    [Inject]
    private void Construct(Transform camTransform)
    {
        cameraTransform = camTransform;
    }

    // ������ �� ����, �� ������� ���������
    private Transform attachedBox = null;
    // ������������� offset ����� �������� ����� � ������ � ������ �������
    private Vector3 grabOffset = Vector3.zero;

    void Update()
    {
        // ������� ������� ��� �������/�������
        if (Input.GetKeyDown(grabKey))
        {
            if (attachedBox == null)
            {
                TryAttachBox();
            }
            else
            {
                DetachBox();
            }
        }

        // ���� �������� �������
        if (attachedBox != null)
        {
            // �������� ����� ���� ��� �����������
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            Vector3 input = new Vector3(horizontal, 0, vertical);

            if (input.sqrMagnitude > 0.01f)
            {
                input = input.normalized;
                // �������� �������������� ��� �� ������:
                Vector3 camForward = cameraTransform.forward;
                camForward.y = 0;
                camForward.Normalize();
                Vector3 camRight = cameraTransform.right;
                camRight.y = 0;
                camRight.Normalize();

                // �������� ����������� �������� ������������ ������������ ������,
                // ����� "�����" ��������������� �����������, ������� ����� ������
                Vector3 moveDir = (camForward * vertical + camRight * horizontal).normalized;

                // ���������� ���� �� ������������ �����������
                attachedBox.position += moveDir * boxMoveSpeed * Time.deltaTime;
            }

            // ��������� ������� ������, ����� ��������� ������������� offset �� �����
            transform.position = attachedBox.position + grabOffset;
        }
    }

    // ����� ���������� ����� � �������� �� ����
    private void TryAttachBox()
    {
        // ���� ��� ������� � ������� grabRange �� ������
        Collider[] colliders = Physics.OverlapSphere(transform.position, grabRange);
        foreach (Collider col in colliders)
        {
            // ��������� ��� (��������, "Box")
            if (col.CompareTag("Box"))
            {
                attachedBox = col.transform;
                // ���������� offset ����� �������� ������ � �����
                grabOffset = transform.position - attachedBox.position;
                Debug.Log("��������: ����� ��������� �� ���� " + attachedBox.name);
                return;
            }
        }
        Debug.Log("���� � ���� ������� �� ������.");
    }

    // ������ ��������
    private void DetachBox()
    {
        if (attachedBox != null)
        {
            Debug.Log("�������� ����� � ����� " + attachedBox.name);
            attachedBox = null;
        }
    }

    // ������������ ������� �������
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, grabRange);
    }
}
