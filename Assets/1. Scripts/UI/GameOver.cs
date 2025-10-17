using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public interface ISetGameOverBtnClickEvent
{
    void SetGameOverBtnClickEvent();
}

public class GameOver : MonoBehaviour, ISetGameOverBtnClickEvent
{
    public TextMeshProUGUI m_curScoreText;
    public TextMeshProUGUI m_highScoreText;

    public Button m_lobbyBtn;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        GameEnd();
    }

    public void GameEnd()
    {
        //Dictionary<string, int> dic = new Dictionary<string, int>();
        //IDictionary id = ManagerManager.Instance.refManager.Point.GetComponent<IDictionary>();
        //if (id != null)
        //{
        //    dic = id.GetScoreTextDic();
        //}
        //if (dic["CurScore"] >= dic["HighScore"])
        //{
        //    ManagerManager.Instance.scoreManager.SaveHighScore(dic["CurScore"]);
        //}
        //m_curScoreText.text = dic["CurScore"].ToString();
        //m_highScoreText.text = ManagerManager.Instance.scoreManager.LoadHighScore().ToString();
        ManagerManager.Instance.scoreManager.SaveHighScore(ManagerManager.Instance.scoreManager.m_highScore);
        m_curScoreText.text = ManagerManager.Instance.scoreManager.m_curScore.ToString();
        m_highScoreText.text = ManagerManager.Instance.scoreManager.m_highScore.ToString();
    }

    public void SetGameOverBtnClickEvent()
    {
        ManagerManager.Instance.buttonManager.ButtonClickEvent(m_lobbyBtn, ManagerManager.Instance.buttonManager.GoToLobbyScene);
    }
}
