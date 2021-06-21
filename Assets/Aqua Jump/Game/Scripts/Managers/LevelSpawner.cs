using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class LevelSpawner : MonoBehaviour
{
    [SerializeField]
    private List<LevelDescription> m_chances;

    public void GenerateLevel(Rect bounds, List<BasePlatform> existingPlatforms, out List<BasePlatform> platforms, out List<BasePowerUpObject> powerUps)
    {
        platforms = new List<BasePlatform>();
        powerUps = new List<BasePowerUpObject>();

        List<BasePlatform> allPlatforms = new List<BasePlatform>(existingPlatforms);

        LevelDescription chances = GetRandomPlatformChance(bounds.center.y);

        float height = bounds.yMin;

        const float maxHeightAddition = 3.0f;
        while(height < bounds.yMax)
        {
            BasePlatform prefab = chances.GetRandomPlatform();

            Vector2 randomPosition = Vector2.zero;

            float halfWidth = (prefab.collider.size.x * prefab.transform.localScale.x) / 2;

            randomPosition.x += UnityEngine.Random.Range(bounds.xMin + halfWidth, bounds.xMax - halfWidth);

            if(platforms.Count == 0)
                randomPosition.y = height + UnityEngine.Random.Range(chances.minimumHeightDifference, maxHeightAddition);
            else
                randomPosition.y = platforms.Max(platform => platform.transform.position.y) + UnityEngine.Random.Range(chances.minimumHeightDifference, maxHeightAddition);

            if(randomPosition.y > bounds.yMax)
                break;

            BasePlatform spawnedPlatform = CreatePlatform(prefab, randomPosition, allPlatforms);

            if(spawnedPlatform != null)
            {
                allPlatforms.Add(spawnedPlatform);
                platforms.Add(spawnedPlatform);

                BasePowerUpObject powerUp = CreatePowerUp(chances, spawnedPlatform);

                if(powerUp)
                {
                    powerUps.Add(powerUp);
                }
            }

            height = randomPosition.y;

        }
    }

    private BasePlatform CreatePlatform(BasePlatform prefab, Vector2 position, List<BasePlatform> platforms)
    {
        BasePlatform platform = Instantiate(prefab, position, Quaternion.identity, transform);

        if(platforms.Any(comp =>
        {
            Vector2 minOne = comp.transform.position - ((Vector3)comp.collider.size + comp.transform.localScale)/ 2;
            Vector2 maxOne = comp.transform.position + ((Vector3)comp.collider.size + comp.transform.localScale)/ 2;
            Vector2 minTwo = platform.transform.position - ((Vector3)platform.collider.size + platform.transform.localScale) / 2;
            Vector2 maxTwo = platform.transform.position + ((Vector3)platform.collider.size + platform.transform.localScale) / 2;

            return (minOne.x <= maxTwo.x && maxOne.x >= minTwo.x) && (minOne.y <= maxTwo.y && maxOne.y >= minTwo.y) ||
                (minOne.x >= maxTwo.x && maxOne.x <= minTwo.x) && (minOne.y >= maxTwo.y && maxOne.y <= minTwo.y); 
        }))
        {
            Destroy(platform.gameObject);
            return null;
        }


        return platform;
    }

    private BasePowerUpObject CreatePowerUp(LevelDescription description, BasePlatform platform)
    {
        BasePowerUpObject prefab = description.GetRandomPowerUp();

        if(prefab == null)
            return null;

        BasePowerUpObject powerUp = Instantiate(prefab);
        powerUp.transform.position = platform.transform.position + new Vector3(0, platform.collider.size.y * platform.transform.localScale.y);

        return powerUp;
    }

    private LevelDescription GetRandomPlatformChance(float height)
    {
        List<LevelDescription> chances = m_chances.Where(val => height >= val.heightRange.x  && height <= val.heightRange.y).ToList();

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
