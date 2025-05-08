using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public class GameProcess : MonoBehaviour
{
    [SerializeField] private MineSpawner _mineSpawner;
    public GameObject Player;
    public GameObject Enemy;

    IReadOnlyList<Mine> _healMines;
    IReadOnlyList<Mine> _damageMines;
    IReadOnlyList<Mine> _buffMines;
    IReadOnlyList<Mine> _debuffMines;

    private PlayerMove PlayerMove;
    private AIController EnemyMove;

    public MiniGamePlayer PlayerChar;
    public MiniGamePlayer EnemyChar;

    [Inject]
    private void Construct([Inject(Id = "Player")] PlayerMove player, [Inject(Id = "Enemy")] AIController enemy)
    {
        UpdateReferences(player, enemy);
    }


    public void UpdateReferences(PlayerMove player, AIController enemy)
    {
        Player = player.gameObject;
        Enemy = enemy.gameObject;

        PlayerMove = player;
        EnemyMove = enemy;

        PlayerChar = Player.GetComponent<MiniGamePlayer>();
        EnemyChar = Enemy.GetComponent<MiniGamePlayer>();

        // Unsubscribe from previous events if they exist
        if (PlayerChar != null && PlayerMove != null)
        {
            PlayerChar.OnSpeedChanged -= PlayerMove.ChangeSpeed;
            PlayerChar.OnSpeedChanged += PlayerMove.ChangeSpeed;
        }
        if (EnemyChar != null && EnemyMove != null)
        {
            EnemyChar.OnSpeedChanged -= EnemyMove.ChangeSpeed;
            EnemyChar.OnSpeedChanged += EnemyMove.ChangeSpeed;
        }
    }

    public event Action<string, string> OnEndGame;

    private void OnEnable()
    {
        InitializeLogic();
    }

    private void InitializeLogic()
    {
        _mineSpawner = GameObject.FindGameObjectWithTag("MineSpawner").GetComponent<MineSpawner>();

        // �������� ������ ��� � MineSpawner
        _healMines = _mineSpawner.HealMines;
        _damageMines = _mineSpawner.DamageMines;
        _buffMines = _mineSpawner.BuffMines;
        _debuffMines = _mineSpawner.DebuffMines;

        // ��� ������ ���� ������������� �� �������
        SubscribeToMineEvents(_healMines);
        SubscribeToMineEvents(_damageMines);
        SubscribeToMineEvents(_buffMines);
        SubscribeToMineEvents(_debuffMines);
    }

    private void FixedUpdate()
    {
        SubscribeToMineEvents(_debuffMines);
        if ((PlayerChar.Health <= 0 && !PlayerChar.isDead) || (EnemyChar.Health <= 0 && !EnemyChar.isDead))
        {
            string winner, loser;
            if (PlayerChar.Health > 0)
            {
                winner = PlayerChar.Name;
                loser = EnemyChar.Name;
            }
            else
            {
                winner = EnemyChar.Name;
                loser = PlayerChar.Name;
            }

            PlayerChar.Health = 0;
            EnemyChar.Health = 0;
            PlayerChar.isDead = true;
            EnemyChar.isDead = true;

            OnEndGame?.Invoke(winner, loser);
        }
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
            return mine;
        }

        mine = FindMineInList(triggeredObject, _damageMines);
        if (mine != null)
        {
            return mine;
        }

        mine = FindMineInList(triggeredObject, _buffMines);
        if (mine != null)
        {
            return mine;
        }

        mine = FindMineInList(triggeredObject, _debuffMines);
        if (mine != null)
        {
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
        MiniGamePlayer playerChar = Player.GetComponent<MiniGamePlayer>();
        MiniGamePlayer enemyChar = Enemy.GetComponent<MiniGamePlayer>();

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
            MineExplosion(buffSpeedMine, Player, Enemy);
        }

        givedMine.SetActive(false);
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
    }
}