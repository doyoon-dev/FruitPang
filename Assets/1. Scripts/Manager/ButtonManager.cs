using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    public static ButtonManager Instance;

    public PauseWindow m_pauseWindow;
    public Button m_playBtn;
    public Button m_restartBtn;
    public Button m_goHomeBtn;
    public ButtonSetting m_bgmOnOffBtn;
    public ButtonSetting m_sfxOnOffBtn;
    public GameObject m_soundSetBtn;

    bool m_isBgmOn = true;
    bool m_isSfxOn = true;
    // Start is called before the first frame update
    void Awake()
    {
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

    // Update is called once per frame
    void Update()
    {
        
    }

    // ���� �ٲ� �� ���� �ֱ�
    public void OnGameSceneLoaded()
    {
        Pause p = FindObjectOfType<Pause>();
        m_pauseWindow = p.GetComponentInChildren<PauseWindow>(true);
        Button[] btns = m_pauseWindow.transform.Find("Btn").GetComponentsInChildren<Button>(true);
        foreach (Button btn in btns)
        {
            if (btn.name == "PlayBtn") m_playBtn = btn;
            if (btn.name == "RestartBtn") m_restartBtn = btn;
            if (btn.name == "GoHomeBtn") m_goHomeBtn = btn;
        }
        GameObject btnGo = m_pauseWindow.transform.Find("Btn").gameObject;
        
        m_bgmOnOffBtn = btnGo.transform.Find("BGMOption").GetComponentInChildren<ButtonSetting>(true);
        m_sfxOnOffBtn = btnGo.transform.Find("SFXOption").GetComponentInChildren<ButtonSetting>(true);
        //Transform canvasTrans = FindObjectOfType<Canvas>().transform;
        //m_soundSetBtn = canvasTrans.Find("SoundSetting").gameObject;
        m_soundSetBtn = ManagerManager.Instance.refManager.Canvas.transform.Find("SoundSetting").gameObject;

        ISetButtonClickEvent isbce = ManagerManager.Instance.refManager.Pause.GetComponent<ISetButtonClickEvent>();
        if (isbce != null)
        {
            isbce.SetButtonClickEvent();
        }

        ISetGameOverBtnClickEvent isgobce = ManagerManager.Instance.refManager.GameOver.GetComponent<ISetGameOverBtnClickEvent>();
        if (isgobce != null)
        {
            isgobce.SetGameOverBtnClickEvent();
        }
    }

    #region Button Event
    // ���� ���� ��ư
    public void StartGameScene()
    {
        SoundManager.Instance.PlaySfx(SfxClip.Touch);
        SceneManager.LoadScene("LoadingScene");
    }

    // �κ�� ���� ��ư
    public void GoToLobbyScene()
    {
        SoundManager.Instance.PlaySfx(SfxClip.Touch);
        m_pauseWindow.gameObject.SetActive(false);
        SceneManager.LoadScene("LobbyScene");
    }

    // �Ͻ����� ��ư
    public void PauseBtnEvent()
    {
        SoundManager.Instance.PlaySfx(SfxClip.Touch);
        m_pauseWindow.gameObject.SetActive(true);
        Time.timeScale = 0;
    }

    // ���� ��ư
    public void PlayBtnEvent()
    {
        SoundManager.Instance.PlaySfx(SfxClip.Touch);
        StopAllCoroutines();
        StartCoroutine(PauseWindowDisable());
    }
    // �Ͻ����� â ����
    IEnumerator PauseWindowDisable()
    {
        RectTransform rect = m_pauseWindow.GetComponent<RectTransform>();
        float delta = 1.2f;
        while (delta > 0)
        {
            delta -= Time.unscaledDeltaTime * 5;
            rect.localScale = new Vector3(delta, delta, delta);
            yield return null;
        }
        rect.localScale = new Vector3(0, 0, 0);
        Time.timeScale = 1.0f;
        m_pauseWindow.gameObject.SetActive(false);
    }

    // ����� ��ư
    public void RestartBtnEvent()
    {
        SoundManager.Instance.PlaySfx(SfxClip.Touch);
        m_pauseWindow.gameObject.SetActive(false);
        ManagerManager.Instance.gameManager.RestartScene();
    }

    // BGM ON OFF ��ư
    public void BgmOnOffBtnEvent()
    {
        SoundOnOffBtn(ref m_isBgmOn, ()=>
        {
            IBgmOnOff ibo = ManagerManager.Instance.soundManager.GetComponent<IBgmOnOff>();
            ibo?.BgmOnOff(m_isBgmOn);

        }, m_bgmOnOffBtn);
    }

    // SFX ON OFF ��ư
    public void SfxOnOffBtnEvent()
    {
        SoundOnOffBtn(ref m_isSfxOn, ()=>
        {
            ISfxOnOff ibo = ManagerManager.Instance.soundManager.GetComponent<ISfxOnOff>();
            ibo?.SfxOnOff(m_isSfxOn);
        }, m_sfxOnOffBtn);
    }

    // ���� ON OFF ��� �Լ� BgmOnOffBtnEvent() , SfxOnOffBtnEvent() ���� ȣ��
    public void SoundOnOffBtn(ref bool on, UnityAction act, ButtonSetting obj)
    {
        SoundManager.Instance.PlaySfx(SfxClip.Touch);
        on = !on;

        act?.Invoke();

        ISetBtnColorText isbct = obj.GetComponent<ISetBtnColorText>();
        isbct?.SetBtnColorText(on);
    }
     
    // ���� ���� ��ư �ѱ�
    public void SoundSettingBtnEvent()
    {
        SoundManager.Instance.PlaySfx(SfxClip.Touch);
        m_soundSetBtn.SetActive(true);
    }
    
    // ���� ���� ��ư ����
    public void SoundSettingBtnExitEvent()
    {
        SoundManager.Instance.PlaySfx(SfxClip.Exit);
        m_soundSetBtn.SetActive(false);
    }
    #endregion

    // �� ���� �� ��ư �̺�Ʈ�� null �� �κ� �̺�Ʈ �ֱ�
    public void ButtonClickEvent(Button btn, UnityAction act)
    {
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(act);
    }
}
