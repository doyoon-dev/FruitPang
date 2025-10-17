using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental.FileFormat;
using UnityEngine;
using UnityEngine.Events;

public class ObjectPoolManager : MonoBehaviour
{
    // ��� ����

    //public enum ColorType { Red, Green, Blue }
    //public enum EnemyType { Slime, Goblin, Dragon }
    //public enum EffectType { Hit, Explosion, Heal }

    //ColorType color = ColorType.Red;
    //GameObject explosion = ObjectPoolManager.Instance.Get($"Explosion_{color}", transform);

    public static ObjectPoolManager Instance;

    public event UnityAction m_act;

    public List<PoolData> PoolDataList = new List<PoolData>();

    private Dictionary<PoolKey, GameObject> prefabDic = new Dictionary<PoolKey, GameObject>();

    private void Awake()
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
        foreach (var data in PoolDataList)
        {
            prefabDic[data.poolKey] = data.prefab;
        }

        //StartCoroutine(InitAllPrefabsAsync());
    }

    // �ε� ������ �̸� ȣ���ؼ�  �� ���� ��� �ʱ�ȭ �۾� ���� (���� ���)
    public void InitAllPrefabs(int preloadCnt = 3)
    {
        // ���� �Ȱɸ��� �̸� Ǯ�� �־� �α�
        foreach (var data in PoolDataList)
        {
            for (int i = 0; i < preloadCnt; i++)
            {
                //GameObject obj = Instantiate(data.prefab);
                GameObject obj = ObjectPool.Pool.Pull(data.prefab);
                Return(obj);
            }
        }
    }

    // �񵿱� �� ȣ�� ���� ��ũ��Ʈ���� ȣ��
    // ���� ���� �Ǹ� 5���� �����ǰ� �� ��
    public IEnumerator InitAllPrefabsAsync(int preloadCnt = 3)
    {
        foreach (var data in PoolDataList)
        {
            for (int i = 0; i < preloadCnt; i++)
            {
                GameObject obj = ObjectPool.Pool.Pull(data.prefab);
                Return(obj);
                yield return null;
            }
        }

        if (m_act == null)
        {
            IInit ii = ReferenceManager.Inst.Board.GetComponent<IInit>();
            if (ii != null)
            {
                m_act += ii.Init;
            }
            
        }
        m_act?.Invoke();
        m_act = null;
        // ���� ����
        //m_createAct?.Invoke();
    }


    public GameObject Get(PoolKey key, Transform parent = null)
    {
        if (!prefabDic.ContainsKey(key))
        {
            Debug.LogError($"[ObjectPoolManager] No prefab found for key: {key}");
            return null;
        }

        return ObjectPool.Pool.Pull(prefabDic[key], parent);
    }

    public void Return(GameObject obj)
    {
        ObjectPool.Pool.Push(obj);
    }
}
