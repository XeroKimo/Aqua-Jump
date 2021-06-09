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

    public GroundPlatform startingPlatform => m_startingPlatform;

    public void Initialize()
    {
        platforms.Add(m_startingPlatform);
        m_startingPlatform.onCollisionEnter += PlatformCollisionEnter;
    }

    public void CreateRandomPlatforms(Rect bounds)
    {
        PlatformChances chances = GetRandomPlatformChance(bounds.center.y);

        float height = bounds.yMin;

        const float maxHeightAddition = 3.0f;

        while(height < bounds.yMax)
        {

            BasePlatform prefab = chances.GetRandomPlatform();

            Vector2 randomPosition = Vector2.zero;

            randomPosition.x += UnityEngine.Random.Range(bounds.xMin, bounds.xMax);

            randomPosition.y = platforms.Max(platform => platform.transform.position.y) + UnityEngine.Random.Range(chances.minimumHeightDifference, maxHeightAddition);

            if(randomPosition.y > bounds.yMax)
                break;

            CreatePlatform(prefab, randomPosition);
            height = randomPosition.y;

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
            return comp.transform.position.x < platform.transform.position.x + platform.collider.size.x * platform.transform.localScale.x &&
                platform.transform.position.x < comp.transform.position.x + comp.collider.size.x * comp.transform.localScale.x &&
                comp.transform.position.y < platform.transform.position.y + platform.collider.size.y * platform.transform.localScale.y &&
                platform.transform.position.y < comp.transform.position.y + comp.collider.size.y * comp.transform.localScale.y;
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
        List<PlatformChances> chances = m_chances.Where(val => height >= val.heightRange.x  && height <= val.heightRange.y).ToList();

        if(chances.Count == 0)
        {
            float maxMinHeight = m_chances.Max(val => val.heightRange.x);
            chances = m_chances.Where(val => val.heightRange.x >= maxMinHeight).ToList();
        }
        if(chances.Count == 0)
            chances = m_chances;


        return chances[UnityEngine.Random.Range(0, chances.Count)];
    }
}
