using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public interface ISetButtonClickEvent
{
    void SetButtonClickEvent();
}

public class Pause : MonoBehaviour, ISetButtonClickEvent
{
    public Button m_pauseBtn;
    public Button m_playBtn;
    public Button m_restartBtn;
    public Button m_goHomeBtn;
    public Button m_bgmBtn;
    public Button m_sfxBtn;
    public Button m_soundBtn;
    public Button m_soundSettingBtn;

    // Start is called before the first frame update
    void Start()
    {
        //SetButtonClickEvent();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetButtonClickEvent()
    {
        ManagerManager.Instance.buttonManager.ButtonClickEvent(m_pauseBtn, ManagerManager.Instance.buttonManager.PauseBtnEvent);
        ManagerManager.Instance.buttonManager.ButtonClickEvent(m_playBtn, ManagerManager.Instance.buttonManager.PlayBtnEvent);
        ManagerManager.Instance.buttonManager.ButtonClickEvent(m_restartBtn, ManagerManager.Instance.buttonManager.RestartBtnEvent);
        ManagerManager.Instance.buttonManager.ButtonClickEvent(m_goHomeBtn, ManagerManager.Instance.buttonManager.GoToLobbyScene);
        ManagerManager.Instance.buttonManager.ButtonClickEvent(m_bgmBtn, ManagerManager.Instance.buttonManager.BgmOnOffBtnEvent);
        ManagerManager.Instance.buttonManager.ButtonClickEvent(m_sfxBtn, ManagerManager.Instance.buttonManager.SfxOnOffBtnEvent);
        ManagerManager.Instance.buttonManager.ButtonClickEvent(m_soundBtn, ManagerManager.Instance.buttonManager.SoundSettingBtnEvent);
        ManagerManager.Instance.buttonManager.ButtonClickEvent(m_soundSettingBtn, ManagerManager.Instance.buttonManager.SoundSettingBtnExitEvent);
        ManagerManager.Instance.buttonManager.ButtonClickEvent(m_soundSettingBtn, ManagerManager.Instance.buttonManager.SoundSettingBtnExitEvent);
    }

    
    
}
