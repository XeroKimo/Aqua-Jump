using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DoubleJumpPowerUp : BasePowerUp
{
    public DoubleJumpPowerUp(PowerUpSettings settings) : base(settings)
    {

    }

}

public class DoubleJumpPowerUpObject : BasePowerUpObject
{
    protected override void OnPlayerEnter(Player player, PowerUpSettings settings)
    {
        player.AddDoubleJumpPowerUp(settings);
    }
}
