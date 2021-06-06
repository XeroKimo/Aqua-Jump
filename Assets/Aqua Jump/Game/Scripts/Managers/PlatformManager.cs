using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class PlatformManager : MonoBehaviour
{
    [SerializeField]
    private List<PlatformChances> m_chances;

    [SerializeField]
    private GroundPlatform m_startingPlatform;

    [HideInInspector]
    public List<BasePlatform> platforms = new List<BasePlatform>();

    public event Action<Collision2D, BasePlatform> onCollisionEnter;

    // Start is called before the first frame update
    private void Start()
    {
        platforms.Add(m_startingPlatform);
        m_startingPlatform.onCollisionEnter += PlatformCollisionEnter;
    }

    public void CreateRandomPlatforms(Rect bounds, float heightOffset)
    {
        PlatformChances chances = GetRandomPlatformChance(heightOffset);

        int totalPlatformsToSpawn = chances.spawnCount;

        Vector2 minPosition = bounds.center - bounds.size / 2;
        Vector2 maxPosition = bounds.center + bounds.size / 2;

        minPosition.y += heightOffset;
        maxPosition.y += heightOffset;

        const int maxFailedAttempts = 100;
        int failedAttempts = 0;

        for(int i = 0; i < totalPlatformsToSpawn; i++)
        {
            if(failedAttempts >= maxFailedAttempts)
                break;

            BasePlatform prefab = chances.GetRandomPlatform();

            Vector2 randomPosition = Vector2.zero;

            randomPosition.x += UnityEngine.Random.Range(minPosition.x, maxPosition.x);
            randomPosition.y += UnityEngine.Random.Range(minPosition.y, maxPosition.y);

            if(!CreatePlatform(prefab, randomPosition))
            {
                failedAttempts++;
            }

        }
    }

    private void PlatformCollisionEnter(Collision2D arg1, BasePlatform arg2)
    {
        onCollisionEnter?.Invoke(arg1, arg2);
    }

    private bool CreatePlatform(BasePlatform prefab, Vector2 position)
    {
        BasePlatform platform = Instantiate(prefab, position, Quaternion.identity, transform);

        if(platforms.Any(comp =>
        {
            return comp.transform.position.x < platform.transform.position.x + platform.collider.size.x &&
                platform.transform.position.x < comp.transform.position.x + comp.collider.size.x &&
                comp.transform.position.y < platform.transform.position.y + platform.collider.size.y &&
                platform.transform.position.y < comp.transform.position.y + comp.collider.size.y;
        }))
        {
            Destroy(platform.gameObject);
            return false;
        }

        platforms.Add(platform);
        platform.onCollisionEnter += PlatformCollisionEnter;
        platform.DisableCollisions();

        return true;
    }

    private PlatformChances GetRandomPlatformChance(float height)
    {
        PlatformChances chances = null;

        chances = m_chances[0];
        return chances;
    }
}
