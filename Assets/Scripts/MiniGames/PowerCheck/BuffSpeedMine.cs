using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffSpeedMine : Mine
{
    private float SpeedBuff;
    private float Time;

    public BuffSpeedMine(uint number, float �ooldown, GameObject mine, float speedbuff, float time/*, bool isdebuff*/) : base(number, �ooldown, mine)
    {
        this.SpeedBuff = speedbuff;
        this.Time = time;
    }

    private void BuffSpeed(Player player)
    {
        player.TakeSpeedboost(-this.SpeedBuff);
        float playerspeed = player.GetSpeed();
    }
}
