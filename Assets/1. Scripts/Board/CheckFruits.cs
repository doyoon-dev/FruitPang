using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckFruits : MonoBehaviour
{
    GetPosition m_getPos;

    public void Init(GetPosition pos)
    {
        m_getPos = pos;
    }

    // ���� �߰�
    public void ResetCheck()
    {
        for (int x = 0; x < m_getPos.x_TileGridSize; x++)
        {
            for (int y = 0; y < m_getPos.y_TileGridSize; y++)
            {
                m_getPos.m_checkFruit[x, y] = false;
            }
        }
    }

    // �̵� �� �ֺ��� ���� ������ �ִ��� üũ
    public void CheckSameFruits(int x, int y, PoolKey poolKey, List<Fruit> list)
    {

        // Ÿ�� ���� �ȿ� ���� ���� ����
        if (!m_getPos.IsBounds(x, y)) { return; }
        if (m_getPos.m_checkFruit[x, y]) { return; }
        if (m_getPos.m_fruits[x, y] == null) { return; }
        if (m_getPos.m_fruits[x, y].m_fruitData.fruitTypePoolKey != poolKey) { return; }
        //if (m_fruits[x, y].m_fruitType != poolKey) { return; }

        m_getPos.m_checkFruit[x, y] = true;
        list.Add(m_getPos.m_fruits[x, y]);

        CheckSameFruits(x + 1, y, poolKey, list);
        CheckSameFruits(x - 1, y, poolKey, list);
        CheckSameFruits(x, y + 1, poolKey, list);
        CheckSameFruits(x, y - 1, poolKey, list);
    }

    //bool IsBounds(int x, int y)
    //{
    //    return x >= 0 && y >= 0 && x < x_TileGridSize && y < y_TileGridSize;
    //}
}
