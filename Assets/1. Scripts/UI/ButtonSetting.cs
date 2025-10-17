using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public interface ISetBtnColorText
{
    void SetBtnColorText(bool on);
}

public class ButtonSetting : MonoBehaviour, ISetBtnColorText
{
    public Image m_image;
    public Button m_btn;
    public TextMeshProUGUI m_btnText;

    Color m_orgColorVal;


    // Start is called before the first frame update
    void Start()
    {
        m_orgColorVal = m_btn.colors.normalColor;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetBtnColorText(bool on)
    {
        Color c = m_image.color;
        if (on)
        {
            c = new Color(m_orgColorVal.r, m_orgColorVal.g, m_orgColorVal.b);
            m_image.color = c;
            m_btnText.text = "On";
        }
        else
        {
            Color color = m_btn.colors.pressedColor;
            c = new Color(color.r, color.g, color.b);
            m_image.color = c;
            m_btnText.text = "Off";
        }
    }
}
