using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IBonusTime
{
    void BonusTime(float time);
}

public class Timer : MonoBehaviour, IBonusTime
{
    public float m_playTime = 10.0f;
    public float m_checkTime = 0;
    public Slider m_slider;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(TimeCheck(0));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartTimeCheck(float bonus)
    {
        StopAllCoroutines();
        StartCoroutine(TimeCheck(bonus));
    }

    IEnumerator TimeCheck(float bonus)
    {
        m_checkTime -= bonus;
        if(m_checkTime < 0 ) { m_checkTime = 0; }
        while (m_checkTime < m_playTime)
        {
            m_checkTime += Time.deltaTime;
            m_slider.value = 1 - (m_checkTime / m_playTime);
            yield return null;
        }
        m_checkTime = 0;
        IGameOver igo = ManagerManager.Instance.gameManager.GetComponent<IGameOver>();
        if (igo != null)
        {
            igo.GameOver();
        }
    }

    public void BonusTime(float bonus)
    {
        StartTimeCheck(bonus);
    }
}
