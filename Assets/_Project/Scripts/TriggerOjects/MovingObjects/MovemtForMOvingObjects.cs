using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovemtForMOvingObjects : MonoBehaviour
{
    [Header("���������")]
    [SerializeField] private Transform _platform; // ������, ������� ����� ��������� (��������, ���� ��� ���������)
    public Transform Platform => _platform;

    [Header("��������� ��������")]
    [SerializeField] private float _moveSpeed = 2.0f;       // �������� ����������� �������
    [SerializeField] private float _rotationSpeed = 200f;     // �������� �������� �������

    // ����, ������������, ����� �� ������� ������
    private bool _needToMove = false;

    void Update()
    {
        if (_needToMove)
        {
            // �������� ���� (��������������, ��� InputManager ���������� � ����� �������)
            Vector2 input = InputManager.GetInstance().GetMoveDirection();
            Vector3 moveInput = new Vector3(input.x, 0, input.y).normalized;

            if (moveInput.sqrMagnitude > 0.01f)
            {
                // ���������� ������
                _platform.position += moveInput * _moveSpeed * Time.deltaTime;

                // ������ ������������ ������ � ����������� ��������
                Quaternion targetRotation = Quaternion.LookRotation(moveInput);
                _platform.rotation = Quaternion.RotateTowards(_platform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
            }
        }
    }

    // ����� ������������ ������ �������� �������.
    // ���������� �� PlayerMoveController ��� ������� �� ������ E.
    public void ChangeNeedToMovie()
    {
        _needToMove = !_needToMove;
        Debug.Log("MovemtForMOvingObjects active: " + _needToMove);
    }
}
