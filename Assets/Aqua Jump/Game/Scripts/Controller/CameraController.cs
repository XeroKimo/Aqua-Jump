using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraController : MonoBehaviour
{
    public event Action onLerpFinished;

    public new Camera camera;

    private Vector3 m_targetPos;
    private Vector3 m_currentPos;
    private float m_lerpTime;
    private float m_time;
    private bool m_isLerping;

    public Rect bounds
    {
        get
        {
            Rect rect = new Rect();
            rect.size = new Vector2(camera.orthographicSize * camera.aspect * 2, camera.orthographicSize * 2);
            rect.center = camera.transform.position;
            return rect;
        }
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    public void LerpPosition(Vector3 position, float t)
    {
        m_targetPos = position;
        m_currentPos = transform.position;
        m_lerpTime = t;
        m_time = 0;
        m_isLerping = true;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(m_isLerping)
        {
            transform.position = Vector3.Lerp(m_currentPos, m_targetPos, m_time / m_lerpTime);

            if(m_time >= m_lerpTime)
            {
                m_isLerping = false;
                onLerpFinished?.Invoke();
            }

            m_time += Time.deltaTime;


        }

    }
}
