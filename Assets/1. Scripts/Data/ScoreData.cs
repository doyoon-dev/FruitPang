using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// JsonUtility가 직렬화 하려면 [System.Serializable] 필요
// JsonUtility는 [System.Serializable]이 붙은 클래스만 JSON으로 변환 가능
[System.Serializable]
public class ScoreData
{
    public int HighScore;
}
