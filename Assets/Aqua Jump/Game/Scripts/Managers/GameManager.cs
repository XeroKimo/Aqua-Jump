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

    public LineRenderer dragVisualizer;

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

        dragVisualizer.enabled = false;
    }

    private void FixedUpdate()
    {
        WrapPlayer();
        UpdatePlatformCollision();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawWireCube(m_camera.transform.position, new Vector2(m_camera.orthographicSize * 2 * m_camera.aspect * m_wrappingBoundsExtent, m_camera.orthographicSize * 2));
    }

    private void OnDrag(PlayerController controller)
    {
        dragVisualizer.enabled = m_aqua.canJump;

        Vector2 startPos = m_camera.ScreenToWorldPoint(controller.startPos);
        Vector2 currentPos = m_camera.ScreenToWorldPoint(controller.currentPos);
        Vector2 deltaPos = startPos - currentPos;
        Vector2 direction = deltaPos.normalized;
        float magnitude = deltaPos.magnitude;

        dragVisualizer.SetPosition(0, m_aqua.transform.position);
        dragVisualizer.SetPosition(1, (Vector2)m_aqua.transform.position + direction * magnitude);

        if(controller.state == PlayerController.State.Ended)
        {
            if(m_aqua.canJump)
            {
                m_aqua.Jump(direction, magnitude);
                dragVisualizer.enabled = false;
            }
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
            float minAquaHeight = platform.transform.position.y + platform.colliderBounds.extents.y + m_aqua.colliderRadius / 2;
            if(minAquaHeight < m_aqua.transform.position.y && m_aqua.velocity.y < 0)
                platform.EnableCollisions();
            else
                platform.DisableCollisions();
        }
    }
}
