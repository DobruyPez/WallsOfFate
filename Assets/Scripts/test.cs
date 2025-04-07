using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public float moveSpeed = 5f; 
    private float moveInputY;

    private void Start()
    {
        // ����� ����� � ����������� �� ��� �������
        Lever lever = FindObjectOfType<Lever>();
        if (lever != null)
        {
            lever.OnActivated += MoveUp;
        }
        else
        {
            Debug.LogWarning("Lever �� ������!");
        }
    }

    private void MoveUp()
    {
        // ������ �������� �����
        moveInputY = 20f;
        Vector3 move = new Vector3(0, moveInputY * moveSpeed * Time.deltaTime, 0);
        transform.Translate(move);
    }
}
