using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 로딩 씬에서 과일들 로딩 애니메이션
public class AnimationFruit : MonoBehaviour
{
    public GameObject[] m_fruits;
    int num = 0;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(AnimFruits());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator AnimFruits()
    {
        RectTransform rect = m_fruits[num].GetComponent<RectTransform>();
        Vector2 orgPos = rect.anchoredPosition;
        float delta = 0;
        float height = 25;
        float duration = 1;
        while (delta < duration)
        {
            delta += Time.deltaTime;
            float t = Mathf.Clamp01(delta / duration);
            float offsetY = Mathf.Sin(t * Mathf.PI) * height;
            rect.anchoredPosition = orgPos + Vector2.up * offsetY;
            yield return null;
        }

        rect.anchoredPosition = orgPos;
        num++;
        if (num >= m_fruits.Length)
        {
            num = 0;
        }
        StartCoroutine(AnimFruits());
    }
}
