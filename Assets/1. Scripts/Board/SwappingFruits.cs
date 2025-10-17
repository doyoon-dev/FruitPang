using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwappingFruits : MonoBehaviour
{
    public bool m_checkSwap = false;

    GetPosition m_getPos;

    public void Init(GetPosition pos)
    {
        m_getPos = pos;
    }

    // ���� ��ǥ ����
    public void Swap(Fruit fruitA, Fruit fruitB)
    {
        if (m_checkSwap) return;
        m_checkSwap = true;
        // ���� ��ǥ ��������
        Vector2Int posA = GetFruitGrid(fruitA);
        Vector2Int posB = GetFruitGrid(fruitB);

        m_getPos.m_fruits[posA.x, posA.y] = fruitB;
        m_getPos.m_fruits[posB.x, posB.y] = fruitA;

        SetFruitGrid(fruitA, posB);
        SetFruitGrid(fruitB, posA);
    }

    // ���� ��ǥ ��������
    Vector2Int GetFruitGrid(Fruit fruit)
    {
        IGetFruitGridPos igfgp = fruit.GetComponent<IGetFruitGridPos>();
        if (igfgp != null)
        {
            return igfgp.m_gridPos;
        }
        return Vector2Int.zero;
    }

    // ���� ��ǥ ����
    void SetFruitGrid(Fruit fruit, Vector2Int grid)
    {
        ISetFruitGridPos isfgp = fruit.GetComponent<ISetFruitGridPos>();
        if (isfgp != null)
        {
            isfgp.SetGridPosition(new Vector2Int(grid.x, grid.y));
        }
    }
}
