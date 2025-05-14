using UnityEngine;

/// <summary> ������ 4 �������� ���������� ������ �������. </summary>
public class BoxGripPoints : MonoBehaviour
{
    [Tooltip("Front, Back, Left, Right")]
    public Transform[] points = new Transform[4];

#if UNITY_EDITOR
    // ������������� ��� �������� (Editor ������ ��� �������)
    [ContextMenu("Auto-create points")]
    private void AutoCreate()
    {
        // ���� ��� ��������� � �� �������
        if (points[0]) return;

        var box = GetComponent<Collider>() as BoxCollider;
        if (!box) { Debug.LogWarning("BoxCollider not found"); return; }

        Vector3 e = box.size * 0.5f;

        Vector3[] localPos =
        {
            new Vector3(0,          0, +e.z),   // Front
            new Vector3(0,          0, -e.z),   // Back
            new Vector3(-e.x,       0, 0),      // Left
            new Vector3(+e.x,       0, 0)       // Right
        };

        string[] names = { "Front", "Back", "Left", "Right" };

        points = new Transform[4];

        for (int i = 0; i < 4; ++i)
        {
            var go = new GameObject(names[i]);
            go.transform.SetParent(transform, false);
            go.transform.localPosition = localPos[i];
            points[i] = go.transform;
        }
    }
#endif
}
