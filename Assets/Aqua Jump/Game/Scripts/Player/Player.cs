using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    private Rigidbody2D m_rigidBody;

    public float powerMultiplier = 2.0f;
    public float maxPower = 10.0f;


    public Vector2 launchDirection { get; private set; }
    public float launchPower { get; private set; }

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

    public void Launch(Vector2 direction, float power)
    {
        launchDirection = direction;
        launchPower = Mathf.Min(power * powerMultiplier, maxPower);

        m_rigidBody.AddForce(launchDirection * launchPower, ForceMode2D.Impulse);
    }

    public PlayerCollisionVisitor CreateVisitor(GameManager gameManager)
    {
        return new PlayerCollisionVisitor(this, gameManager);
    }
}
