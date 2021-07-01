using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class BasePowerUp
{
    private PowerUpSettings m_settings;
    private float m_time;
    public event Action onPowerUpEnded;

    public bool ended => m_time <= 0;
    public Sprite icon => m_settings.icon;
    public float time => m_time;
    public float initialTime => m_settings.duration;


    public BasePowerUp(PowerUpSettings settings)
    {
        m_settings = settings;
        m_time = initialTime;
    }

    public void Update()
    {
        m_time -= Time.deltaTime;
        if(m_time <= 0)
        {
            onPowerUpEnded?.Invoke();
        }
    }

    public void Reset(PowerUpSettings settings)
    {
        m_settings = settings;
        m_time = m_settings.duration;
    }

    public void End()
    {
        m_time = 0;
    }
};

public abstract class BasePowerUpObject : MonoBehaviour
{
    [SerializeField]
    private PowerUpSettings m_settings;

    [SerializeField]
    private SpriteRenderer m_icon;

    private void Start()
    {
        m_icon.sprite = m_settings.icon;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.GetComponent<Player>())
        {
            OnPlayerEnter(collision.gameObject.GetComponent<Player>(), m_settings);

            Destroy(gameObject);
        }
    }

    protected abstract void OnPlayerEnter(Player player, PowerUpSettings settings);
}
