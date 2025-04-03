using UnityEngine;

public class BoxGrabber : MonoBehaviour
{
    [Header("��������� �������")]
    // ����������, � �������� �������� ����� �������� �������
    public float grabDistance = 2.0f;
    // ������� ��� �������/������� �������
    public KeyCode grabKey = KeyCode.E;
    // �����, � ������� ���������� ������ (��������, ������� ��� ���������)
    public Transform grabPoint;

    // ������� FixedJoint, ����������� ������� � ����������
    private FixedJoint grabJoint;
    // Rigidbody ����������� �������
    private Rigidbody grabbedBoxRb;

    void Update()
    {
        if (Input.GetKeyDown(grabKey))
        {
            if (grabJoint == null)
            {
                TryGrab();
            }
            else
            {
                ReleaseGrab();
            }
        }
    }

    // ����� ������� ������� �������
    void TryGrab()
    {
        // ������� ��� ���������� � �������� grabDistance �� ����� �������
        Collider[] hits = Physics.OverlapSphere(grabPoint.position, grabDistance);
        foreach (Collider hit in hits)
        {
            // ��� ������� ���� ������ � ����� "Box"
            if (hit.CompareTag("Box"))
            {
                Rigidbody rb = hit.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    // ��������� FixedJoint � ������� grabPoint � ��������� ��� � Rigidbody �������
                    grabJoint = grabPoint.gameObject.AddComponent<FixedJoint>();
                    grabJoint.connectedBody = rb;
                    grabbedBoxRb = rb;
                    // ��� ������������� ����� ��������� ��������� ����������� (��������, breakForce)
                    return;
                }
            }
        }
    }

    // ����� ���������� �������
    void ReleaseGrab()
    {
        if (grabJoint != null)
        {
            Destroy(grabJoint);
            grabJoint = null;
            grabbedBoxRb = null;
        }
    }

    // ��������� ����� ������� � ��������� ��� �������� ���������
    private void OnDrawGizmosSelected()
    {
        if (grabPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(grabPoint.position, grabDistance);
        }
    }
}
