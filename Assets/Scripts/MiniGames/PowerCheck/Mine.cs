using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine 
{
    private uint Number;
    private float �ooldown;
    private GameObject MineGameObject;

    public Mine(uint number, float �ooldown, GameObject mine)
    {
        Number = number;
        �ooldown = �ooldown;
        MineGameObject = mine;
    }

    public uint GetNumber()
    {
        return this.Number;
    }

    public float GetCooldown()
    {
        return this.�ooldown;
    }

    public GameObject GetMine()
    {
        return this.MineGameObject;
    }

    public void SetActive(bool isActive)
    {
        if (MineGameObject != null)
        {
            MineGameObject.SetActive(isActive);
        }
        else
        {
            Debug.LogWarning("Mine GameObject is null. Cannot change active state.");
        }
    }
}
