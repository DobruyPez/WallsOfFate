using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    [Header("������")]
    [SerializeField] private TextMeshPro textMesh;
    [SerializeField] private SpriteRenderer backgroundSR;

    [Header("���������")]
    [Tooltip("�� ������� ��� ������ ���� ������ ����� ������ ������")]
    [SerializeField] private float scaleFactorX = 1.2f;
    [Tooltip("�� ������� ��� ������ ���� ������ ����� ������ ������")]
    [SerializeField] private float scaleFactorY = 1.2f;
    [Tooltip("����� ����� ������ � ��������")]
    [SerializeField] private float lifeTime = 1f;
    [SerializeField] private Vector3 offset = new Vector3(0f, 2.5f, 0f);
    private Transform _player;

    void Awake()
    {
        if (textMesh == null)
            textMesh = GetComponentInChildren<TextMeshPro>();
        if (backgroundSR == null)
            backgroundSR = GetComponentInChildren<SpriteRenderer>();

        var playerGO = GameObject.FindGameObjectWithTag("Player");
        if (playerGO != null)
        {
            _player = playerGO.transform;
        }
        else
        {
            Debug.LogError("Player not found � please tag the player object as 'Player'.");
        }
    }

    /// <summary>
    /// ������������� ����� � ����� �� ��������� ��� �� ��������� �������� X � Y
    /// </summary>
    public void SetText(string msg)
    {
        // ��������� �����
        textMesh.text = msg;
        textMesh.ForceMeshUpdate();

        // �������� �������� ������
        Vector2 textSize = textMesh.GetRenderedValues(false);

        // ��������� ��� �� X � Y
        backgroundSR.size = new Vector2(
            textSize.x * scaleFactorX,
            textSize.y * scaleFactorY
        );
    }

    void Update()
    {
        //float xRotation = this.transform.localEulerAngles.x;
        //if (xRotation > 180f) xRotation -= 360f;
        //bool isInRotationRange = xRotation >= 85f && xRotation <= 95f;
        //if (isInRotationRange)
        //{
        //    this.transform.rotation = Quaternion.Euler(90f, 180f, 0f);
        //    //this.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        //}
        if (_player != null)
        {
            // ����� ������ x � y �� ������� ������, ��������� ������� z
            transform.position = new Vector3(
                _player.position.x + offset.x,
                _player.position.y + offset.y,
                _player.position.z + offset.z
            );
        }
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0f)
            Destroy(gameObject);
    }
}
