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
    private LevelSpawner m_levelGenerator;

    [SerializeField]
    private MainUI m_mainUI;

    [SerializeField]
    private GameOverUI m_gameOverUI;

    [SerializeField]
    private GroundPlatform m_startingPlatform;

    private List<BasePlatform> m_platforms = new List<BasePlatform>();

    private bool m_firstJump = true;

    private float m_highestPlayerHeight = 0;
    private float m_minimumNextPlatformHeight = 0;

    private BasePlatform m_previousPlatform;
    private float m_highestPlatformHeight = 0;

    private float m_cameraStartHeight = 0;

    public LineRenderer dragVisualizer;
    public float maxDragVisualizerDistance = 3;

    public int score => (int)(m_highestPlayerHeight - m_aqua.colliderHeight);

    private void OnEnable()
    {
        m_controller.onDrag += OnDrag;
        m_mainUI.onRestart += ShuffleWorld;
        m_gameOverUI.onRestart += RestartGame;
    }

    private void OnDisable()
    {
        m_controller.onDrag -= OnDrag;
        m_mainUI.onRestart -= ShuffleWorld;
        m_gameOverUI.onRestart -= RestartGame;
    }

    // Start is called before the first frame update
    private void Start()
    {
        m_camera.trackedGameObject = m_aqua.gameObject;
        m_cameraStartHeight = m_camera.transform.position.y;

        dragVisualizer.enabled = false;
        m_camera.onLerpFinished += OnLerpFinished;
        m_previousPlatform = m_startingPlatform;

        m_platforms.Add(m_startingPlatform);

        ShuffleWorld();
    }

    private void FixedUpdate()
    {
        WrapPlayer();
        UpdatePlatformCollision();
        DetectFall();

        if(m_aqua.transform.position.y > m_highestPlayerHeight)
            m_highestPlayerHeight = m_aqua.transform.position.y;

        m_mainUI.SetScore(score);

        while(m_minimumNextPlatformHeight < m_highestPlayerHeight)
        {
            Rect platformBounds = m_camera.bounds;

            platformBounds.center += new Vector2(0, platformBounds.height * 2);
            GenerateLevel(platformBounds);
            m_minimumNextPlatformHeight += m_camera.bounds.height;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawWireCube(m_camera.camera.transform.position, new Vector2(m_camera.camera.orthographicSize * 2 * m_camera.camera.aspect * m_wrappingBoundsExtent, m_camera.camera.orthographicSize * 2));

        Gizmos.color = Color.red;

        Gizmos.DrawSphere(m_camera.bounds.min, 1);

        Gizmos.DrawWireCube(new Vector2(0, m_minimumNextPlatformHeight + m_cameraStartHeight) , m_camera.bounds.size);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(new Vector2(0, m_minimumNextPlatformHeight + m_cameraStartHeight + m_camera.bounds.size.y * 2) , m_camera.bounds.size);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(new Vector2(0, m_minimumNextPlatformHeight + m_cameraStartHeight - m_camera.bounds.size.y) , m_camera.bounds.size);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(new Vector2(0, m_minimumNextPlatformHeight + m_cameraStartHeight + m_camera.bounds.size.y) , m_camera.bounds.size);
    }

    public void DestroyPlatform(BasePlatform platform)
    {
        if(platform == null)
            return;

        m_platforms.Remove(platform);
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

        if(deltaPos.y < 0)
            magnitude = 0;

        float dragVisualizerDistance = Mathf.Lerp(0, maxDragVisualizerDistance, magnitude * m_aqua.powerMultiplier / m_aqua.maxPower);

        dragVisualizer.SetPosition(0, m_aqua.transform.position);
        dragVisualizer.SetPosition(1, (Vector2)m_aqua.transform.position + direction * dragVisualizerDistance);


        if(controller.state == PlayerController.State.Executing)
        {
            if(m_aqua.canJump)
            {
                m_aqua.SquishAqua(Mathf.Clamp(magnitude * m_aqua.powerMultiplier / m_aqua.maxPower, 0, 1));
            }
        }
        if(controller.state == PlayerController.State.Ended)
        {
            if(m_aqua.canJump && magnitude > 0)
            {
                m_aqua.Jump(direction, magnitude);
                dragVisualizer.enabled = false;
                m_aqua.ReleaseSquish();

                if(m_previousPlatform is FragilePlatform)
                    DestroyPlatform(m_previousPlatform);
            }
        }
    }

    private void OnLerpFinished()
    {
        RemovePlatforms();
    }

    private void HandlePlatformCollision(Collision2D arg1, BasePlatform arg2)
    {
        if(arg1.gameObject == m_aqua.gameObject)
        {
            m_aqua.ResetVelocity();
            arg2.CollisionVisit(m_aqua.CreateVisitor(this));

            if(arg2.transform.position.y > m_highestPlatformHeight)
            {
                //m_killPlane = m_camera.bounds.yMin;
                m_highestPlatformHeight = arg2.transform.position.y;

                RemovePlatforms();
            }

            if(m_previousPlatform != null)
            {
                if(m_firstJump && m_previousPlatform != arg2)
                {
                    m_firstJump = false;
                    m_mainUI.DisableRestart();
                }
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
        foreach(BasePlatform platform in m_platforms)
        {
            float minAquaHeight = platform.transform.position.y + platform.collider.bounds.extents.y + m_aqua.colliderHeight / 2;
            if(minAquaHeight < m_aqua.transform.position.y)
            {
                if(m_aqua.velocity.y < 0)
                    platform.EnableCollisions();
            }
            else
                platform.DisableCollisions();
        }
    }

    private void DetectFall()
    {
        if(m_aqua.transform.position.y < m_camera.bounds.yMin)
        {
            if(m_aqua.canRevive)
            {
                m_aqua.ConsumeSecondChance();


            }
            else
            {
                m_gameOverUI.Show();

                const string highScoreString = "Highscore";
                int highScore = PlayerPrefs.GetInt(highScoreString, 0);

                if(score > highScore)
                    highScore = score;

                PlayerPrefs.SetInt(highScoreString, highScore);

                m_gameOverUI.SetScore(score);
                m_gameOverUI.SetHighscore(highScore);
            }
        }
    }

    private void RemovePlatforms()
    {
        Rect cameraBounds = m_camera.bounds;
        Bounds bounds = new Bounds();
        bounds.size = cameraBounds.size;
        bounds.center = cameraBounds.center;
        List<BasePlatform> platformCopy = new List<BasePlatform>(m_platforms);

        foreach(BasePlatform platform in platformCopy)
        {
            if(!platform.collider.bounds.Intersects(bounds) && platform.transform.position.y < cameraBounds.yMin)
                DestroyPlatform(platform);
        }
    }

    private void ShuffleWorld()
    {
        List<BasePlatform> platforms = new List<BasePlatform>(m_platforms);

        platforms.Remove(m_previousPlatform);
        foreach(BasePlatform platform in platforms)
        {
            DestroyPlatform(platform);
        }

        Rect platformBounds = m_camera.bounds;
        GenerateLevel(platformBounds);

        platformBounds.center += new Vector2(0, platformBounds.height);
        GenerateLevel(platformBounds);

        platformBounds.center += new Vector2(0, platformBounds.height);
        GenerateLevel(platformBounds);

        m_minimumNextPlatformHeight = platformBounds.height;
    }

    private void GenerateLevel(Rect bounds)
    {
        m_levelGenerator.GenerateLevel(bounds, m_platforms, out List<BasePlatform> platforms, out List<BasePowerUpObject> powerUps);

        foreach(BasePlatform platform in platforms)
        {
            platform.onCollisionEnter += HandlePlatformCollision;
        }

        m_platforms.AddRange(platforms);
    }

}
