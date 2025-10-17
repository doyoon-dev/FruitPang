using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental.FileFormat;
using UnityEngine;
using UnityEngine.Events;

public class ObjectPoolManager : MonoBehaviour
{
    // 사용 예시

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

    // 로딩 씬에서 미리 호출해서  한 번에 즉시 초기화 작업 진행 (동기 방식)
    public void InitAllPrefabs(int preloadCnt = 3)
    {
        // 지연 안걸리게 미리 풀에 넣어 두기
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

    // 비동기 식 호출 현재 스크립트에서 호출
    // 현재 실행 되면 5개씩 생성되고 안 들어감
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
        // 과일 생성
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
