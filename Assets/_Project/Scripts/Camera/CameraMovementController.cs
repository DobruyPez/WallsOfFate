using UnityEngine;
using Zenject;

public class CameraMovementController : MonoBehaviour
{
    // �������� ������������ ��������� ����� ����
    [SerializeField] private Vector3 offset = new Vector3(0f, 10f, -10f);
    // ����������� ��������� �������� ������
    [SerializeField] private float smoothSpeed = 0.125f;

    // ���� ��� ��������� ��������������� ������
    [SerializeField] private float angleX = 30f;
    [SerializeField] private float angleY = 45f;

    // �������� ���� �� ����������� ����� ������������ ������, ����� �������� ��������� ���� ������ �����
    [SerializeField] private float verticalBias = 1f;

    // ��������� ���� ������ (��� ������������� ������ � ��������� ���� ������)
    [SerializeField] private float zoomSpeed = 2f;
    [SerializeField] private float minFOV = 15f;
    [SerializeField] private float maxFOV = 90f;

    // ������ �� ��������� ������
    private Camera cam;
    // ����, �� ������� ����� ��������� ������ (��������, ��������)
    private Transform _player;

    [Inject]
    private void Construct(PlayerMoveController player)
    {
        Debug.Log("Player injected: " + player);
        _player = player.gameObject.transform;
    }

    void Awake()
    {
        if (_player == null)
        {
            Debug.LogError("Player is not assigned!");
            enabled = false;
            return;
        }
        cam = GetComponent<Camera>();
        if (cam != null)
        {
            // ��������� ������ � ������������� �����
            cam.orthographic = false;
            // ������������� ��������� �������� ���� ������ (Field of View)
            cam.fieldOfView = 40f;
        }

        // ������������� �������������� ����������� ������
        transform.rotation = Quaternion.Euler(angleX, angleY, 0f);
    }

    void Update()
    {
        // ��������� ���� � ������� ������� ���� ��� ������������� ������ (��������� Field of View)
        if (cam != null)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.01f)
            {
                cam.fieldOfView -= scroll * zoomSpeed;
                cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, minFOV, maxFOV);
            }
        }
    }

    void LateUpdate()
    {
        if (_player != null)
        {
            // ������� ����� ���������� �� �������� verticalBias ����� ����������� "�����" ������
            Vector3 biasedTargetPosition = _player.position + transform.up * verticalBias;
            // ������������ �������� ������� ������ � ������ ��������
            Vector3 desiredPosition = biasedTargetPosition + offset;
            // ������� ����������� ������
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }
}
