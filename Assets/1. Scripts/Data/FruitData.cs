using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FruitData", menuName = "Datas/FruitData")]
public class FruitData : ScriptableObject
{
    public int point;
    public Category category;
    //public FruitType type;
    public ColorType colorType;
    public PoolKey fruitTypePoolKey;
    public PoolKey explosionEffectPoolKey;
}
