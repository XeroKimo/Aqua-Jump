using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class PlatformCollisionVisitor
{
    public abstract void Resolve(StationaryPlatform platform);
    public abstract void Resolve(GroundPlatform platform);
    public abstract void Resolve(FragilePlatform platform);
    public abstract void Resolve(BouncePlatform platform);
    public abstract void Resolve(MovingPlatform platform);
}
