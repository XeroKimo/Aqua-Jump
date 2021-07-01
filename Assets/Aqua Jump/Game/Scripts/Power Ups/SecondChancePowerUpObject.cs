using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SecondChancePowerUp : BasePowerUp
{
    public SecondChancePowerUp(PowerUpSettings settings) : base(settings)
    {

    }

}

public class SecondChancePowerUpObject : BasePowerUpObject
{
    protected override void OnPlayerEnter(Player player, PowerUpSettings settings)
    {
        player.AddSecondChancePowerUp(settings);
    }
}
