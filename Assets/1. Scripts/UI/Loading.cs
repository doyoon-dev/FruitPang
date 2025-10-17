using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    public Slider m_slider;
    public TextMeshProUGUI m_text;

    public string m_nextSceneName = "GameScene";
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadAsyncScene());
    }

    IEnumerator LoadAsyncScene()
    {
        float timer = 0;

        yield return StartCoroutine(InitSetBGMSound());
        yield return StartCoroutine(InitGameData());
        yield return StartCoroutine(InitObjectPool());

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(m_nextSceneName);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            timer += Time.deltaTime;

            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);

            m_slider.value = Mathf.Lerp(m_slider.value, progress, Time.deltaTime * 5f);

            if (asyncLoad.progress >= 0.9f && timer >= 1.5f)
            {
                m_slider.value = 1f;
                yield return new WaitForSeconds(0.5f);
                asyncLoad.allowSceneActivation = true;
            }
            yield return null;
        }
    }

    IEnumerator InitGameData()
    {
        ManagerManager.Instance.scoreManager.LoadData();
        yield return null;
    }

    IEnumerator InitObjectPool()
    {
        ManagerManager.Instance.objectPoolManager.InitAllPrefabs();
        yield return null;
    }

    IEnumerator InitSetBGMSound()
    {
        ManagerManager.Instance.soundManager.SetBGMSound(BgmClip.MainTheme);
        yield return null;
    }
}

