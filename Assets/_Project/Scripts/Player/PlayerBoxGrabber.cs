using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBoxGrabber : MonoBehaviour
{
    [Header("Grab Settings")]
    [SerializeField, Tooltip("������������ ���������� ��� ������� �����")]
    private float grabRange = 2.0f;
    [SerializeField, Tooltip("������� ��� �������/������� �����")]
    private KeyCode grabKey = KeyCode.E;

    [Header("Box Movement Settings")]
    [SerializeField, Tooltip("������������ �������� �����")]
    private float maxSpeed = 5f;
    [SerializeField, Tooltip("�������� �������� ����� (������� � �������)")]
    private float turnSpeed = 100f;

    // ������ �� ����, �� ������� ���������
    private Transform attachedBox = null;
    // ��������� offset ����� �������� ����� � ������ � ������ �������
    private Vector3 localGrabOffset = Vector3.zero;
    // ������� �������� �������� ����� (������������� � �����, ������������� � �����)
    private float currentSpeed = 0f;

    private void Update()
    {
        // ��������� ������� �������/�������
        if (Input.GetKeyDown(grabKey))
        {
            if (attachedBox == null)
                TryAttachBox();
            else
                DetachBox();
        }

        if (attachedBox != null)
        {
            // �������� ���� �� ������
            float vertical = Input.GetAxis("Vertical");
            float horizontal = Input.GetAxis("Horizontal");

            // ���� ���� ���� �� ��������� � ��������� ������������� ��������
            if (Mathf.Abs(vertical) > 0.1f)
            {
                currentSpeed = vertical * maxSpeed;
            }
            else
            {
                currentSpeed = 0f;
            }

            // ������� ����� �������������� � ������� ��������������� ����� (������ ���� ���� ��������)
            if (Mathf.Abs(currentSpeed) > 0.1f)
            {
                float turnAmount = horizontal * turnSpeed * Time.deltaTime;
                // ���� �������� �����, ����������� ������� ��� ������������ ����������
                if (vertical < 0)
                    turnAmount = -turnAmount;
                attachedBox.Rotate(0, turnAmount, 0);
            }

            // ���������� ����: �� �������� � �����������, ���� ������� �����
            attachedBox.position += transform.forward * currentSpeed * Time.deltaTime;

            // ��������� ����� ������ �������� �������� �������� �����, ��� ������� ����������� ������������ �����.
            // �� ��� ��������� ���������� ����� ��� � ������������� ��������� �������:
            transform.position = attachedBox.TransformPoint(localGrabOffset);

            // ����� ������ ������� �� ���� (�� �����������)
            Vector3 dirToBox = attachedBox.position - transform.position;
            dirToBox.y = 0;
            if (dirToBox != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(dirToBox);
            }
        }
    }

    // ����� ���������� ����� � ������ (��������)
    private void TryAttachBox()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, grabRange);
        foreach (Collider col in cols)
        {
            if (col.CompareTag("Box"))
            {
                attachedBox = col.transform;
                // ��������� ��������� offset: ������� ������ � ��������� ������� ��������� �����
                localGrabOffset = attachedBox.InverseTransformPoint(transform.position);
                currentSpeed = 0f;
                // ������ ������ �������� �������� �����, ����� ��������� ������������� ���������
                transform.SetParent(attachedBox);
                Debug.Log("��������: ����� �������� ���� " + attachedBox.name);
                return;
            }
        }
        Debug.Log("���� � ���� ������� �� ������.");
    }

    // ���������� �����
    private void DetachBox()
    {
        if (attachedBox != null)
        {
            Debug.Log("�������� ����� � ����� " + attachedBox.name);
            // ��������� ������ (������� ��������)
            transform.SetParent(null);
            attachedBox = null;
            currentSpeed = 0f;
        }
    }

    // ��� ������������ ������� ������� � ���������
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, grabRange);
    }
}
