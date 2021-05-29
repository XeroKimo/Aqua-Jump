using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BouncePlatform : BasePlatform
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void CollisionVisit(PlatformCollisionVisitor visitor)
    {
        visitor.Resolve(this);
    }
}
