using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

public interface ICheckDestroyFruitCnt
{
    void CheckDestroyFruitCnt();
}

public interface IMovingNewFruits
{
    void MovingNewFruits();
}

public class FruitMovement : MonoBehaviour, IMovingNewFruits, ICheckDestroyFruitCnt
{
    GetPosition m_getPos;
    SwappingFruits m_swappingFruits;
    CreateFruit m_createFruit;
    DestroytFruit m_destroyFruit;

    public float m_moveSpeed = 1.0f;

    public Dictionary<int, Vector2Int> m_changeFruitsOrgGridPos = new Dictionary<int, Vector2Int>();

    protected Dictionary<Fruit, Coroutine> m_checkCoroutine = new Dictionary<Fruit, Coroutine>();

    List<Fruit> m_destroyFruitsList = new List<Fruit>();
    List<Fruit> m_moveDownFruits = new List<Fruit>();

    public int m_checkCnt = 0;
    int m_destroyCnt = 0;
    int m_movingDownFruitsCnt = 0;
    int m_check = 0;

    public void Init(GetPosition pos)
    {
        m_getPos = pos;
    }

    public void SetSwappingFruits(SwappingFruits sf)
    {
        m_swappingFruits = sf;
    }

    public void SetCreateFruit(CreateFruit cf)
    {
        m_createFruit = cf;
    }

    public void SetDestroyFruit(DestroytFruit df)
    {
        m_destroyFruit = df;
    }

    // 과일 이동
    public void MoveFruit(Fruit fruit, Fruit targetFruit, UnityAction<Fruit, Fruit> done)
    {
        if (m_checkCoroutine.ContainsKey(fruit) && m_checkCoroutine[fruit] != null)
        {
            StopCoroutine(m_checkCoroutine[fruit]);
        }
        Coroutine c = StartCoroutine(SwapMovingFruit(fruit, targetFruit, done));
        m_checkCoroutine[fruit] = c;
    }

    // 타겟 방향으로 과일 이동
    IEnumerator SwapMovingFruit(Fruit fruit, Fruit targetFruit, UnityAction<Fruit, Fruit> done)
    {
        //m_checkSwap = false;
        Vector2Int targetPos = Vector2Int.zero;

        IGetFruitGridPos igfgp = targetFruit.GetComponent<IGetFruitGridPos>();
        if (igfgp != null)
        {
            targetPos = igfgp.m_gridPos;
        }

        Vector3 dir = m_getPos.GetTilePosition(targetPos.x, targetPos.y);

        Transform trans = fruit.transform;
        Vector3 pos = trans.position;

        float delta = 0;
        float duration = 1.0f;

        while (delta < duration)
        {
            delta += Time.deltaTime * m_moveSpeed;
            fruit.transform.position = Vector3.Lerp(pos, dir, Mathf.Clamp01(delta));
            yield return null;
        }
        fruit.transform.position = dir;
        done?.Invoke(fruit, targetFruit);
    }

    
    // 이동 완료 알림 함수
    public void MoveDone(Fruit fruit, Fruit targetFruit)
    {
        m_swappingFruits.Swap(fruit, targetFruit);
        
        m_checkCoroutine.Remove(fruit);

        List<Fruit> fruitList = new List<Fruit>();
        m_checkCnt++;

        fruitList = m_destroyFruit.GetDestroyFruitsList(fruit);
        if (m_destroyFruit.CanDestroyFruits(fruitList))
        {
            m_destroyFruitsList.AddRange(fruitList);
        }

        if (m_checkCnt >= 2)
        {
            if (m_destroyFruitsList.Count > 0)
            {
                // 파괴 애니메이션 실행
                m_destroyFruit.DestroyFruits(m_destroyFruitsList.Distinct().ToList(), MovingNewFruits);
            }
            else
            {
                // 되돌리기
                m_swappingFruits.m_checkSwap = false;
                Fruit fruit1 = m_getPos.m_fruits[m_changeFruitsOrgGridPos[0].x, m_changeFruitsOrgGridPos[0].y];
                Fruit fruit2 = m_getPos.m_fruits[m_changeFruitsOrgGridPos[1].x, m_changeFruitsOrgGridPos[1].y];
                MoveFruit(fruit1, fruit2, null);
                MoveFruit(fruit2, fruit1, null);
                m_swappingFruits.Swap(fruit1, fruit2);
                m_checkCnt = 0;
            }
            m_destroyFruitsList.Clear();
        }
    }
    
    public void CheckDestroyFruitCnt()
    {
        m_destroyCnt++;
    }


    public void WaitFruitDestroyAnim(int destroytListCnt, UnityAction done)
    {
        StartCoroutine(WaitFruitAnim(destroytListCnt, done));
    }

    IEnumerator WaitFruitAnim(int destroytListCnt, UnityAction done)
    {
        while (destroytListCnt > m_destroyCnt)
        {
            yield return null;
        }
        m_destroyCnt = 0;
        done?.Invoke();
        
    }

    public void MovingNewFruits()
    {
        Dictionary<int, List<Fruit>> dic = new Dictionary<int, List<Fruit>>();

        dic = m_createFruit.CreateNewFruits();

        m_moveDownFruits.Clear();
        m_check = 0;

        foreach (List<Fruit> val in dic.Values)
        {
            int y = val.Count;

            m_movingDownFruitsCnt += y;
        }

        foreach (var kvp in dic)
        {
            int key = kvp.Key;
            int val = kvp.Value.Count;

            int maxVal = m_createFruit.m_nullGridDic[key].Max();

            for (int j = 0; j < val; j++)
            {
                // fruits 리스트에 저장한 과일들을
                // 제일 아래에 있는 빈칸부터 한 칸 씩 채우기
                int x = key;
                int y = maxVal - j;

                m_getPos.m_fruits[x, y] = dic[key][j].GetComponent<Fruit>();

                ISetFruitGridPos ifi = dic[key][j].GetComponent<ISetFruitGridPos>();
                if (ifi != null)
                {
                    ifi.SetGridPosition(new Vector2Int(x, y));
                }
                MoveFruit(dic[key][j], m_getPos.m_fruits[x, y], MoveDownFruits);
            }
        }
    }


    // 과일들이 새로 생성된 후 아래로 이동하는 함수
    // MovingNewFruits() 함수에서 실행
    public void MoveDownFruits(Fruit fruit, Fruit targetFruit)
    {
        m_check++;

        List<Fruit> fruitList = new List<Fruit>();
        fruitList = m_destroyFruit.GetDestroyFruitsList(fruit);
        //Debug.Log($"[Check:{m_check}/{m_movingDownFruitsCnt}] Fruit: {fruit.name} matched: {CanDestroyFruits(fruitList)}");
        if (m_destroyFruit.CanDestroyFruits(fruitList))
        {
            m_moveDownFruits.AddRange(fruitList);
        }
        if (m_check >= m_movingDownFruitsCnt)
        {
            if (m_moveDownFruits.Count > 0)
            {
                //Debug.Log($"[Check:{m_check}/{m_movingDownFruitsCnt}] Fruit: {fruit.name} matched: {CanDestroyFruits(fruitList)}");
                m_destroyFruit.DestroyFruits(m_moveDownFruits.Distinct().ToList(), MovingNewFruits);
            }
            m_movingDownFruitsCnt = 0;
        }
    }
}
