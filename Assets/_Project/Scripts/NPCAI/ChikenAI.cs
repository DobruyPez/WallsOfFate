using UnityEngine;
using UnityEngine.AI;

public class ChickenAI : MonoBehaviour
{
    public float detectionRange = 5f;    // ������, ��� ������� ������ �������� �������
    public float fleeSpeed = 3.5f;         // �������� ��� ��������
    public float wanderSpeed = 1.5f;       // �������� ��� ���������
    public float idleTime = 2f;            // ����� ������� � ��������� Idle
    public float wanderRadius = 10f;       // ������ ������ ��������� ����� ��� ���������

    private NavMeshAgent agent;
    private Animator animator;

    // ������ �� ������. ����� ��������� ����� ��������� ��� ���������� �� ���� "Player" � ������ Start.
    public Transform player;

    // ���������� ��������� ������
    private enum ChickenState { Idle, Wander, Flee }
    private ChickenState currentState;

    private float idleTimer;
    private Vector3 wanderTarget;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // ���� ������ �� ������ �� ����������� ����� ���������, ���� ������ � ����� "Player"
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
            else
            {
                Debug.LogError("����� �� ������! ���������� ��� 'Player' ��� ������� ������ ��� ������� ������ � ����������.");
            }
        }

        // ��������� ��������� � Idle
        currentState = ChickenState.Idle;
        idleTimer = idleTime;
        SetAnimatorParameters(0f, 0);
    }

    void Update()
    {
        // ��������� ������ �� ������
        CheckForThreats();

        switch (currentState)
        {
            case ChickenState.Idle:
                HandleIdleState();
                break;
            case ChickenState.Wander:
                HandleWanderState();
                break;
            case ChickenState.Flee:
                HandleFleeState();
                break;
        }
    }

    /// <summary>
    /// ������������� ��������� ��������� �������� �������:
    /// - Idle: Vert = 0, State = 0
    /// - Walk: Vert = 1, State = 0
    /// - Run:  Vert = 1, State = 1
    /// </summary>
    private void SetAnimatorParameters(float vert, int state)
    {
        animator.SetFloat("Vert", vert);
        animator.SetInteger("State", state);
    }

    private void HandleIdleState()
    {
        // Idle: ������ ����� �� �����
        SetAnimatorParameters(0f, 0);
        agent.speed = 0f; // ����� �� ��������� � Idle

        idleTimer -= Time.deltaTime;
        if (idleTimer <= 0)
        {
            // ������� � ���������
            currentState = ChickenState.Wander;
            ChooseNewWanderTarget();
        }
    }

    private void HandleWanderState()
    {
        // ��������� (Walk):
        SetAnimatorParameters(1f, 0);
        agent.speed = wanderSpeed;

        // ���� ������ �������� ���� ���������
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            currentState = ChickenState.Idle;
            idleTimer = idleTime;
        }
    }

    private void HandleFleeState()
    {
        // �������� (Run):
        SetAnimatorParameters(1f, 1);
        agent.speed = fleeSpeed;

        // ��������� ����������� ��� �������� �� ������ (������)
        Vector3 fleeDirection = GetFleeDirection();
        Vector3 fleeDestination = transform.position + fleeDirection * 5f; // ����� �����
        agent.SetDestination(fleeDestination);

        // ���� ����� ������� ���������� ������, ������������ � Idle
        if (Vector3.Distance(transform.position, player.position) > detectionRange)
        {
            currentState = ChickenState.Idle;
            idleTimer = idleTime;
        }
    }

    /// <summary>
    /// �������� ��������� ����� ��� ��������� � �������� ��������� �������.
    /// </summary>
    private void ChooseNewWanderTarget()
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

    /// <summary>
    /// �������� ������: ���� ����� ��������� �����, ��� detectionRange, ����������� � ����� Flee.
    /// </summary>
    private void CheckForThreats()
    {
        if (player == null)
            return;

        float distToPlayer = Vector3.Distance(transform.position, player.position);

        if (distToPlayer < detectionRange)
        {
            currentState = ChickenState.Flee;
        }
    }

    /// <summary>
    /// ���������� ��������������� ����������� �� ������ � ������.
    /// </summary>
    private Vector3 GetFleeDirection()
    {
        if (player == null)
            return Vector3.zero;

        Vector3 direction = transform.position - player.position;
        direction.Normalize();
        return direction;
    }
}
