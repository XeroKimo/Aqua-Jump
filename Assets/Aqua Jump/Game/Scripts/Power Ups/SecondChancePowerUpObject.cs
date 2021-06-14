using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SecondChancePowerUp : BasePowerUp
{
    public SecondChancePowerUp(float time, Action onPowerUpEnded) : base(time, onPowerUpEnded)
    {

    }

}

public class SecondChancePowerUpObject : BasePowerUpObject
{
    protected override void OnPlayerEnter(Player player)
    {
        player.AddSecondChancePowerUp();
    }
}
