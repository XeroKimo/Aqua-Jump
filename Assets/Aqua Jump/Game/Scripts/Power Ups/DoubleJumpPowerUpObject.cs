using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DoubleJumpPowerUp : BasePowerUp
{
    public DoubleJumpPowerUp(float time, Action onPowerUpEnded) : base(time, onPowerUpEnded)
    {

    }

}

public class DoubleJumpPowerUpObject : BasePowerUpObject
{
    protected override void OnPlayerEnter(Player player)
    {
        player.AddDoubleJumpPowerUp();
    }
}
