using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Aqua Jump/Platform Chances")]
public class PlatformChances : ScriptableObject
{
    [Serializable]
    public struct Chances
    {
        public BasePlatform platform;
        public float chance;
    }

    public List<Chances> platforms;
    public int spawnCount;
    public Vector2 heightRange;

    public BasePlatform GetRandomPlatform()
    {
        float value = UnityEngine.Random.Range(0.0f, 1.0f);
        foreach(Chances chance in platforms)
        {
            value -= chance.chance;

            if(value < 0)
                return chance.platform;
        }

        return null;
    }
}
