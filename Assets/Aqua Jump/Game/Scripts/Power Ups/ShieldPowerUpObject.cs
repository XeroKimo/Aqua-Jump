using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ShieldPowerUp : BasePowerUp
{
    public ShieldPowerUp(float time, Action onPowerUpEnded) : base(time, onPowerUpEnded)
    {

    }

}

public class ShieldPowerUpObject : BasePowerUpObject
{
    protected override void OnPlayerEnter(Player player)
    {
        player.AddShieldPowerUp();
    }
}
