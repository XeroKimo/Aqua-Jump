using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class BasePlatform : MonoBehaviour
{
    public event Action<Collision2D, BasePlatform> onCollisionEnter;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        onCollisionEnter?.Invoke(collision, this);   
    }

    public abstract void CollisionVisit(PlatformCollisionVisitor visitor);
}
