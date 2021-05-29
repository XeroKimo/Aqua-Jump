using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Player m_aqua;

    [SerializeField]
    private PlayerController m_controller;

    [SerializeField]
    private Camera m_camera;

    [SerializeField]
    private float m_wrappingBoundsExtent = 1.3f;

    private List<BasePlatform> m_platforms = new List<BasePlatform>();

    public Vector2 m_debugStartPos;
    public Vector2 m_debugCurrentPos;

    private void OnEnable()
    {
        m_controller.onDrag += OnDrag;
    }

    private void OnDisable()
    {
        m_controller.onDrag -= OnDrag;
    }

    // Start is called before the first frame update
    private void Start()
    {
        foreach(BasePlatform platform in FindObjectsOfType<BasePlatform>())
        {
            AddPlatform(platform);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        UpdatePlatformCollision();
    }

    private void FixedUpdate()
    {
        WrapPlayer();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawLine(m_debugStartPos, m_debugCurrentPos);

        Gizmos.color = Color.green;

        Gizmos.DrawWireCube(m_camera.transform.position, new Vector2(m_camera.orthographicSize * 2 * m_camera.aspect * m_wrappingBoundsExtent, m_camera.orthographicSize * 2));
    }

    private void OnDrag(PlayerController controller)
    {
        Debug.Log("Dragging");
        m_debugStartPos = m_camera.ScreenToWorldPoint(controller.startPos);
        m_debugCurrentPos = m_camera.ScreenToWorldPoint(controller.currentPos);

        if(controller.state == PlayerController.State.Ended)
        {
            if(m_aqua.canJump)
                m_aqua.Jump((m_debugStartPos - m_debugCurrentPos).normalized, (m_debugStartPos - m_debugCurrentPos).magnitude);
            m_debugStartPos = Vector2.zero;
            m_debugCurrentPos = Vector2.zero;
        }
    }

    public void DestroyPlatform(BasePlatform platform)
    {
        m_platforms.Remove(platform);
        Destroy(platform.gameObject);
    }

    private void AddPlatform(BasePlatform platform)
    {
        m_platforms.Add(platform);
        platform.onCollisionEnter += Platform_onCollisionEnter;
        platform.DisableCollisions();
    }

    private void Platform_onCollisionEnter(Collision2D arg1, BasePlatform arg2)
    {
        if(arg1.gameObject == m_aqua.gameObject)
        {
            arg2.CollisionVisit(m_aqua.CreateVisitor(this));
        }
    }

    private Vector2 GetHorizontalWrappingBounds()
    {
        float width = m_camera.orthographicSize * 2 * m_camera.aspect * m_wrappingBoundsExtent;
        return new Vector2(m_camera.transform.position.x - width / 2, m_camera.transform.position.x + width / 2);
    }

    private void WrapPlayer()
    {
        Vector2 horizontalWrapBounds = GetHorizontalWrappingBounds();

        if(m_aqua.transform.position.x < horizontalWrapBounds.x)
        {
            m_aqua.Teleport(new Vector2(horizontalWrapBounds.y, m_aqua.transform.position.y));
        }
        else if(m_aqua.transform.position.x > horizontalWrapBounds.y)
        {
            m_aqua.Teleport(new Vector2(horizontalWrapBounds.x, m_aqua.transform.position.y));
        }
    }

    private void UpdatePlatformCollision()
    {
        foreach(BasePlatform platform in m_platforms)
        {
            if(platform.transform.position.y < m_aqua.transform.position.y)
                platform.EnableCollisions();
        }
    }
}
