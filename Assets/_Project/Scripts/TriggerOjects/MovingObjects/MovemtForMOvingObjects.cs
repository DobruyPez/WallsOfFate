using UnityEngine;

/// <summary>
/// ���������� �����������/�������� ����� ���������� ��������� (�����, ������� � �. �.).
/// </summary>
public class MovingPlatformController : MonoBehaviour
{
    #region Inspector
    [Header("���������")]
    [SerializeField] private Transform _platform;          // ������, ������� �������
    public Transform Platform => _platform;

    [Header("��������� ��������")]
    [SerializeField] private float _moveSpeed = 2.0f;  // �/�
    [SerializeField] private float _rotationSpeed = 200f;  // �/�
    #endregion

    public bool IsMoving => _needToMove;

    /*-------------------------------------------------------------------*/

    private bool _needToMove = false;
    private Rigidbody _rb;                      // �������������� Rigidbody

    private void Awake()
    {
        if (_platform == null)
        {
            Debug.LogWarning($"{name}: ��������� �� ���������, ���� self");
            _platform = transform;
        }

        _rb = _platform.GetComponent<Rigidbody>();
    }

    /*======================== INPUT & LOGIC ==========================*/

    private void Update()
    {
        if (!_needToMove) return;

        // --- �������� ���� ������ -----------------------------------
        InputManager inputMgr = InputManager.GetInstance();
        if (inputMgr == null) { Debug.LogError("InputManager not found"); return; }

        Vector2 input = inputMgr.GetMoveDirection();               // (x = �����������, y = ���������)
        Vector3 move = new Vector3(input.x, 0f, input.y).normalized;

        if (move.sqrMagnitude < 0.01f) return;                     // ������ �� ������

        // --- ������� (������ �����, �.�. RotateTowards �� ������� �� ������)
        Quaternion targetRot = Quaternion.LookRotation(move);
        _platform.rotation = Quaternion.RotateTowards(
            _platform.rotation,
            targetRot,
            _rotationSpeed * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (!_needToMove) return;

        InputManager inputMgr = InputManager.GetInstance();
        if (inputMgr == null) return;

        Vector2 input = inputMgr.GetMoveDirection();
        Vector3 move = new Vector3(input.x, 0f, input.y).normalized;

        if (move.sqrMagnitude < 0.01f) return;

        // --- ����������� (���� ���� Rigidbody � ���������� ����� ����)
        float step = _moveSpeed * Time.fixedDeltaTime;
        Vector3 newPos = _platform.position + move * step;

        if (_rb != null && !_rb.isKinematic)
            _rb.MovePosition(newPos);
        else
            _platform.position = newPos;
    }

    /*======================== PUBLIC API =============================*/

    /// <summary>���������� ����� (��������, �� PlayerMoveController) ��� ������� E.</summary>
    public void ToggleMovement()
    {
        _needToMove = !_needToMove;
        Debug.Log($"{name}: moving = {_needToMove}");
    }
}
