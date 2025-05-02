using UnityEngine;
using GameResources;
using Zenject;

public enum ResourceType { Gold, Food, PeopleSatisfaction, CastleStrength }

[RequireComponent(typeof(Collider))]
public class InteractableItem : MonoBehaviour, ITriggerable
{
    public ResourceType resourceType;
    public int amount = 1;
    [TextArea] public string message = "+1 resource";
    public GameObject floatingTextPrefab;

    [Header("�������� ����� ������ ������")]
    [Tooltip("�������� �� ������� ������ �� �����, ��� ������ ��������� �����")]
    public Vector3 spawnOffset = new Vector3(0f, 2.5f, 0f);

    private Transform _player;

    [Inject]
    private void Construct(PlayerMoveController player)
    {
        _player = player.transform;
    }

    public void Interact()
    {
        // 1) �������
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
            // ������� ��������� ������������ ������� (����� ��������� ����� spawnOffset)
            Vector3 worldPos = _player.position + spawnOffset;
            // Instantiate(prefab, position, rotation, parent) � � ���� �����������
            // ��� ����� ����� ������ ������� _player � ��� ���� �������� ������� ������� worldPos
            GameObject ftGO = Instantiate(floatingTextPrefab, worldPos, Quaternion.identity, _player);

            // ���� ������, ����� ��� ���� ��������� ������� ���� ������ spawnOffset:
            // ftGO.transform.localPosition = spawnOffset;

            // ����� �����
            var ft = ftGO.GetComponent<FloatingText>();
            if (ft != null)
                ft.SetText(message);
        }

        // 3) ������� �������
        Destroy(gameObject);
    }

    public void Triggered() => Interact();
}
