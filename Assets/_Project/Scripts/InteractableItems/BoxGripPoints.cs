using UnityEngine;

/// <summary> ������ 4 �������� ���������� ������ �������. </summary>
public class BoxGripPoints : MonoBehaviour
{
    [Tooltip("Front, Back, Left, Right")]
    public Transform[] points = new Transform[4];
}
