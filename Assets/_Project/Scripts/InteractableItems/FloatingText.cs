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

    void Awake()
    {
        if (textMesh == null)
            textMesh = GetComponentInChildren<TextMeshPro>();
        if (backgroundSR == null)
            backgroundSR = GetComponentInChildren<SpriteRenderer>();
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
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0f)
            Destroy(gameObject);
    }
}
