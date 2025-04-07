using UnityEngine;

public class TriggerHandler : MonoBehaviour
{
    public System.Action<GameObject, GameObject> OnObjectEnteredTrigger; // ������� ��� �������� ������

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log($"������ {other.name} ����� � ������� {gameObject.name}");
        OnObjectEnteredTrigger?.Invoke(gameObject, other.gameObject); // ����� �������
    }
}