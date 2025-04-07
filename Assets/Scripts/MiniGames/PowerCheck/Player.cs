using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour 
{


    // ============================
    // ��������� ������� ���������� ������
    // ============================
    [Header("Player Settings")]
    [SerializeField] public string Name;           // ���
    [SerializeField] private uint MaxHealth;       // ������������ ��������
    [SerializeField] private uint Health;          // �������� � ������� �� ������������
    [SerializeField] private uint Damage;          // ����
    [SerializeField] private float Speed;          // ��������
    [SerializeField] private uint HealingAmount;   // ���������� �������


    //public Player(uint maxhealth,  uint health,  uint damage, float speed, uint healingAmount, string name)
    //{
    //    Name = name;
    //    MaxHealth = maxhealth;
    //    Health = health;
    //    Damage = damage;
    //    Speed = speed;
    //    HealingAmount = healingAmount;
    //}

    public uint GetHealth()
    {
        return this.Health;
    }

    public uint GetDamage()
    {
        return this.Damage;
    }

    public float GetSpeed()
    {
        return this.Speed;
    }

    public void TakeDamage(uint damage)
    {
        Debug.Log(this.Name + " ������� ������");
        this.Damage += damage;
    }

    public void TakeHeal()
    {
        Debug.Log(this.Name + " ���������");
        this.Health += HealingAmount;
        if (this.Health > MaxHealth)
        {
            this.Health = MaxHealth;
        }
    }

    public void TakeSpeedboost(float speed)
    {
        Debug.Log(this.Name + " ������� ���������� ��� � " + speed);
        this.Speed *= speed;
    }
}
