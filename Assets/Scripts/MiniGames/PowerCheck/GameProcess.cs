using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameProcess : MonoBehaviour
{
    [SerializeField] private MineSpawner mineSpawner;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject enemy;

    // 
    // ��������� ������� ������============================
    // ============================
    [Header("Game Rules")]

    IReadOnlyList<Mine> _healMines;
    IReadOnlyList<Mine> _damageMines;
    IReadOnlyList<Mine> _buffMines;
    IReadOnlyList<Mine> _debuffMines;

    private PlayerMove _playerMove;
    private AIController _enemyMove;

    void Start()
    {
        _playerMove = player.GetComponent<PlayerMove>();
        _enemyMove = enemy.GetComponent<AIController>();

        MiniGamePlayer playerChar = player.GetComponent<MiniGamePlayer>();
        MiniGamePlayer enemyChar = enemy.GetComponent<MiniGamePlayer>();

        playerChar.OnSpeedChanged += _playerMove.ChangeSpeed;
        enemyChar.OnSpeedChanged += _enemyMove.ChangeSpeed;

        // �������� ������ ��� � MineSpawner
        _healMines = mineSpawner.HealMines;
        _damageMines = mineSpawner.DamageMines;
        _buffMines = mineSpawner.BuffMines;
        _debuffMines = mineSpawner.DebuffMines;

        // ��� ������ ���� ������������� �� �������
        SubscribeToMineEvents(_healMines);
        SubscribeToMineEvents(_damageMines);
        SubscribeToMineEvents(_buffMines);
        SubscribeToMineEvents(_debuffMines);
        //mineSpawner.SpawnMines();
    }

    private void FixedUpdate()
    {
        SubscribeToMineEvents(_debuffMines);

    }

    private void SubscribeToMineEvents(IEnumerable<Mine> mines)
    {
        foreach (Mine mine in mines)
        {
            // �������� ������ � ��������� TriggerHandler
            GameObject minePrefab = mine.MineGameObject;
            TriggerHandler mineTriggerHandler = minePrefab.GetComponent<TriggerHandler>();

            if (mineTriggerHandler != null)
            {
                // ������������� �� ������� OnMineTriggered
                mineTriggerHandler.OnObjectEnteredTrigger += (triggeredObject, objectWhoTriger) =>
                {
                    HandleTriggeredObject(triggeredObject, objectWhoTriger);
                };
            }
        }
    }

    private void HandleTriggeredObject(GameObject triggeredObject, GameObject objectWhoTriger)
    {
        // ����������, � ����� ��������� ��� ����������� ������
        Mine mine = FindMineByGameObject(triggeredObject);

        if (mine != null)
        {
            //Debug.Log($"���� � ������� {mine.GetNumber()} ������� �������.");
            HandleMineTriggered(mine, objectWhoTriger);
        }
        else
        {
            Debug.LogWarning($"�� ������� ����������, � ����� ���� ����������� ������ {triggeredObject.name}.");
        }
    }

    private Mine FindMineByGameObject(GameObject triggeredObject)
    {
        // ��������� �� ���� �������
        Mine mine = FindMineInList(triggeredObject, _healMines);
        if (mine != null)
        {
            //Debug.Log("��� ���� ����");
            return mine;
        }

        mine = FindMineInList(triggeredObject, _damageMines);
        if (mine != null)
        {
            //Debug.Log("��� ���� ������");
            return mine;
        }

        mine = FindMineInList(triggeredObject, _buffMines);
        if (mine != null)
        {
            //Debug.Log("��� ���� ���������");
            return mine;
        }

        mine = FindMineInList(triggeredObject, _debuffMines);
        if (mine != null)
        {
            //Debug.Log("��� ���� ����������");
            return mine;
        }

        // ���� �� �������, ���������� null
        return null;
    }

    private Mine FindMineInList(GameObject triggeredObject, IEnumerable<Mine> mines)
    {
        foreach (Mine mine in mines)
        {
            if (mine.MineGameObject == triggeredObject)
            {
                return mine;
            }
        }

        return null;
    }


    private void HandleMineTriggered(Mine givedMine, GameObject givedPlayer)
    {
        MiniGamePlayer givedPlayerChar = givedPlayer.GetComponent<MiniGamePlayer>();
        MiniGamePlayer playerChar = player.GetComponent<MiniGamePlayer>();
        MiniGamePlayer enemyChar = enemy.GetComponent<MiniGamePlayer>();
        

        if (givedMine is HealMine healMine)
        {
            healMine.Heal(givedPlayerChar);
        }
        else if (givedMine is DamageMine damageMine)
        {
            if (givedPlayerChar.Name == "Player")
                damageMine.Damage(enemyChar, playerChar);
            else
                damageMine.Damage(playerChar, enemyChar);
        }
        else if (givedMine is BuffSpeedMine buffSpeedMine)
        {
            MineExplosion(buffSpeedMine, player, enemy);
        }

        givedMine.SetActive(false); 
        //mineSpawner.SpawnMine(givedMine);
    }

    private async void MineExplosion(BuffSpeedMine mine, params GameObject[] objects)
    {
        Vector3 initialMinePosition = mine.MineGameObject.transform.position;
        
        // ���� ����� � 3 �������
        await Task.Delay(mine.GetTimeBeforeExplosion());

        // ������� ��� ������� �� ������������ ���������� �� ����
        List<MiniGamePlayer> affectedPlayers = mine.FindDistanceToMine(initialMinePosition, objects);

        // �������� ��������� ������� � ����� BuffSpeedList
        await mine.BuffSpeedList(affectedPlayers);

        //Debug.Log("Mine explosion completed, buff applied to nearby players.");
    }
}
