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
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrag(PlayerController controller)
    {
        Debug.Log("Dragging");
        m_debugStartPos = m_camera.ScreenToWorldPoint(controller.startPos);
        m_debugCurrentPos = m_camera.ScreenToWorldPoint(controller.currentPos);

        if(controller.state == PlayerController.State.Ended)
        {
            m_aqua.Launch((m_debugStartPos - m_debugCurrentPos).normalized, (m_debugStartPos - m_debugCurrentPos).magnitude);
            m_debugStartPos = Vector2.zero;
            m_debugCurrentPos = Vector2.zero;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawLine(m_debugStartPos, m_debugCurrentPos);

        Gizmos.color = Color.green;

        Gizmos.DrawWireCube(m_camera.transform.position, new Vector2(m_camera.orthographicSize * 2 * m_camera.aspect * m_wrappingBoundsExtent, m_camera.orthographicSize * 2));
    }
}
