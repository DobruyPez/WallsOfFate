using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine
{
    private uint number;            // private ����
    private float cooldown;         // private ����
    private GameObject mineGameObject;  // private ����
    private bool isFirst = true;    // private ����

    public Mine(uint number, float cooldown, GameObject mine)
    {
        this.number = number;
        this.cooldown = cooldown;
        this.mineGameObject = mine;
    }

    // ��������� �������� ��� ������� � �����
    public uint Number
    {
        get { return this.number; }
    }

    public float Cooldown
    {
        get { return this.cooldown; }
    }

    public GameObject MineGameObject
    {
        get { return this.mineGameObject; }
    }

    public bool IsFirstSpawn
    {
        get { return this.isFirst; }
    }

    public void SetActive(bool isActive)
    {
        if (mineGameObject != null)
        {
            mineGameObject.SetActive(isActive);
        }
        else
        {
            Debug.LogWarning("Mine GameObject is null. Cannot change active state.");
        }
    }
}
