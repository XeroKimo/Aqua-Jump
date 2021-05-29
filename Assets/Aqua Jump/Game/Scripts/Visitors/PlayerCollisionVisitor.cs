using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerCollisionVisitor : PlatformCollisionVisitor
{
    private Player m_player;
    private GameManager m_gameManager;

    public PlayerCollisionVisitor(Player player, GameManager gameManager)
    {
        m_player = player;
        m_gameManager = gameManager;
    }

    public override void Resolve(StationaryPlatform platform)
    {
        CommonResolution(platform);
    }

    public override void Resolve(GroundPlatform platform)
    {
        CommonResolution(platform);
    }

    public override void Resolve(FragilePlatform platform)
    {
        CommonResolution(platform);
    }

    public override void Resolve(BouncePlatform platform)
    {
        CommonResolution(platform);
    }

    public override void Resolve(MovingPlatform platform)
    {
        CommonResolution(platform);
    }

    private void CommonResolution(BasePlatform platform)
    {
        Debug.Log($"{platform.GetType()} Collided with player");
        m_player.ResetJumpCount();
    }
}
