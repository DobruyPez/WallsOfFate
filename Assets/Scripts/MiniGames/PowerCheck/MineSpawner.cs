using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineSpawner : MonoBehaviour
{
    // ============================
    // ��������� ������ � �������
    // ============================
    [Header("Spawn Settings")]
    [SerializeField] private Transform CenterPoint;                         // ����� ������ ���
    [SerializeField] private Vector2 spawnAreaSize = new Vector2(10, 10);   // ������ ������� ������
    [SerializeField] private Transform parentTransform;                     // ������������ ������ ��� ���
    [SerializeField] private List<Transform> forbiddenSpawnPoints;          // �����, ��� ����� ��������
    [SerializeField] private float allowedDistanseForForrbidenSpawnPoint;   // ��������� �� ����� ��� ����� ��������
    [SerializeField] private bool onTestSpawn;                              // ��������� �������� �������� ������ ��� ��� ������� ���������� ��������
    [SerializeField] private bool onTestProgressionSpawn;                   // ��������� �������� �������� ������ ��� � ����������
    [SerializeField] private float yPositionOfSpawnMine;                    // ������  �� ������� ���� ���������� ����

    // ============================
    // ������� ��� ���
    // ============================
    [Header("Mine Prefabs")]
    [SerializeField] private GameObject HealMinePrefab;     // ������ ���� �������
    [SerializeField] private GameObject DamageMinePrefab;   // ������ ���� �����
    [SerializeField] private GameObject BuffMinePrefab;     // ������ ���� �������� ��������
    [SerializeField] private GameObject DebuffMinePrefab;   // ������ ���� ���������� ��������

    // ============================
    // ��������� ��� ���� �������
    // ============================
    [Header("Heal Mine Settings")]
    [SerializeField] private float healCooldown = 1.0f; // ����� ����������� ���� �������
    [SerializeField] private int numberOfHealMines = 5; // ���������� ���, ������� ����� ���������� �����


    // ============================
    // ��������� ��� ���� �����
    // ============================
    [Header("Damage Mine Settings")]
    [SerializeField] private float damageCooldown = 1.0f; // ����� ����������� ���� �����
    [SerializeField] private int numberOfDamageMines = 5; // ���������� ���, ������� ����� ���������� �����


    // ============================
    // ��������� ��� ���� �������� ��������
    // ============================
    [Header("Speed Buff Mine Settings")]
    [SerializeField] private float speedBufCooldown = 1.0f; // ����� ����������� ���� �������� ��������
    [SerializeField] private float speedBuf = 1.0f;         // ��������� �������� ��������
    [SerializeField] private float buffTime = 2.0f;         // ����� �������� �������� ��������
    [SerializeField] private int numberOfBuffMines = 5;     // ���������� ���, ������� ����� ���������� �����
    [SerializeField] private int buffTimeBeforeExplosion = 5;     // ����� �� ������(��������� ��������)
    [SerializeField] private float buffRadiusOfExplosion = 5;     // ������ ������

    // ============================
    // ��������� ��� ���� ���������� ��������
    // ============================
    [Header("Speed Debuff Mine Settings")]
    [SerializeField] private float speedDebufCooldown = 1.0f; // ����� ����������� ���� ���������� ��������
    [SerializeField] private float speedDebuf = 1.0f;         // ��������� ���������� ��������
    [SerializeField] private float debuffTime = 5.0f;         // ����� �������� ���������� ��������
    [SerializeField] private int numberOfDebuffMines = 5;     // ���������� ���, ������� ����� ���������� �����
    [SerializeField] private int debuffTimeBeforeExplosion = 5;     // ����� �� ������(��������� ��������)
    [SerializeField] private float debuffRadiusOfExplosion = 5;     // ������ ������

    // ============================
    // ������ ��� ������� ���� ���
    // ============================
    private MineList healMineList;
    private MineList damageMineList;
    private MineList buffMineList;
    private MineList debuffMineList;

    public IReadOnlyList<Mine> HealMines => healMineList.Minelist;
    public IReadOnlyList<Mine> DamageMines => damageMineList.Minelist;
    public IReadOnlyList<Mine> BuffMines => buffMineList.Minelist;
    public IReadOnlyList<Mine> DebuffMines => debuffMineList.Minelist;

    void Awake()
    {
        healMineList = new MineList(numberOfHealMines);
        damageMineList = new MineList(numberOfDamageMines);
        buffMineList = new MineList(numberOfBuffMines);
        debuffMineList = new MineList(numberOfDebuffMines);
       
        healMineList.InitializeMines(HealMinePrefab, healCooldown, (number, cooldown, mineGameObject) => new HealMine(number, cooldown, mineGameObject));
        damageMineList.InitializeMines(DamageMinePrefab, damageCooldown, (number, cooldown, mineGameObject) => new DamageMine(number, cooldown, mineGameObject));
        buffMineList.InitializeSpeedBuffMines(BuffMinePrefab, speedBufCooldown, speedBuf, buffTime, buffTimeBeforeExplosion, buffRadiusOfExplosion);
        debuffMineList.InitializeSpeedBuffMines(DebuffMinePrefab, speedDebufCooldown, speedDebuf, debuffTime, debuffTimeBeforeExplosion, debuffRadiusOfExplosion);

    }

    private void Update()
    {
        if(onTestSpawn) SpawnMines();
        if (onTestProgressionSpawn) StartCoroutine(AddAndSpawnMines(2, 3));
    }

    // ��������� ���������� ���������� ��� � ������ ����������� ����� ������.
    private void AddForbiddenSpawnPoints(params Mine[] mines)
    {
        foreach (var mine in mines)
        {
            if (mine != null)
            {
                Transform mineTransform = mine.GetMine().transform;
                if (!forbiddenSpawnPoints.Contains(mineTransform))
                {
                    forbiddenSpawnPoints.Add(mineTransform);
                }
            }
        }
    }
    private void AddForbiddenSpawnPoints(List<Mine> mines)
    {
        foreach (var mine in mines)
        {
            if (mine != null)
            {
                Transform mineTransform = mine.GetMine().transform;
                if (!forbiddenSpawnPoints.Contains(mineTransform))
                {
                    forbiddenSpawnPoints.Add(mineTransform);
                }
            }
        }
    }

    public void SpawnMines()
    {
        SpawnMinesFromList(healMineList);

        SpawnMinesFromList(damageMineList);

        SpawnMinesFromList(buffMineList);

        SpawnMinesFromList(debuffMineList);
    }

    public void SpawnMinesFromList(MineList mineList)
    {
        foreach (Mine mine in mineList.Minelist)
        {
            SpawnMine(mine);
        }
    }

    public IEnumerator AddAndSpawnMines(int numOfMines, uint typeOfMine, float delayBetweenSpawns = 0.5f)
    {
        Mine newMine = null;
        while (numOfMines > 0)
        {
            switch (typeOfMine)
            {
                case 0:
                    newMine = this.healMineList.AddMine(HealMinePrefab, healCooldown,
                        (number, cooldown, mineGameObject) => new HealMine(number, cooldown, mineGameObject));
                    this.SpawnMine(newMine);
                    break;

                case 1:
                    newMine = this.damageMineList.AddMine(DamageMinePrefab, damageCooldown,
                        (number, cooldown, mineGameObject) => new DamageMine(number, cooldown, mineGameObject));
                    this.SpawnMine(newMine);
                    break;

                case 2:
                    newMine = this.buffMineList.AddMine(BuffMinePrefab, speedBufCooldown, speedBuf, buffTime, buffTimeBeforeExplosion, buffRadiusOfExplosion,
                        (number, cooldown, mineGameObject, speedbuff, buffcooldown, buffTimeBeforeExplosion, buffRadiusOfExplosion) => new BuffSpeedMine(number, cooldown, mineGameObject, speedbuff, buffcooldown, buffTimeBeforeExplosion, buffRadiusOfExplosion));
                    this.SpawnMine(newMine);
                    break;

                case 3:
                    newMine = this.debuffMineList.AddMine(DebuffMinePrefab, speedDebufCooldown, speedDebuf, debuffTime, buffTimeBeforeExplosion, buffRadiusOfExplosion,
                        (number, cooldown, mineGameObject, speedbuff, buffcooldown, buffTimeBeforeExplosion, buffRadiusOfExplosion) => new BuffSpeedMine(number, cooldown, mineGameObject, speedbuff, buffcooldown, buffTimeBeforeExplosion, buffRadiusOfExplosion));
                    this.SpawnMine(newMine);
                    break;
            }

            numOfMines--;

            // �������� ����� ��������� ��������� ����
            if (delayBetweenSpawns > 0)
                yield return new WaitForSeconds(delayBetweenSpawns);
            else
                yield return null;
        }
    }


    public void SpawnMine(Mine mine)
    {
        StartCoroutine(SpawnMineWithDelay(mine));
    }

    private IEnumerator SpawnMineWithDelay(Mine mine)
    {
        this.AddForbiddenSpawnPoints(healMineList.Minelist);
        this.AddForbiddenSpawnPoints(damageMineList.Minelist);
        this.AddForbiddenSpawnPoints(buffMineList.Minelist);
        this.AddForbiddenSpawnPoints(debuffMineList.Minelist);

        yield return new WaitForSeconds(mine.GetCooldown());

        Vector3 randomPosition;
        bool positionValid;
        int numOfIterations = 0;

        do
        {
            float xPos = Random.Range(CenterPoint.position.x - spawnAreaSize.x / 2, CenterPoint.position.x + spawnAreaSize.x / 2);
            float zPos = Random.Range(CenterPoint.position.z - spawnAreaSize.y / 2, CenterPoint.position.z + spawnAreaSize.y / 2);

            randomPosition = new Vector3(xPos, yPositionOfSpawnMine, zPos);
            positionValid = true;

            foreach (Transform forbiddenPoint in forbiddenSpawnPoints)
            {
                numOfIterations++;
                if (Vector3.Distance(randomPosition, forbiddenPoint.position) < allowedDistanseForForrbidenSpawnPoint) 
                {
                    positionValid = false;
                    break;
                }
            }
        } while (!positionValid && numOfIterations < 1000000);

        if (numOfIterations < 100000)
        {
            mine.GetMine().transform.position = randomPosition;

            if (parentTransform != null)
            {
                mine.GetMine().transform.SetParent(parentTransform);
            }

            mine.SetActive(true);
        }

    }

}
