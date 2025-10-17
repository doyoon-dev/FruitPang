using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public interface IDictionary
{
    Dictionary<string, int> GetScoreTextDic();
}

public interface IGetPoint
{
    void GetPoint(int score);
}

public class Point : MonoBehaviour, IGetPoint, IDictionary
{
    public TextMeshProUGUI m_curScoreText;
    public TextMeshProUGUI m_highScoreText;

    public Dictionary<string, int> m_scoreTextDic = new Dictionary<string, int>();

    int m_curScore = 0;
    int m_highScore = 0;

    // Start is called before the first frame update
    void Start()
    {
        m_highScore = ManagerManager.Instance.scoreManager.LoadHighScore();
        m_curScore = 0;
        m_curScoreText.text = m_curScore.ToString();
        m_highScoreText.text = ManagerManager.Instance.scoreManager.LoadHighScore().ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetPoint(int score)
    {
        if (m_curScore == m_highScore)
        {
            m_highScore += score;
        }
        m_curScore += score;
        Invoke("SetTimePoint", 0.3f);
        //m_curScoreText.text = m_curScore.ToString();
        //m_highScoreText.text = m_highScore.ToString();
    }

    public void SetTimePoint()
    {
        m_curScoreText.text = m_curScore.ToString();
        m_highScoreText.text = m_highScore.ToString();
    }

    IEnumerator AnimText(int score)
    {
        int v = 0;
        while (v < score)
        {
            v++;
            m_curScore += 1;
            m_curScoreText.text = m_curScore.ToString();
            yield return new WaitForSeconds(0.3f);
        }
        m_highScoreText.text = m_highScore.ToString();
    }

    public Dictionary<string, int> GetScoreTextDic()
    {
        m_scoreTextDic.Add("CurScore", m_curScore);
        m_scoreTextDic.Add("HighScore", m_highScore);
        return m_scoreTextDic;
    }
}
