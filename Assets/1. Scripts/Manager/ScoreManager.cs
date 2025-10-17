using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public TextMeshProUGUI m_curScoreText;
    public TextMeshProUGUI m_highScoreText;

    Coroutine m_scoreCor;

    public int m_curScore = 0;
    int m_targetScore = 0;
    public int m_highScore = 0;

    float m_increaseDelay = 0.005f;
    float m_popScale = 1.3f;
    float m_popDuration = 0.005f;

    

    // Start is called before the first frame update
    void Awake()
    {
        //UpdateScoreText();
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadData()
    {
        m_targetScore = 0;
        m_curScore = 0;
        m_highScore = LoadHighScore();
    }

    #region Json으로 점수 저장 함수

    string GetFilePath()
    {
        string folder = Path.Combine(Application.persistentDataPath, "Save");
        // 이미 폴더가 있으면 무시
        Directory.CreateDirectory(folder);
        return Path.Combine(folder, "Score.json");
    }

    // 점수 저장 함수
    // 게임 오버 됐을 때 호출
    public void SaveHighScore(int score)
    {
        ScoreData data = new ScoreData();
        data.HighScore = score;

        string json = JsonUtility.ToJson(data, true);

        // 파일 덮어쓰기
        // 파일 여러 개를 따로 보관할 때 : File.AppenAllText() 사용 또는 파일 이름에 날짜/시간 등을 붙여서 여러 개 저장
        File.WriteAllText(GetFilePath(), json);
    }

    // 게임 시작 시 Point 스크립트 m_highScore에 넣기
    public int LoadHighScore()
    {
        string path = GetFilePath();
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            ScoreData data = JsonUtility.FromJson<ScoreData>(json);
            return data.HighScore;
        }
        else return 0;
    }

    public void DeleteHighScore()
    {
        string path = GetFilePath();
        if (File.Exists(path)) { File.Delete(path); }
    }

    #endregion

    #region 점수 획득 애니메이션

    public void AddScore(int score)
    {
        m_targetScore += score;

        if (m_scoreCor == null)
        {
            m_scoreCor = StartCoroutine(AnimateScore());
        }
    }

    IEnumerator AnimateScore()
    {
        while (m_curScore < m_targetScore)
        {
            m_curScore++;
            UpdateScoreText();
            yield return StartCoroutine(PopText());

            yield return new WaitForSeconds(m_increaseDelay);
        }

        m_scoreCor = null;
        
    }

    IEnumerator PopText()
    {
        Vector3 orgScale = m_curScoreText.transform.localScale;
        Vector3 targetScale = orgScale * m_popScale;

        float delta = 0;
        while (delta < m_popDuration)
        {
            delta += Time.deltaTime;
            float n = delta / m_popDuration;
            m_curScoreText.transform.localScale = Vector3.Lerp(orgScale, targetScale, n);
            yield return null;
        }

        delta = 0;
        while (delta < m_popDuration)
        {
            delta += Time.deltaTime;
            float n = delta / m_popDuration;
            m_curScoreText.transform.localScale = Vector3.Lerp(targetScale, orgScale, n);
            yield return null;
        }
    }

    public void UpdateScoreText()
    {
        if (m_curScoreText != null)
        {
            if (m_curScore >= m_highScore)
            {
                m_highScore = m_curScore;
                m_highScoreText.text = m_highScore.ToString();
                SaveHighScore(m_highScore);
            }
            m_curScoreText.text = m_curScore.ToString();
        }
    }

    #endregion

    public void OnGameSceneLoaded()
    {
        m_curScoreText = GameObject.Find("CurPointText").GetComponent<TextMeshProUGUI>();
        m_highScoreText = GameObject.Find("HighPointText").GetComponent<TextMeshProUGUI>();
        m_curScoreText.text = m_curScore.ToString();
        m_highScoreText.text = m_highScore.ToString();
    }
}
