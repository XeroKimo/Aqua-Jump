using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlatformManager : MonoBehaviour
{
    [SerializeField]
    private List<PlatformChances> m_chances;

    [SerializeField]
    private GroundPlatform m_startingPlatform;

    [SerializeField]
    private Rect m_platformSpawnBounds;

    [HideInInspector]
    public List<BasePlatform> platforms = new List<BasePlatform>();

    public event Action<Collision2D, BasePlatform> onCollisionEnter;

    // Start is called before the first frame update
    private void Start()
    {
        platforms.Add(m_startingPlatform);
        m_startingPlatform.onCollisionEnter += PlatformCollisionEnter;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(m_platformSpawnBounds.center, m_platformSpawnBounds.size);

    }

    public void CreateRandomPlatforms(float height)
    {
        PlatformChances chances = GetRandomPlatformChance(height);

        int totalPlatformsToSpawn = platforms.Count + chances.spawnCount;

        Vector2 minPosition = m_platformSpawnBounds.center - m_platformSpawnBounds.size / 2;
        Vector2 maxPosition = m_platformSpawnBounds.center + m_platformSpawnBounds.size / 2;

        minPosition.y += height;
        maxPosition.y += height;

        while(platforms.Count < totalPlatformsToSpawn)
        {
            BasePlatform prefab = chances.GetRandomPlatform();

            Vector2 randomPosition = Vector2.zero;

            randomPosition.x += UnityEngine.Random.Range(minPosition.x, maxPosition.x);
            randomPosition.y += UnityEngine.Random.Range(minPosition.y, maxPosition.y);

            CreatePlatform(prefab, randomPosition);
        }
    }

    private void PlatformCollisionEnter(Collision2D arg1, BasePlatform arg2)
    {
        onCollisionEnter?.Invoke(arg1, arg2);
    }

    private void CreatePlatform(BasePlatform prefab, Vector2 position)
    {
        BasePlatform platform = Instantiate(prefab, position, Quaternion.identity, transform);
        platforms.Add(platform);
        platform.onCollisionEnter += PlatformCollisionEnter;
        platform.DisableCollisions();
    }

    private PlatformChances GetRandomPlatformChance(float height)
    {
        PlatformChances chances = null;

        chances = m_chances[0];
        return chances;
    }
}
