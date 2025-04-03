using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class CameraObstacleTransparency : MonoBehaviour
{
    [Header("��������� ��������")]
    public LayerMask obstacleMask;        // ����, �� ������� ������ �����������
    public float fadeSpeed = 3f;          // �������� ��������� ������������
    [Range(0f, 1f)]
    public float transparentAlpha = 0.3f; // �������� ������������ (�� 0 �� 1)

    // ������� ��� �������� ������������ ������ ��� ������� ��������� �������
    private Dictionary<Renderer, Color[]> originalColors = new Dictionary<Renderer, Color[]>();
    // ����� ��������, ������� � ������ ������ ������ ���� ���������
    private HashSet<Renderer> currentObstacles = new HashSet<Renderer>();

    private Transform _player;

    [Inject]
    private void Construct(PlayerMoveController player)
    {
        Debug.Log("Player injected: " + player);
        _player = player.gameObject.transform;
    }

    void Update()
    {
        if (_player == null)
            return;

        // ���������� ����������� � ���������� �� ������ �� ����
        Vector3 direction = _player.position - transform.position;
        float distance = direction.magnitude;

        // ������� ��� ����������� ����� ������� � �����
        RaycastHit[] hits = Physics.RaycastAll(transform.position, direction.normalized, distance, obstacleMask);
        HashSet<Renderer> hitRenderers = new HashSet<Renderer>();

        foreach (RaycastHit hit in hits)
        {
            Renderer rend = hit.collider.GetComponent<Renderer>();
            if (rend == null)
                continue;

            hitRenderers.Add(rend);

            // ���� ������� ��������� ���� ������, ��������� ������������ �����
            if (!originalColors.ContainsKey(rend))
            {
                int count = rend.materials.Length;
                Color[] origColors = new Color[count];
                for (int i = 0; i < count; i++)
                {
                    // ���� ������� ���� �� ���������
                    origColors[i] = rend.materials[i].color;
                }
                originalColors[rend] = origColors;
            }

            // ��������� ������� ���������� ��� ������� ��������� ������� �������
            int matCount = rend.materials.Length;
            for (int i = 0; i < matCount; i++)
            {
                MaterialPropertyBlock block = new MaterialPropertyBlock();
                rend.GetPropertyBlock(block, i);

                // ���� ���� �� ����� � �����, ���������� ������������ ����
                Color currentColor = block.GetColor("_Color");
                if (currentColor == default(Color))
                {
                    currentColor = originalColors[rend][i];
                }

                // ������ ������ ����� � ����������� ��������
                float newAlpha = Mathf.MoveTowards(currentColor.a, transparentAlpha, fadeSpeed * Time.deltaTime);
                Color newColor = new Color(currentColor.r, currentColor.g, currentColor.b, newAlpha);
                block.SetColor("_Color", newColor);
                rend.SetPropertyBlock(block, i);
            }

            currentObstacles.Add(rend);
        }

        // ��� ��������, ������� ����� ���� ���������, �� ������ �� ������ � Raycast,
        // ���������� ������������ ���� (����� = 1 ��� �������� ��������)
        List<Renderer> toRemove = new List<Renderer>();
        foreach (Renderer rend in currentObstacles)
        {
            if (hitRenderers.Contains(rend))
                continue;

            bool fullyRestored = true;
            int matCount = rend.materials.Length;
            for (int i = 0; i < matCount; i++)
            {
                MaterialPropertyBlock block = new MaterialPropertyBlock();
                rend.GetPropertyBlock(block, i);
                Color currentColor = block.GetColor("_Color");
                if (currentColor == default(Color))
                {
                    // ���� ���� �� ����������, ���� ������������
                    currentColor = originalColors[rend][i];
                }
                float targetAlpha = originalColors[rend][i].a; // �������� �������� ����� (������ 1)
                float newAlpha = Mathf.MoveTowards(currentColor.a, targetAlpha, fadeSpeed * Time.deltaTime);
                Color newColor = new Color(currentColor.r, currentColor.g, currentColor.b, newAlpha);
                block.SetColor("_Color", newColor);
                rend.SetPropertyBlock(block, i);

                if (!Mathf.Approximately(newAlpha, targetAlpha))
                    fullyRestored = false;
            }

            if (fullyRestored)
            {
                // ���� ��� ���� ���������� ����� �������������, ������� ���������
                rend.SetPropertyBlock(new MaterialPropertyBlock());
                toRemove.Add(rend);
            }
        }

        // ������� ��������� ��������������� ������� �� ������ ������������� � ������� �� ����������� ���������
        foreach (Renderer rend in toRemove)
        {
            currentObstacles.Remove(rend);
            originalColors.Remove(rend);
        }
    }
}
