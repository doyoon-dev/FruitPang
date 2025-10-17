using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public interface IUpdateCnt
{
    void UpdateCnt();
}

public class Count : MonoBehaviour, IUpdateCnt
{
    int cnt = 0;

    public TextMeshProUGUI text;
    // Start is called before the first m_frame update
    void Start()
    {
        
    }

    // Update is called once per m_frame
    void Update()
    {
        
    }

    public void UpdateCnt()
    {
        cnt++;
        text.text = cnt.ToString();
    }
}
