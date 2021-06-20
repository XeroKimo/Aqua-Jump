using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public abstract class BasePlatform : MonoBehaviour
{
    public event Action<Collision2D, BasePlatform> onCollisionEnter;

    [SerializeField]
    private Rigidbody2D m_rigidBody;

    [SerializeField]
    private BoxCollider2D m_collider;

    public new BoxCollider2D collider => m_collider;

    void Awake()
    {
        m_rigidBody.bodyType = RigidbodyType2D.Kinematic;
    }

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

    public void EnableCollisions()
    {
        m_collider.enabled = true;
    }

    public void DisableCollisions()
    {
        m_collider.enabled = false;
    }
}
