﻿using Assets.Scripts.MiniGames.PowerCheck.GridCoordinates;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public class MineSpawner : MonoBehaviour
{
    // =====================================================================
    // Spawn area settings
    // =====================================================================
    [Header("Spawn Area Settings")]
    [SerializeField] private Transform CenterPoint;
    public Vector2 spawnAreaSize = new Vector2(10, 10);

    [SerializeField] private Transform parentTransform;
    [SerializeField] private List<Transform> _forbiddenSpawnPoints;
    [SerializeField, Tooltip("Минимальная дистанция до запрещённых точек (игрок, враг и т.д.) при спавне")] private float allowedDistanseForForrbidenSpawnPoint = 2f;
    [SerializeField, Tooltip("Y‑координата, на которой появляются мины")] private float yPositionOfSpawnMine = 0f;

    [Header("Proximity Rules")]
    [SerializeField, Tooltip("Минимальная дистанция между любыми двумя минами при спавне")] private float minDistanceBetweenMines = 1.25f;

    // =====================================================================
    // Spawn visual
    // =====================================================================
    [Header("Spawn Animation")]
    [SerializeField, Tooltip("Длительность анимации появления (сек.)")] private float spawnAnimationDuration = 0.35f;

    // =========================================
    // Prefab scale cache
    // =========================================
    private Dictionary<Mine, Vector3> _originalScales = new Dictionary<Mine, Vector3>();

    // =====================================================================
    // Mine prefabs
    // =====================================================================
    [Header("Mine Prefabs")]
    [SerializeField] private GameObject HealMinePrefab;
    [SerializeField] private GameObject DamageMinePrefab;
    [SerializeField] private GameObject BuffMinePrefab;
    [SerializeField] private GameObject DebuffMinePrefab;

    // =====================================================================
    // Per‑type spawn rules
    // =====================================================================
    [System.Serializable]
    private class MineSpawnConfig
    {
        public int initialCount;
        public int maxCount;
        public Vector2 spawnIntervalRange;
        public float RandomInterval => Random.Range(spawnIntervalRange.x, spawnIntervalRange.y);
    }

    [Header("Heal mines rules")]
    [SerializeField] private MineSpawnConfig healConfig = new MineSpawnConfig { initialCount = 3, maxCount = 6, spawnIntervalRange = new Vector2(8f, 12f) };
    [Header("Damage mines rules")]
    [SerializeField] private MineSpawnConfig damageConfig = new MineSpawnConfig { initialCount = 4, maxCount = 8, spawnIntervalRange = new Vector2(5f, 10f) };
    [Header("Speed‑buff mines rules")]
    [SerializeField] private MineSpawnConfig buffConfig = new MineSpawnConfig { initialCount = 2, maxCount = 4, spawnIntervalRange = new Vector2(12f, 18f) };
    [Header("Trap (debuff) mines rules")]
    [SerializeField] private MineSpawnConfig debuffConfig = new MineSpawnConfig { initialCount = 10, maxCount = 30, spawnIntervalRange = new Vector2(1f, 4f) };

    // =====================================================================
    // Specific parameters
    // =====================================================================
    [SerializeField] private float healCooldown;
    [SerializeField] private float damageCooldown;
    [SerializeField] private float speedBufCooldown;
    [SerializeField] private float speedBuf;
    [SerializeField] private float buffTime;
    [SerializeField] private int buffTimeBeforeExplosion;
    [SerializeField] private float buffRadiusOfExplosion;
    [SerializeField] private uint buffDamage;
    [SerializeField] private float speedDebufCooldown;
    [SerializeField] private float speedDebuf;
    [SerializeField] private float debuffTime;
    [SerializeField] private int debuffTimeBeforeExplosion;
    [SerializeField] private float debuffRadiusOfExplosion;
    [SerializeField] private uint debuffDamage;

    // =====================================================================
    // Mine lists
    // =====================================================================
    private MineList healMineList;
    private MineList damageMineList;
    private MineList buffMineList;
    private MineList debuffMineList;

    private GridCordinates _gridCordinates;

    public IReadOnlyList<Mine> HealMines => healMineList.Minelist;
    public IReadOnlyList<Mine> DamageMines => damageMineList.Minelist;
    public IReadOnlyList<Mine> BuffMines => buffMineList.Minelist;
    public IReadOnlyList<Mine> DebuffMines => debuffMineList.Minelist;

    [Inject]
    private void Construct([Inject(Id = "Player")] PlayerMove player,
                           [Inject(Id = "Enemy")] AIController enemy)
    {
        _forbiddenSpawnPoints.Add(player.transform);
        _forbiddenSpawnPoints.Add(enemy.transform);
    }

    //private void Awake()
    //{
    //    InitializeMines();
    //}

    private void InitializeMines()
    {
        if (healMineList == null) healMineList = new MineList(healConfig.maxCount);
        if (damageMineList == null) damageMineList = new MineList(damageConfig.maxCount);
        if (buffMineList == null) buffMineList = new MineList(buffConfig.maxCount);
        if (debuffMineList == null) debuffMineList = new MineList(debuffConfig.maxCount);

        healMineList.InitializeMines(HealMinePrefab, healCooldown, (n, c, go) => new HealMine(n, c, go));
        damageMineList.InitializeMines(DamageMinePrefab, damageCooldown, (n, c, go) => new DamageMine(n, c, go));
        buffMineList.InitializeSpeedBuffMines(BuffMinePrefab, speedBufCooldown, speedBuf, buffTime, buffTimeBeforeExplosion, buffRadiusOfExplosion, buffDamage, false);
        debuffMineList.InitializeSpeedBuffMines(DebuffMinePrefab, speedDebufCooldown, speedDebuf, debuffTime, debuffTimeBeforeExplosion, debuffRadiusOfExplosion, debuffDamage, true);

        CacheOriginalScales();
    }

    public void ClearAllMineObjects()
    {
        // Удаляем все объекты для каждого типа мин
        ClearMineObjects(healMineList);
        ClearMineObjects(damageMineList);
        ClearMineObjects(buffMineList);
        ClearMineObjects(debuffMineList);

        // Очищаем кеш масштабов
        _originalScales.Clear();
    }

    private void ClearMineObjects(MineList mineList)
    {
        foreach (var mine in mineList.Minelist)
        {
            if (mine.MineGameObject != null)
            {
                Destroy(mine.MineGameObject);
            }
        }
        mineList.Minelist.Clear();
    }

    private void CacheOriginalScales()
    {
        foreach (var mine in EnumerateAllMines())
            _originalScales[mine] = mine.MineGameObject.transform.localScale;
    }

    private void OnEnable()
    {
        InitializeMines();
        StartingSpawn();
    }

    private void OnDisable()
    {
        ClearAllMineObjects();
        StopAllCoroutines(); // Останавливаем все корутины спавна
    }

    private void Start()
    {
        StartingSpawn();
       
    }

    private void StartingSpawn()
    {
        SpawnInitialMines(healMineList, 0, healConfig.initialCount);
        SpawnInitialMines(damageMineList, 1, damageConfig.initialCount);
        SpawnInitialMines(buffMineList, 2, buffConfig.initialCount);
        SpawnInitialMines(debuffMineList, 3, debuffConfig.initialCount);

        StartCoroutine(SpawnRoutine(healMineList, 0, healConfig));
        StartCoroutine(SpawnRoutine(damageMineList, 1, damageConfig));
        StartCoroutine(SpawnRoutine(buffMineList, 2, buffConfig));
        StartCoroutine(SpawnRoutine(debuffMineList, 3, debuffConfig));
    }

    private IEnumerator SpawnRoutine(MineList list, uint typeId, MineSpawnConfig cfg)
    {
        yield return null;
        while (true)
        {
            yield return new WaitForSeconds(cfg.RandomInterval);

            int activeCount = CountActiveMines(list);
            if (typeId == 3 && activeCount >= cfg.maxCount)
            {
                // при достижении лимита ловушек — удаляем одну и спавним новую
                Mine toRemove = FindActiveMine(list);
                if (toRemove != null)
                {
                    toRemove.SetActive(false);
                }
            }
            else if (activeCount >= cfg.maxCount)
            {
                continue;
            }

            Mine candidate = FindInactiveMine(list);
            if (candidate == null && list.Minelist.Count < cfg.maxCount)
            {
                AddMineByType(typeId);
                candidate = list.Minelist[^1];
            }
            if (candidate != null && !candidate.MineGameObject.activeSelf)
            {
                SpawnMineAnimated(candidate);
            }
        }
    }

    private void SpawnInitialMines(MineList list, uint typeId, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Mine m = FindInactiveMine(list);
            if (m == null)
            {
                AddMineByType(typeId);
                m = list.Minelist[^1];
            }
            if (m != null) SpawnMineAnimated(m);
        }
    }

    private void SpawnMineAnimated(Mine mine)
    {
        Vector3 pos = FindValidPosition();
        mine.MineGameObject.transform.position = pos;
        if (parentTransform != null) mine.MineGameObject.transform.SetParent(parentTransform);

        Vector3 targetScale = _originalScales.TryGetValue(mine, out var s) ? s : Vector3.one;
        mine.MineGameObject.transform.localScale = Vector3.zero;
        mine.SetActive(true);
        StartCoroutine(AnimateScale(mine.MineGameObject.transform, targetScale));
    }

    private IEnumerator AnimateScale(Transform t, Vector3 to)
    {
        float elapsed = 0f;
        while (elapsed < spawnAnimationDuration)
        {
            elapsed += Time.deltaTime;
            float p = Mathf.Clamp01(elapsed / spawnAnimationDuration);
            t.localScale = Vector3.Lerp(Vector3.zero, to, EasingOutBack(p));
            yield return null;
        }
        t.localScale = to;
    }
    private float EasingOutBack(float t)
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
    }

    private Mine FindActiveMine(MineList list)
    {
        foreach (var m in list.Minelist)
            if (m.MineGameObject.activeSelf) return m;
        return null;
    }

    private Vector3 FindValidPosition()
    {
        Vector3 cand;
        int safe = 0;
        do
        {
            int x = Mathf.RoundToInt(Random.Range(CenterPoint.position.x - spawnAreaSize.x * 0.5f, CenterPoint.position.x + spawnAreaSize.x * 0.5f));
            int z = Mathf.RoundToInt(Random.Range(CenterPoint.position.z - spawnAreaSize.y * 0.5f, CenterPoint.position.z + spawnAreaSize.y * 0.5f));
            cand = new Vector3(x, yPositionOfSpawnMine, z);
            safe++;
        }
        while (!IsPositionFree(cand) && safe < 1000);
        return cand;
    }

    private bool IsPositionFree(Vector3 pos)
    {
        foreach (var f in _forbiddenSpawnPoints)
            if (Vector3.Distance(pos, f.position) < allowedDistanseForForrbidenSpawnPoint) return false;
        foreach (var m in EnumerateAllMines())
            if (m.MineGameObject.activeSelf && Vector3.Distance(pos, m.MineGameObject.transform.position) < minDistanceBetweenMines)
                return false;
        return true;
    }

    private IEnumerable<Mine> EnumerateAllMines()
    {
        foreach (var m in healMineList.Minelist) yield return m;
        foreach (var m in damageMineList.Minelist) yield return m;
        foreach (var m in buffMineList.Minelist) yield return m;
        foreach (var m in debuffMineList.Minelist) yield return m;
    }

    private int CountActiveMines(MineList list)
    {
        int c = 0;
        foreach (var m in list.Minelist) if (m.MineGameObject.activeSelf) c++;
        return c;
    }

    private Mine FindInactiveMine(MineList list)
    {
        foreach (var m in list.Minelist) if (!m.MineGameObject.activeSelf) return m;
        return null;
    }

    private void AddMineByType(uint type)
    {
        switch (type)
        {
            case 0: healMineList.AddMine(HealMinePrefab, healCooldown, (n, cd, go) => new HealMine(n, cd, go)); break;
            case 1: damageMineList.AddMine(DamageMinePrefab, damageCooldown, (n, cd, go) => new DamageMine(n, cd, go)); break;
            case 2:
                buffMineList.AddMine(BuffMinePrefab, speedBufCooldown, speedBuf, buffTime, buffTimeBeforeExplosion, buffRadiusOfExplosion, buffDamage,
                        (n, cd, go, s, cd2, tbe, r, d) => new BuffSpeedMine(n, cd, go, s, cd2, tbe, r, d, false)); break;
            case 3:
                debuffMineList.AddMine(DebuffMinePrefab, speedDebufCooldown, speedDebuf, debuffTime, debuffTimeBeforeExplosion, debuffRadiusOfExplosion, debuffDamage,
                        (n, cd, go, s, cd2, tbe, r, d) => new BuffSpeedMine(n, cd, go, s, cd2, tbe, r, d, true)); break;
        }
    }
}
