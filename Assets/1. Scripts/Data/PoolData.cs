using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PoolKey
{
    Blue1 = 0,
    Blue2,
    Green1,
    Green2,
    Orange1,
    Orange2,
    Red1,
    Red2,
    Yellow1,
    Yellow2,
    Blue_Bomb,
    Green_Bomb,
    Orange_Bomb,
    Red_Bomb,
    Yellow_Bomb,
    Blue_Explosion,
    Green_Explosion,
    Orange_Explosion,
    Red_Explosion,
    Yellow_Explosion,
    Tile
}

[CreateAssetMenu(fileName = "PoolData", menuName = "Pooling/PoolData")]
public class PoolData : ScriptableObject
{
    public PoolKey poolKey;
    public GameObject prefab;
}
