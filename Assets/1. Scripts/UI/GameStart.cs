using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStart : MonoBehaviour
{
    public GameObject m_logo;
    public GameObject m_btnObj;
    public TextMeshProUGUI m_btnText;
    public Button m_startBtn;

    // Start is called before the first frame update
    void Start()
    {
        ManagerManager.Instance.soundManager.SetBGMSound(BgmClip.LobbyTheme);
        ManagerManager.Instance.soundManager.PlayBgm();
        m_startBtn.onClick.RemoveAllListeners();
        m_startBtn.onClick.AddListener(ManagerManager.Instance.buttonManager.StartGameScene);
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }
        StartCoroutine(MovingLogo());
    }

    public void StartGameScene()
    {
        SceneManager.LoadScene("LoadingScene");
    }

    IEnumerator MovingLogo()
    {
        RectTransform rect = m_logo.GetComponent<RectTransform>();
        Vector2 startPos = rect.anchoredPosition;
        Vector2 endPos = Vector2.zero;
        float duration = 1.0f;
        float delta = 0;
        while (delta < duration)
        {
            delta += Time.deltaTime;
            float t = Mathf.Clamp01(delta / duration);
            float easedT = 1f - Mathf.Pow(1f - t, 3f);

            rect.anchoredPosition = Vector3.Lerp(startPos, new Vector2(0, -5), easedT);
            yield return null;
        }

        float bounceHeight = 20f;
        float bounceDuration = 1f;
        delta = 0;
        while (delta < bounceDuration)
        {
            delta += Time.deltaTime;
            float t = Mathf.Clamp01(delta / duration);

            float offsetY = Mathf.Sin(t * Mathf.PI) * bounceHeight;

            rect.anchoredPosition = endPos + Vector2.up * offsetY;
            if (Mathf.Approximately(rect.anchoredPosition.y, 0))
            {
                break;
            }

            yield return null;
        }
        rect.anchoredPosition = endPos;
        StartCoroutine(FadeInBtn());
    }


    IEnumerator FadeInBtn()
    {
        Image image = m_btnObj.GetComponent<Image>();
        float delta = 0.0f;
        float duration = 1.0f;
        while (delta < duration)
        {
            delta += Time.deltaTime;
            Color btnColor = image.color;
            Color textColor = m_btnText.color;
            btnColor.a = delta;
            textColor.a = delta;
            image.color = btnColor;
            m_btnText.color = textColor;

            yield return null;
        }
        Button btn = m_btnObj.GetComponent<Button>();
        yield return new WaitForSeconds(0.5f);
        btn.interactable = true;
    }
}
