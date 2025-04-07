using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageMine : Mine
{
    public DamageMine(uint number, float �ooldown, GameObject mine) : base(number, �ooldown, mine) {}

    public void Damage(Player player1, Player player2)
    {
        //Debug.Log(player2.Name + " ���� " + player1.Name);
        player1.TakeDamage(player2.GetDamage());
    }
}
