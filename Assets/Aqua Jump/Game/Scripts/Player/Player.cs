using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    private Rigidbody2D m_rigidBody;

    private byte m_jumpCount = 0;

    public float powerMultiplier = 2.0f;
    public float maxPower = 10.0f;

    public Vector2 launchDirection { get; private set; }
    public float launchPower { get; private set; }

    public bool canJump => m_jumpCount > 0;

    private void Awake()
    {
        m_rigidBody = GetComponent<Rigidbody2D>();
        m_rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
    }

    public void ResetVelocity()
    {
        m_rigidBody.velocity = Vector2.zero;
    }

    public PlayerCollisionVisitor CreateVisitor(GameManager gameManager)
    {
        return new PlayerCollisionVisitor(this, gameManager);
    }
}
