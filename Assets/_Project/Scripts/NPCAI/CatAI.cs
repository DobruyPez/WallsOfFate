using UnityEngine;
using UnityEngine.AI;
using Zenject;

public class CatAI : MonoBehaviour
{
    [Header("��������� ��������� � ������")]
    public float approachRange = 5f;       // ���� ����� � �������� 5 � � ��� �������� ���������
    public float tooCloseRange = 1.5f;       // ���� ����� ������� ������ (< 1.5 �) � ��� ���������� ����������� (����� Turn)
    public float retreatExitRange = 3f;      // ��� ���������� > 3 � �� ������ Retreat ��� ������������ � Idle

    [Header("��������� ������� (���������� ��������)")]
    public float minApproachDistance = 2f;   // ���� ��� ����������� ����� 2 � � �������, ��� �� ������� ������
    public float maxApproachDistance = 4f;   // ���������� ����������, ��� ������� ���� �������� ���������� ����� � �������

    [Header("��������� �����������")]
    public float wanderRadius = 10f;       // ������ ��� ���������� ���������
    public float idleTime = 2f;            // ����� ������� � ��������� Idle
    public float wanderSpeed = 1.5f;       // �������� ���������
    public float approachSpeed = 2.5f;     // �������� ��� ������� � ������
    public float retreatSpeed = 3.5f;      // �������� ����������� ��� ������� ������� �����������

    [Header("��������� ���������")]
    public float followMaxTime = 10f;      // ������������ �����, ������� ��� ����� ��������� �� �������

    [Header("��������� ��������")]
    public float turnDuration = 0.5f;      // �����, ������� ��� �������������� �� �����

    private NavMeshAgent agent;
    private Animator animator;
    private Transform player;

    [Inject]
    private void Construct(PlayerMoveController _player)
    {
        Debug.Log("Player injected: " + _player);
        player = _player.gameObject.transform;
    }

    // ���������� ��������� ����
    private enum CatState { Idle, Wander, Approach, Turn, Retreat }
    private CatState currentState;

    private float idleTimer;
    private float followTimer;  // ������ ���������� � ������ Approach
    private float turnTimer;    // ������ ��� �������� � ��������� Turn
    private Vector3 wanderTarget;
    private Quaternion targetRotation; // ����������� ���������� ��� ��������

    /// <summary>
    /// ������������� ��������� ��������� �� ��� �� �����, ��� � � ������:
    /// - Idle: Vert = 0, State = 0
    /// - ������: Vert = 1, State = 0
    /// - ��� (�����������): Vert = 1, State = 1
    /// </summary>
    private void SetAnimatorParameters(float vert, int state)
    {
        animator.SetFloat("Vert", vert);
        animator.SetInteger("State", state);
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        currentState = CatState.Idle;
        idleTimer = idleTime;
        followTimer = 0f;
        turnTimer = 0f;
        SetAnimatorParameters(0f, 0);
    }

    void Update()
    {
        float distToPlayer = Vector3.Distance(transform.position, player.position);

        // ������ ������������ ��������� � ������ ����������� ��������� � ��������
        switch (currentState)
        {
            case CatState.Idle:
            case CatState.Wander:
                if (distToPlayer < approachRange && distToPlayer > maxApproachDistance)
                {
                    currentState = CatState.Approach;
                    followTimer = 0f; // ����� ������� ��� ����� � ����� Approach
                }
                if (distToPlayer < tooCloseRange)
                {
                    // ������ ������� �������� � Retreat ��������� � Turn
                    PrepareTurn();
                }
                break;

            case CatState.Approach:
                if (distToPlayer < tooCloseRange)
                {
                    PrepareTurn();
                }
                else if (distToPlayer > approachRange)
                {
                    currentState = CatState.Idle;
                    idleTimer = idleTime;
                    followTimer = 0f;
                }
                else
                {
                    followTimer += Time.deltaTime;
                    if (followTimer > followMaxTime)
                    {
                        // ���� ������� ��������� �� ������� � ��������� � ����� ���������
                        currentState = CatState.Wander;
                        followTimer = 0f;
                    }
                }
                break;

            case CatState.Turn:
                // � ��������� Turn ������ ����, ���� �� ������� �������� ����� �������� ��� �� ��������� ������ ����������
                turnTimer -= Time.deltaTime;
                // ������� ���������� �������� � �������������� Quaternion.RotateTowards
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 360f * Time.deltaTime);
                if (turnTimer <= 0f || Quaternion.Angle(transform.rotation, targetRotation) < 5f)
                {
                    // ����� ������� ��������, ��������� � ����� Retreat
                    currentState = CatState.Retreat;
                }
                break;

            case CatState.Retreat:
                if (distToPlayer > retreatExitRange)
                {
                    currentState = CatState.Idle;
                    idleTimer = idleTime;
                }
                break;
        }

        // ���������� �������� �� �������� ���������
        switch (currentState)
        {
            case CatState.Idle:
                HandleIdleState();
                break;
            case CatState.Wander:
                HandleWanderState();
                break;
            case CatState.Approach:
                HandleApproachState();
                break;
            case CatState.Turn:
                // � ��������� Turn �������� �� ������������ � ����� ���������������
                agent.speed = 0f;
                SetAnimatorParameters(0f, 0); // ����� �������������� Idle-��������
                break;
            case CatState.Retreat:
                HandleRetreatState();
                break;
        }
    }

    /// <summary>
    /// ����� ���������� � ��������: ��������� �������� ���������� (targetRotation) ���, ����� ��� ����� ������� � ������� �� ������,
    /// ������������� ��������� Turn � ��������� ������ ��������.
    /// </summary>
    void PrepareTurn()
    {
        // ��������� ����������� �� ������ � ����
        Vector3 retreatDirection = (transform.position - player.position).normalized;
        targetRotation = Quaternion.LookRotation(retreatDirection);
        currentState = CatState.Turn;
        turnTimer = turnDuration;
        // ����� followTimer, ���� ����������
        followTimer = 0f;
    }

    /// <summary>
    /// ��������� Idle � ��� ����� �� �����.
    /// </summary>
    void HandleIdleState()
    {
        SetAnimatorParameters(0f, 0);
        agent.speed = 0f;
        idleTimer -= Time.deltaTime;
        if (idleTimer <= 0)
        {
            currentState = CatState.Wander;
            ChooseNewWanderTarget();
        }
    }

    /// <summary>
    /// ��������� Wander � ��� �������� �������� �� �������.
    /// </summary>
    void HandleWanderState()
    {
        SetAnimatorParameters(1f, 0);
        agent.speed = wanderSpeed;
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            currentState = CatState.Idle;
            idleTimer = idleTime;
        }
    }

    /// <summary>
    /// ��������� Approach � ��� ��� � ������ (�����������, �������������).
    /// </summary>
    void HandleApproachState()
    {
        SetAnimatorParameters(1f, 0);
        agent.speed = approachSpeed;
        agent.SetDestination(player.position);
    }

    /// <summary>
    /// ��������� Retreat � ��� ������ ���������, ���� ����� ������� ������.
    /// </summary>
    void HandleRetreatState()
    {
        SetAnimatorParameters(1f, 1);
        agent.speed = retreatSpeed;
        // ������������ ����������� ��� ����������� (��� ��� ���� ���������� � PrepareTurn)
        Vector3 retreatDirection = (transform.position - player.position).normalized;
        Vector3 retreatDestination = transform.position + retreatDirection * 5f;
        agent.SetDestination(retreatDestination);
    }

    /// <summary>
    /// �������� ��������� ����� ��� ��������� � �������� wanderRadius.
    /// </summary>
    void ChooseNewWanderTarget()
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, NavMesh.AllAreas))
        {
            wanderTarget = hit.position;
            agent.SetDestination(wanderTarget);
        }
    }
}
