using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ShieldPowerUp : BasePowerUp
{
    public ShieldPowerUp(PowerUpSettings settings) : base(settings)
    {

    }

}

public class ShieldPowerUpObject : BasePowerUpObject
{
    protected override void OnPlayerEnter(Player player, PowerUpSettings settings)
    {
        player.AddShieldPowerUp(settings);
    }
}
