using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

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

    private bool m_canJump = false;
    private bool m_canDoubleJump = false;

    private bool m_inAirLastFrame = false;

    private List<BasePowerUp> m_powerUps = new List<BasePowerUp>();

    public float powerMultiplier = 2.0f;
    public float maxPower = 10.0f;

    public Vector2 launchDirection { get; private set; }
    public float launchPower { get; private set; }

    public bool canJump => m_canJump || m_canDoubleJump;
    public float colliderHeight => m_collider.bounds.size.y;

    public Vector2 velocity => m_rigidBody.velocity;
    public bool canRevive => m_powerUps.Any(powerUp => powerUp is SecondChancePowerUp);

    public List<BasePowerUp> powerUps => m_powerUps;

    public event Action<BasePowerUp> onPowerUpRefreshed;

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
        List<BasePowerUp> powerUps = new List<BasePowerUp>(m_powerUps);
        foreach(BasePowerUp powerUp in powerUps)
        {
            powerUp.Update();
            if(powerUp.ended)
                m_powerUps.Remove(powerUp);
        }
    }

    private void FixedUpdate()
    {
        float distance = float.MaxValue;

        RaycastHit2D hit = Physics2D.CapsuleCast((Vector2)transform.position - new Vector2(0, m_collider.size.y), m_collider.size, CapsuleDirection2D.Horizontal, 0, Vector2.down);
        if(hit.collider)
            distance = hit.distance;

        m_animator.SetFloat("Nearest Ground Distance Y", distance);
        bool inAirCurrentFrame = m_rigidBody.velocity.y != 0;

        if(!inAirCurrentFrame && m_inAirLastFrame)
        {
            m_animator.SetTrigger("Landed");
            ResetJumpCount();
        }

        m_inAirLastFrame = inAirCurrentFrame;
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
        ResetVelocity();

        launchDirection = direction;
        launchPower = Mathf.Min(power * powerMultiplier, maxPower);

        m_rigidBody.AddForce(launchDirection * launchPower, ForceMode2D.Impulse);

        if(!m_canJump && m_canDoubleJump)
            m_canDoubleJump = false;
        m_canJump = false;
    }

    public void Teleport(Vector2 position)
    {
        m_rigidBody.position = position;
    }

    public void ResetJumpCount()
    {
        m_canJump = true;
        m_canDoubleJump = (m_canDoubleJump == true) ? true : (m_powerUps.Any(powerUp => powerUp is DoubleJumpPowerUp));
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

    public void AddDoubleJumpPowerUp(PowerUpSettings settings)
    {
        BasePowerUp powerUp = m_powerUps.FirstOrDefault(p => p is DoubleJumpPowerUp);

        m_canDoubleJump = true;

        if(powerUp != null)
        {
            powerUp.Reset(settings);
        }
        else
        {
            powerUp = new DoubleJumpPowerUp(settings);
            powerUp.onPowerUpEnded += EndDoubleJumpPowerUp;
            m_powerUps.Add(powerUp);
        }

        onPowerUpRefreshed?.Invoke(powerUp);
    }

    public void AddShieldPowerUp(PowerUpSettings settings)
    {
        BasePowerUp powerUp = m_powerUps.FirstOrDefault(p => p is ShieldPowerUp);
        if(powerUp != null)
        {
            powerUp.Reset(settings);
        }
        else
        {
            powerUp = new ShieldPowerUp(settings);
            powerUp.onPowerUpEnded += EndShieldPowerUp;
            m_powerUps.Add(powerUp);
        }

        onPowerUpRefreshed?.Invoke(powerUp);
    }

    public void AddSecondChancePowerUp(PowerUpSettings settings)
    {
        BasePowerUp powerUp = m_powerUps.FirstOrDefault(p => p is SecondChancePowerUp);
        if(powerUp != null)
        {
            powerUp.Reset(settings);
        }
        else
        {
            powerUp = new SecondChancePowerUp(settings);
            powerUp.onPowerUpEnded += EndSecondChancePowerUp;
            m_powerUps.Add(powerUp);
        }

        onPowerUpRefreshed?.Invoke(powerUp);
    }

    public void ConsumeSecondChance()
    {
        m_powerUps.First(powerUp => powerUp is SecondChancePowerUp).End();
    }

    private void EndDoubleJumpPowerUp()
    {
        Debug.LogWarning("Double jump ended");
    }

    private void EndShieldPowerUp()
    {

    }

    private void EndSecondChancePowerUp()
    {

    }
}
