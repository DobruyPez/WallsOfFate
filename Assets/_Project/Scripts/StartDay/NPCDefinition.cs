using UnityEngine;

[CreateAssetMenu(menuName = "Audience/NPC")]
public class NPCDefinition : ScriptableObject
{
    public string npcName;           // ��� DialogeTrigger
    public GameObject prefab;        // ������ � NavMeshAgent + DialogeTrigger
}