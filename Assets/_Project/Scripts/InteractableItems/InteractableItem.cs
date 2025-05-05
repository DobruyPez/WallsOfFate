using UnityEngine;
using GameResources;
using Zenject;
using Zenject.SpaceFighter;

public enum ResourceType { Gold, Food, PeopleSatisfaction, CastleStrength }

[RequireComponent(typeof(Collider))]
public class InteractableItem : MonoBehaviour, ITriggerable
{
    public ResourceType resourceType;
    public int amount = 1;

    [TextArea]
    public string message = "+1 resource";

    public GameObject floatingTextPrefab;

    [Header("�������� ����� ������ ������")]
    [Tooltip("�������� �� ������� ������ �� �����, ��� ������ ��������� �����")]
    public Vector3 spawnOffset = new Vector3(0f, 2.5f, 0f);

    [Header("��������� ����� ��������������")]
    [Tooltip("���� true � ������ �������� ����� ��������������. ����� � ����������, �� ���������.")]
    public bool destroyAfterUse = true;

    private Transform _player;
    private bool _hasBeenUsed = false;
    public bool HasBeenUsed => _hasBeenUsed;


    private void Start()
    {
        if (!_player)
        {
            var go = GameObject.FindGameObjectWithTag("Player");
            if (go) _player = go.transform;
            else Debug.LogError("Player not found � ������� ������ ������ ��� 'Player'.");
        }
    }


    public void Interact()
    {
        if (_hasBeenUsed) return;

        _hasBeenUsed = true;

        // 1) �������� �������
        switch (resourceType)
        {
            case ResourceType.Gold:
                GameResources.GameResources.ChangeGold(amount);
                break;
            case ResourceType.Food:
                GameResources.GameResources.ChangeFood(amount);
                break;
            case ResourceType.PeopleSatisfaction:
                GameResources.GameResources.ChangePeopleSatisfaction(amount);
                break;
            case ResourceType.CastleStrength:
                GameResources.GameResources.ChangeCastleStrength(amount);
                break;
        }

        // 2) ������� �����
        if (floatingTextPrefab != null && _player != null)
        {
            Vector3 worldPos = _player.position + spawnOffset;
            GameObject ftGO = Instantiate(floatingTextPrefab, worldPos, Quaternion.identity, _player);
            var ft = ftGO.GetComponent<FloatingText>();
            if (ft != null)
                ft.SetText(message);
        }

        // 3) ������� ����� ��������������
        if (destroyAfterUse)
        {
            gameObject.SetActive(false);
        }
        else
        {
            // ��������� ��������� � (�� �������) ���������� ������ ���������
            Collider col = GetComponent<Collider>();
            if (col != null)
                col.enabled = false;

            // ���� ���� ��������� � ���������, ��������:
            var ps = GetComponentInChildren<ParticleSystem>();
            if (ps != null)
                ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }

        var outlines = GetComponentsInChildren<cakeslice.Outline>();
        foreach (var outline in outlines)
        {
            outline.enabled = false;
        }
    }

    public void Triggered() => Interact();
}
