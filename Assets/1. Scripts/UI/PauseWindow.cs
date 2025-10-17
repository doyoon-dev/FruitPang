using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PauseWindow : MonoBehaviour
{
    public GameObject m_playBtn;
    public GameObject m_restartBtn;
    public GameObject m_goHomeBtn;
    public GameObject m_bgmOnOffBtn;
    public GameObject m_sfxOnOffBtn;
    public GameObject m_soundSetBtn;


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
        StartCoroutine(PauseWindowEnable());
    }

    IEnumerator PauseWindowEnable()
    {
        RectTransform rect = GetComponent<RectTransform>();
        float delta = 0.0f;
        while (delta < 1f)
        {
            // Time.unscaledDeltaTime : 일시정지 상태에서도 작동
            delta += Time.unscaledDeltaTime * 5;
            rect.localScale = new Vector3(delta, delta, delta);
            yield return null;
        }
        rect.localScale = new Vector3(1, 1, 1);
    }

    IEnumerator PauseWindowDisable()
    {
        RectTransform rect = GetComponent<RectTransform>();
        float delta = 1.2f;
        while (delta > 0)
        {
            delta -= Time.unscaledDeltaTime * 5;
            rect.localScale = new Vector3(delta, delta, delta);
            yield return null;
        }
        rect.localScale = new Vector3(0, 0, 0);
        Time.timeScale = 1.0f;
        gameObject.SetActive(false);
    }

    public void PlayBtnEvent()
    {
        StopAllCoroutines();
        StartCoroutine(PauseWindowDisable());
    }

    //public void RestartBtnEvent()
    //{
    //    ManagerManager.Instance.gameManager.RestartScene();
    //}

    //public void GoHomeBtnEvent()
    //{

    //}

    //public void BgmOnOffBtnEvent()
    //{
    //    SoundOnOffBtn(ref m_isBgmOn, () =>
    //    {
    //        IBgmOnOff ibo = ManagerManager.Instance.soundManager.GetComponent<IBgmOnOff>();
    //        ibo?.BgmOnOff(m_isBgmOn);
    //    }, m_bgmOnOffBtn);
    //}

    //public void SfxOnOffBtnEvent()
    //{
    //    SoundOnOffBtn(ref m_isSfxOn, () =>
    //    {
    //        ISfxOnOff ibo = ManagerManager.Instance.soundManager.GetComponent<ISfxOnOff>();
    //        ibo?.SfxOnOff(m_isSfxOn);
    //    }, m_sfxOnOffBtn);
    //}

    //public void SoundOnOffBtn(ref bool on, UnityAction act, GameObject obj)
    //{
    //    on = !on;

    //    act?.Invoke();

    //    ISetBtnColorText isbct = obj.GetComponent<ISetBtnColorText>();
    //    isbct?.SetBtnColorText(on);
    //}

    //public void SoundSettingBtnEvent()
    //{
    //    m_soundSetBtn.SetActive(true);
    //}

    //public void SoundSettingBtnExitEvent()
    //{
    //    m_soundSetBtn.SetActive(false);
    //}
}
