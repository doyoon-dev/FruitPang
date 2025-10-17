using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static Unity.VisualScripting.Dependencies.Sqlite.SQLite3;
using static UnityEngine.GraphicsBuffer;

public interface IDestroyFruits
{
    void DestroyFruits(List<Fruit> fruitList, UnityAction MovingNewFruitAct);
}

public class DestroytFruit : MonoBehaviour, IDestroyFruits
{
    public Canvas m_canvas;
    GetPosition m_getPos;
    CheckFruits m_checkFruits;
    
    public void Init(GetPosition pos)
    {
        m_getPos = pos;
    }

    public void SetCheckFruits(CheckFruits checkFruits)
    {
        m_checkFruits = checkFruits;
    }

    // 과일 파괴 가능 여부 확인
    public bool CanDestroyFruits(List<Fruit> fruitList)
    {
        m_checkFruits.ResetCheck();
        return fruitList.Count >= 3;
    }

    // 과일 파괴
    public void DestroyFruits(List<Fruit> fruitList, UnityAction MoveNewFruitAct)
    {
        int score = 0;
        bool isBomb = false;
        for (int i = 0; i < fruitList.Count; i++)
        {
            if (fruitList[i].m_fruitData.category == Category.Bomb)
            {
                isBomb = true;
            }
        }
        //Debug.Log("DestroyFruits 호출!!!");
        for (int i = 0; i < fruitList.Count; i++)
        {
            IGetFruitGridPos igfgp = fruitList[i].GetComponent<IGetFruitGridPos>();
            if (igfgp != null)
            {
                m_getPos.m_fruits[igfgp.m_gridPos.x, igfgp.m_gridPos.y] = null;
                m_getPos.m_checkFruit[igfgp.m_gridPos.x, igfgp.m_gridPos.y] = false;
            }
            GameObject obj = ManagerManager.Instance.objectPoolManager.Get(fruitList[i].m_fruitData.explosionEffectPoolKey);
            obj.transform.SetParent(m_canvas.transform, false);

            if (isBomb) ManagerManager.Instance.soundManager.PlaySfx(SfxClip.Explosion);
            else ManagerManager.Instance.soundManager.PlaySfx(SfxClip.Match);

            IGetCanvase igc = obj.GetComponent<IGetCanvase>();
            if (igc != null)
            {
                igc.GetCanvase(m_canvas);
            }
            ISetTarget ist = obj.GetComponent<ISetTarget>();
            if (ist != null)
            {
                ist.SetTarget(fruitList[i].transform);
            }

            IStartExplosionAnim isea = obj.GetComponent<IStartExplosionAnim>();
            if (isea != null)
            {
                isea.StartExplosionAnim(MoveNewFruitAct);
            }


            IDelayDestroy idd = fruitList[i].GetComponent<IDelayDestroy>();
            if (idd != null)
            {
                idd.DelayDestroy();
            }
            score += fruitList[i].m_fruitData.point;

            IBonusTime ibt = ManagerManager.Instance.refManager.Timer.GetComponent<IBonusTime>();
            if (ibt != null)
            {
                ibt.BonusTime(1);
            }
        }

        ManagerManager.Instance.scoreManager.AddScore(score);
    }

    // 파괴할 과일들 리스트

    public List<Fruit> GetDestroyFruitsList(Fruit fruit)
    {
        InitFruitGrid();
        List<Fruit> list = new List<Fruit>();
        IGetFruitGridPos igfgp = fruit.GetComponent<IGetFruitGridPos>();
        if (igfgp != null)
        {
            m_checkFruits.CheckSameFruits(igfgp.m_gridPos.x, igfgp.m_gridPos.y, fruit.m_fruitData.fruitTypePoolKey, list);
        }
        return list;
    }

    void InitFruitGrid()
    {
        for (int y = 0; y < m_getPos.y_TileGridSize; y++)
        {
            for (int x = 0; x < m_getPos.x_TileGridSize; x++)
            {
                if (m_getPos.m_checkFruit[x, y])
                {
                    m_getPos.m_checkFruit[x, y] = false;
                }
            }
        }
    }
}
