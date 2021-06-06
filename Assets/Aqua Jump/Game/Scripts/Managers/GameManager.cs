using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Player m_aqua;

    [SerializeField]
    private PlayerController m_controller;

    [SerializeField]
    private CameraController m_camera;

    [SerializeField]
    private float m_wrappingBoundsExtent = 1.3f;

    [SerializeField]
    private PlatformManager m_platformManager;

    private float m_highestPlayerHeight = 0;
    private float m_minimumNextPlatformHeight = 0;

    private BasePlatform m_previousPlatform;
    private float m_highestPlatformHeight = 0;

    private const float m_cameraOffset = 3;

    public LineRenderer dragVisualizer;
    public float maxDragVisualizerDistance = 3;



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
        dragVisualizer.enabled = false;
        m_camera.onLerpFinished += OnLerpFinished;

        m_platformManager.Initialize();
        m_platformManager.onCollisionEnter += Platform_onCollisionEnter;

        Debug.Log(m_camera.bounds.center.y);
        m_platformManager.CreateRandomPlatforms(m_camera.bounds, 0);
        m_platformManager.CreateRandomPlatforms(m_camera.bounds, m_camera.bounds.height);
        m_platformManager.CreateRandomPlatforms(m_camera.bounds, m_camera.bounds.height * 2);
        m_platformManager.CreateRandomPlatforms(m_camera.bounds, m_camera.bounds.height * 3);
        m_minimumNextPlatformHeight = m_camera.bounds.yMax;

        m_previousPlatform = m_platformManager.startingPlatform;
    }

    private void FixedUpdate()
    {
        WrapPlayer();
        UpdatePlatformCollision();
        DetectFall();

        m_highestPlayerHeight = m_aqua.transform.position.y;

        while(m_minimumNextPlatformHeight < m_highestPlayerHeight)
        {
            m_platformManager.CreateRandomPlatforms(m_camera.bounds, m_camera.bounds.height);
            m_minimumNextPlatformHeight += m_camera.bounds.height;
        }
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawWireCube(m_camera.camera.transform.position, new Vector2(m_camera.camera.orthographicSize * 2 * m_camera.camera.aspect * m_wrappingBoundsExtent, m_camera.camera.orthographicSize * 2));

        Gizmos.color = Color.red;

        Gizmos.DrawSphere(m_camera.bounds.min, 1);

        Gizmos.DrawWireCube(new Vector2(0, m_minimumNextPlatformHeight + m_camera.bounds.height / 2) , m_camera.bounds.size);
    }

    public void DestroyPlatform(BasePlatform platform)
    {
        m_platformManager.platforms.Remove(platform);
        Destroy(platform.gameObject);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("Game");
    }

    private void OnDrag(PlayerController controller)
    {
        dragVisualizer.enabled = m_aqua.canJump;

        Vector2 startPos = m_camera.camera.ScreenToWorldPoint(controller.startPos);
        Vector2 currentPos = m_camera.camera.ScreenToWorldPoint(controller.currentPos);
        Vector2 deltaPos = startPos - currentPos;
        Vector2 direction = deltaPos.normalized;
        direction = m_aqua.ClampDirection(direction);
        float magnitude = deltaPos.magnitude;

        float dragVisualizerDistance = Mathf.Lerp(0, maxDragVisualizerDistance, magnitude * m_aqua.powerMultiplier / m_aqua.maxPower);

        dragVisualizer.SetPosition(0, m_aqua.transform.position);
        dragVisualizer.SetPosition(1, (Vector2)m_aqua.transform.position + direction * dragVisualizerDistance);

        if(controller.state == PlayerController.State.Ended)
        {
            if(m_aqua.canJump)
            {
                m_aqua.Jump(direction, magnitude);
                dragVisualizer.enabled = false;

                if(m_previousPlatform is FragilePlatform)
                    DestroyPlatform(m_previousPlatform);
                    
            }
        }
    }

    private void OnLerpFinished()
    {
        RemovePlatforms();
    }

    private void Platform_onCollisionEnter(Collision2D arg1, BasePlatform arg2)
    {
        if(arg1.gameObject == m_aqua.gameObject)
        {
            arg2.CollisionVisit(m_aqua.CreateVisitor(this));

            if(arg2.transform.position.y > m_highestPlatformHeight)
            {
                m_highestPlatformHeight = arg2.transform.position.y;
                m_camera.LerpPosition(new Vector3(0, m_aqua.transform.position.y + m_cameraOffset, -10), 1);
            }

            m_previousPlatform = arg2;
        }
    }

    private Vector2 GetHorizontalWrappingBounds()
    {
        float width = m_camera.bounds.width;
        return new Vector2(m_camera.camera.transform.position.x - width / 2, m_camera.camera.transform.position.x + width / 2);
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
        foreach(BasePlatform platform in m_platformManager.platforms)
        {
            float minAquaHeight = platform.transform.position.y + platform.collider.bounds.extents.y + m_aqua.colliderHeight / 2;
            if(minAquaHeight < m_aqua.transform.position.y && m_aqua.velocity.y < 0)
                platform.EnableCollisions();
            else
                platform.DisableCollisions();
        }
    }

    private void DetectFall()
    {
        if(m_aqua.transform.position.y < m_camera.bounds.yMin)
        {
            RestartGame();
        }
    }

    private void RemovePlatforms()
    {
        Rect cameraBounds = m_camera.bounds;
        List<BasePlatform> platformCopy = new List<BasePlatform>(m_platformManager.platforms);

        foreach(BasePlatform platform in platformCopy)
        {
            if(platform.transform.position.y < cameraBounds.yMin)
            {
                DestroyPlatform(platform);
            }
        }
    }
}
