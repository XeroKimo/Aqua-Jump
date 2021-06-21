using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Aqua Jump/Platform Chances")]
public class LevelDescription : ScriptableObject
{
    [Serializable]
    public struct PlatformChances
    {
        public BasePlatform platform;
        public float chance;
    }

    [Serializable]
    public struct PowerUpChances
    {
        public BasePowerUpObject powerUp;
        public float chance;
    }

    public List<PlatformChances> platforms;
    public List<PowerUpChances> powerUps;

    public float minimumHeightDifference;
    public Vector2 heightRange;

    public BasePlatform GetRandomPlatform()
    {
        float value = UnityEngine.Random.Range(0.0f, 1.0f);
        foreach(PlatformChances chance in platforms)
        {
            value -= chance.chance;

            if(value < 0)
                return chance.platform;
        }

        return null;
    }

    public BasePowerUpObject GetRandomPowerUp()
    {
        float value = UnityEngine.Random.Range(0.0f, 1.0f);
        foreach(PowerUpChances chance in powerUps)
        {
            value -= chance.chance;

            if(value < 0)
                return chance.powerUp;
        }

        return null;
    }
}
