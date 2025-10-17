using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    static ObjectPool instance = null;
    public static ObjectPool Pool
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ObjectPool>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("ObjectPool");
                    instance = obj.AddComponent<ObjectPool>();
                }
            }
            return instance;
        }
    }

    Dictionary<string, Queue<GameObject>> m_queue = new Dictionary<string, Queue<GameObject>>();
    Dictionary<string, Transform> m_explore = new Dictionary<string, Transform>();

    public GameObject Pull(GameObject org, Transform pos = null)
    {
        string name = org.name;
        if (m_queue.ContainsKey(name) && m_queue[name].Count > 0)
        {
            GameObject obj = m_queue[name].Dequeue();
            obj.SetActive(true);
            obj.transform.SetParent(pos);
            return obj;
        }
        return Instantiate(org, pos);
    }

    public void Push(GameObject org)
    {
        string name = org.name;
        org.SetActive(false);
        if (!m_queue.ContainsKey(name))
        {
            m_queue[name] = new Queue<GameObject>();
            GameObject directory = new GameObject(name);
            directory.transform.SetParent(transform);
            
            m_explore[name] = directory.transform;
        }

        org.transform.SetParent(m_explore[name]);
        m_queue[name].Enqueue(org);
    }
}
