using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public interface IOnDead
{
    void OnDead();
}

public interface ISetText
{
    void SetText(int x, int y);
}

public interface ITarget
{
    Transform myTarget { get; }
}

public class ShowGridText : MonoBehaviour, ISetText, IOnDead, ITarget
{
    public TextMeshProUGUI m_text;
    public Transform myTarget { get; private set; }
    // Start is called before the first m_frame update
    void Start()
    {
        
    }

    // Update is called once per m_frame
    void Update()
    {
        if (myTarget != null)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(myTarget.position);
            RectTransform rect = transform.GetComponent<RectTransform>();
            rect.position = screenPos;
        }
    }

    public void Initialize(Transform target)
    {
        myTarget = target;
    }

    public void SetText(int x, int y)
    {
        m_text.text = $"({x}, {y})";
    }

    public void OnDead()
    {
        myTarget = null;
        //ObjectPool.Pool.Push(gameObject);
        ObjectPoolManager.Instance.Return(gameObject);
    }
}
