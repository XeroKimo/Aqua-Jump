using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BasePowerUp
{
    private float m_initialTime;
    private float m_time;
    private Action m_onPowerUpEnded;

    public bool ended => m_time <= 0;

    public BasePowerUp(float time, Action onPowerUpEnded)
    {
        m_time = time;
        m_initialTime = time;
        m_onPowerUpEnded = onPowerUpEnded;
    }

    public void Update()
    {
        if(m_time <= 0)
        {
            m_onPowerUpEnded?.Invoke();
        }

        m_time -= Time.deltaTime;
    }

    public void Reset()
    {
        m_time = m_initialTime;
    }

    public void End()
    {
        m_time = 0;
    }
};

public abstract class BasePowerUpObject : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.GetComponent<Player>())
        {
            OnPlayerEnter(collision.gameObject.GetComponent<Player>());

            Destroy(gameObject);
        }
    }

    protected abstract void OnPlayerEnter(Player player);
}
