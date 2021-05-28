using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    public enum State
    {
        Starting,
        Executing,
        Ended
    }

    [HideInInspector]
    public Action<PlayerController> onDrag;

    public float minDragDistance = 0.03f;

    public State state { get; private set; }

    public Vector2 startPos { get; private set; }
    public Vector2 currentPos { get; private set; }
    public Vector2 deltaPos => currentPos - startPos;
    private int m_oldTouchCount = 0;

    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTouches();
        UpdateMouse();
    }

    private void UpdateTouches()
    {
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if(m_oldTouchCount == 0)
            {
                startPos = touch.position;
            }

            currentPos = touch.position;

            if(deltaPos.sqrMagnitude > minDragDistance * minDragDistance)
            {
                onDrag?.Invoke(this);
            }
        }

        m_oldTouchCount = Input.touchCount;

    }

    private void UpdateMouse()
    {
        if(Input.GetMouseButtonDown(0))
        {
            startPos = Input.mousePosition;
            state = State.Starting;
        }
        else if(Input.GetMouseButtonUp(0))
        {
            if(state == State.Executing)
            {
                state = State.Ended;
                onDrag?.Invoke(this);
            }
            state = State.Ended;
        }

        if(Input.GetMouseButton(0))
        {
            currentPos = Input.mousePosition;

            if(deltaPos.sqrMagnitude > minDragDistance * minDragDistance)
            {
                onDrag?.Invoke(this);
            }

            state = State.Executing;
        }

    }
}
