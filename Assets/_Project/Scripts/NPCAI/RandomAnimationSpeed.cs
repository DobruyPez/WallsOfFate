using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ArcherIdleAsShoot : MonoBehaviour
{
    [Header("��� ����")]
    public Transform target;

    [Header("���, ������� ������� �����")]
    public string playerTag = "Player";

    [Header("�������� ��������� ��������")]
    public float minSpeed = 0.8f;
    public float maxSpeed = 1.2f;

    private Animator animator;
    private bool isBlocked;
    private float currentSpeed;

    // ����� ��� ������ ��������� Idle, � ������� �������� ���� Shoot
    private readonly int idleStateHash = Animator.StringToHash("Idle");

    void Start()
    {
        animator = GetComponent<Animator>();
        // �������� ����� ���������� (Idle-������) �� ��������� ��������
        ResumeLoop();
    }

    void Update()
    {
        if (animator == null || target == null)
            return;

        // ��� �� ���� ���� ��� � ������
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Vector3 dir = (target.position - origin).normalized;
        float dist = Vector3.Distance(origin, target.position);

        bool nowBlocked = false;
        if (Physics.Raycast(origin, dir, out var hit, dist))
            nowBlocked = hit.collider.CompareTag(playerTag);

        // ���� � ����������: ������ ��� ���������� �� 0-� ���� � ��������
        if (nowBlocked && !isBlocked)
        {
            isBlocked = true;
            animator.Play(idleStateHash, 0, 0f);   // ������������ ������� �� ������ �����
            animator.Update(0f);                  // ��������� ������ ���� �����
            animator.speed = 0f;                  // ������������ ��������
            return;
        }
        // ����� �� ����������: ������������ ���� �� ����� ��������
        else if (!nowBlocked && isBlocked)
        {
            isBlocked = false;
            ResumeLoop();
        }
    }

    // ������/������������� ������������ Shoot-����� � ����� ���������
    private void ResumeLoop()
    {
        currentSpeed = Random.Range(minSpeed, maxSpeed);
        animator.speed = currentSpeed;
        animator.Play(idleStateHash, 0, 0f);
    }

    void OnDrawGizmosSelected()
    {
        if (target == null) return;
        Gizmos.color = Color.red;
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Gizmos.DrawLine(origin, target.position);
    }
}
