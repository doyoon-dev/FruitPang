using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.UI;

public enum AudioType
{
    BGM,
    SFX
}
public enum BgmClip
{
    MainTheme,
    LobbyTheme
}
public enum SfxClip
{
    Match,
    Explosion,
    Timer,
    Exit,
    Touch
}

public interface IBgmOnOff
{
    void BgmOnOff(bool on);
}

public interface ISfxOnOff
{
    void SfxOnOff(bool on);
}

public class SoundManager : MonoBehaviour, IBgmOnOff, ISfxOnOff
{
    const int MaxVolumeLevel = 10;
    const int MaxSfcPlayCnt = 3;

    public static SoundManager Instance = null;

    public AudioMixer m_audioMixer;

    public AudioSource[] m_audio;

    public AudioClip[] m_bgmClips;
    public AudioClip[] m_sfxClips;

    public Slider m_masterSlider;
    public Slider m_bgmSlider;
    public Slider m_sfxSlider;

    Dictionary<SfxClip, int> m_sfxPlayList = new Dictionary<SfxClip, int>();

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

    // ���� ����
    public void SetVolume(int level)
    {
        VolumeSetting((int)AudioType.BGM, level);
        VolumeSetting((int)AudioType.SFX, level);
    }

    public void VolumeSetting(int typeNum, int level)
    {
        if (level < 0) level = 0;
        if (level > MaxVolumeLevel) level = MaxVolumeLevel;
        m_audio[typeNum].volume = (float)level / MaxVolumeLevel;
    }

    public void SetMasterVolume(float value)
    {
        float sound = value;
        float db = Mathf.Log10(Mathf.Max(sound, 0.0001f)) * 20;

        // Mathf.InverseLerp(minDB, 0, db) : db�� log ���̹Ƿ� Mathf.Log10(1) = 0
        // ���� -80 ~ 0������ ������ �ۼ�Ʈ ������ ��ȯ
        // Mathf.InverseLerp(minDB, 0, db) : db ���� -80�� �� 0, db ���� 0�� �� 1�� ��ȯ
        // Lerp : a, b ������ �ۼ�Ʈ ������ ������ ��ȯ
        db = Mathf.Lerp(-80, -10, Mathf.InverseLerp(-80, 0, db));
        m_audioMixer.SetFloat("Master", db);
    }

    public void SetBGMVolume(float value)
    {
        float sound = value;
        float db = Mathf.Log10(Mathf.Max(sound, 0.0001f)) * 20;
        db = Mathf.Lerp(-80, -10, Mathf.InverseLerp(-80, 0, db));
        m_audioMixer.SetFloat("BGM", db);
    }

    public void SetSFXVolume(float value)
    {
        float sound = value;
        float db = Mathf.Log10(Mathf.Max(sound, 0.0001f)) * 20;
        db = Mathf.Lerp(-80, -10, Mathf.InverseLerp(-80, 0, db));
        m_audioMixer.SetFloat("SFX", db);
    }

    // �Ҹ� ���
    public void PlayBgm()
    {
        //m_audio[(int)AudioType.BGM].clip = m_bgmClips[(int)bgm];
        m_audio[(int)AudioType.BGM].Play();
    }

    public void SetBGMSound(BgmClip bgm)
    {
        m_audio[(int)AudioType.BGM].Stop();
        m_audio[(int)AudioType.BGM].clip = m_bgmClips[(int)bgm];
    }

    // ���� ���� ����
    public void PlaySfx(SfxClip sfx)
    {
        if (m_sfxPlayList.ContainsKey(sfx))
        {
            if (m_sfxPlayList[sfx] >= MaxSfcPlayCnt)
            {
                return;
            }
            else
            {
                m_sfxPlayList[sfx]++;
            }
        }
        else
        {
            m_sfxPlayList.Add(sfx, 1);
        }
        m_audio[(int)AudioType.SFX].PlayOneShot(m_sfxClips[(int)sfx]);
        StartCoroutine(RemoveSfxPlayList(sfx, m_sfxClips[(int)sfx].length));
    }

    IEnumerator RemoveSfxPlayList(SfxClip sfx, float length)
    {
        yield return new WaitForSeconds(length);
        if (m_sfxPlayList[sfx] > 1)
        {
            m_sfxPlayList[sfx]--;
        }
        else
        {
            m_sfxPlayList.Remove(sfx);
        }
    }

    public void BgmOnOff(bool on)
    {
        SoundOnOff(AudioType.BGM, on);
    }

    public void SfxOnOff(bool on)
    {
        SoundOnOff(AudioType.SFX, on);
    }

    void SoundOnOff(AudioType at, bool on)
    {
        if (on)
        {
            m_audio[(int)at].mute = false;
        }
        else
        {
            m_audio[(int)at].mute = true;
        }
    }

    public void OnGameSceneLoaded()
    {
        Transform canvasTrans = ManagerManager.Instance.refManager.Canvas.transform;
        Slider[] sl = canvasTrans.Find("SoundSetting").GetComponentsInChildren<Slider>(true);
        foreach (Slider s in sl)
        {
            if (s.name == "MasterSlider")
            {
                m_masterSlider = s;
                SliderEvent(m_masterSlider, SetMasterVolume);
            }
            if (s.name == "BgmSlider")
            {
                m_bgmSlider = s;
                SliderEvent(m_bgmSlider, SetBGMVolume);
            }
            if (s.name == "SfxSlider")
            {
                m_sfxSlider = s;
                SliderEvent(m_sfxSlider, SetSFXVolume);
            }
        }
    }

    public void SliderEvent(Slider sl, UnityAction<float> act)
    {
        sl.onValueChanged.RemoveAllListeners();
        sl.onValueChanged.AddListener(act);
        act.Invoke(sl.value);
    }
}
