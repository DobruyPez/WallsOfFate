using System;
using UnityEngine;

public class MiniGamePlayer : MonoBehaviour
{
    // ============================
    // ��������� ������� ���������� ������
    // ============================
    [SerializeField] private string playerName;  // ��� ������
    [SerializeField] private uint maxHealth;     // ������������ ��������
    [SerializeField] private uint health;        // ������� ��������
    [SerializeField] private uint damage;        // ����
    [SerializeField] private float speed;        // ��������
    [SerializeField] private uint healingAmount; // ���������� �������

    public string Name
    {
        get => playerName;
        set => playerName = value;
    }

    public uint MaxHealth => maxHealth;
    public uint Health => health;
    public uint Damage => damage;

    public float Speed
    {
        get => speed;
        set
        {
            //Debug.Log($"Setting speed: {value}, current speed: {speed}");
            if (Math.Abs(speed - value) > 0.01f)
            {
                speed = value;
                OnSpeedChanged?.Invoke(speed); // �������� ������� ��� ��������� ��������
            }
        }
    }

    public uint HealingAmount => healingAmount;

    public event Action<float> OnSpeedChanged;

    // Unity-����� ��� ��������� �������������
    private void Start()
    {
        //Debug.Log($"{Name} ������ � {health} �������� � ��������� {speed}");
    }

    public void Initialize(string playerName, uint maxHp, uint startHealth, uint dmg, float initialSpeed, uint healAmount)
    {
        Name = playerName;
        maxHealth = maxHp;
        health = startHealth;
        damage = dmg;
        speed = initialSpeed;
        healingAmount = healAmount;
    }

    public void TakeDamage(uint damage)
    {
        health = health >= damage ? health - damage : 0;
        Debug.Log($"{Name} ������� ����. ��������: {health}");
    }

    public void TakeHeal()
    {
        health += healingAmount;
        if (health > maxHealth) health = maxHealth;
        //Debug.Log($"{Name} ���������. ��������: {health}");
    }

    public void TakeSpeedboost(float speedMultiplier)
    {
        //Debug.Log($"{Name} ������� ���������� ��� {speedMultiplier}");
        Speed = (float)speedMultiplier; // �������� ��������, ������� �������
        //Debug.Log($"{Name} ����� �������� {Speed}");
    }
}
