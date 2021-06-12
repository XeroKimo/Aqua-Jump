﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class Player : MonoBehaviour
{
    private Rigidbody2D m_rigidBody;
    private CapsuleCollider2D m_collider;

    [SerializeField]
    private Animator m_animator;

    [SerializeField]
    private float m_jumpAngleClamp;
    private float m_jumpAngleDotClamp;
    private Vector3 m_jumpMinAngle;
    private Vector3 m_jumpMaxAngle;

    private byte m_jumpCount = 0;
    private byte m_maxJumpCount = 1;

    private bool m_inAir = false;

    public float powerMultiplier = 2.0f;
    public float maxPower = 10.0f;

    public Vector2 launchDirection { get; private set; }
    public float launchPower { get; private set; }

    public bool canJump => m_jumpCount > 0;
    public float colliderHeight => m_collider.bounds.size.y;

    public Vector2 velocity => m_rigidBody.velocity;

    private void Awake()
    {
        m_rigidBody = GetComponent<Rigidbody2D>();
        m_rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;

        m_collider = GetComponent<CapsuleCollider2D>();

        m_jumpAngleDotClamp = Vector3.Dot(Vector3.up, Quaternion.Euler(0, 0, m_jumpAngleClamp) * Vector3.up);
        m_jumpMinAngle = Quaternion.Euler(0, 0, -m_jumpAngleClamp) * Vector3.up;
        m_jumpMaxAngle = Quaternion.Euler(0, 0, m_jumpAngleClamp) * Vector3.up;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate()
    {
        m_animator.SetFloat("Y Velocity", m_rigidBody.velocity.y);
        RaycastHit2D hit = Physics2D.CapsuleCast((Vector2)transform.position - new Vector2(0, m_collider.size.y), m_collider.size, CapsuleDirection2D.Horizontal, 0, Vector2.down);
        if(hit.collider)
            m_animator.SetFloat("Nearest Ground Distance Y", hit.distance);
        else
            m_animator.SetFloat("Nearest Ground Distance Y", float.MaxValue);

        if(m_rigidBody.velocity.y == 0)
            ResetJumpCount();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Quaternion direction = Quaternion.Euler(0, 0, m_jumpAngleClamp);
        Quaternion direction2 = Quaternion.Euler(0, 0, -m_jumpAngleClamp);
        Vector3 axis = direction * Vector3.up;
        Vector3 axis2 = direction2 * Vector3.up;

        Gizmos.DrawLine(transform.position, transform.position + axis * 5);
        Gizmos.DrawLine(transform.position, transform.position + axis2 * 5);
    }

    public void Jump(Vector2 direction, float power)
    {
        launchDirection = direction;
        launchPower = Mathf.Min(power * powerMultiplier, maxPower);

        m_rigidBody.AddForce(launchDirection * launchPower, ForceMode2D.Impulse);

        m_jumpCount--;
    }

    public void Teleport(Vector2 position)
    {
        m_rigidBody.position = position;
    }

    public void ResetJumpCount()
    {
        m_jumpCount += 1;

        m_jumpCount = (byte)Mathf.Clamp(m_jumpCount, 0, m_maxJumpCount);
    }

    public void ResetVelocity()
    {
        m_rigidBody.velocity = Vector2.zero;
    }

    public Vector3 ClampDirection(Vector3 value)
    {
        if(Vector3.Dot(value, Vector3.up) < m_jumpAngleDotClamp)
        {
            if(Vector3.Dot(value, m_jumpMinAngle) > Vector3.Dot(value, m_jumpMaxAngle))
                return m_jumpMinAngle;
            else
                return m_jumpMaxAngle;
        }
        return value;
    }

    public PlayerCollisionVisitor CreateVisitor(GameManager gameManager)
    {
        return new PlayerCollisionVisitor(this, gameManager);
    }

    public void SquishAqua(float value)
    {
        m_animator.SetFloat("Drag", value);
        m_animator.SetBool("Squishing", true);
    }

    public void ReleaseSquish()
    {
        m_animator.SetFloat("Drag", 0);
        m_animator.SetBool("Squishing", false);
    }
}
