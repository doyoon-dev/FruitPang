using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IDestroyFruitAnimStart
{
    void DestroyFruitAnimStart();
}

public interface ICheckMovableDirection
{
    public Vector2Int CheckMovableDirection(Vector2 dragDir);
}

public interface IGetFruitGridPos
{
    Vector2Int m_gridPos { get; }
}

public interface ISetFruitGridPos
{
    void SetGridPosition(Vector2Int gridPos);
}

public enum ColorType
{
    Blue = 0,
    Green,
    Orange,
    Red,
    Yellow
}

public enum Category
{
    Fruit,
    Bomb
}


public interface IPushGridPos
{
    void PushGridPos();
}

public interface IDelayDestroy
{
    void DelayDestroy();
}

public class Fruit : MonoBehaviour, ISetFruitGridPos, IGetFruitGridPos, ICheckMovableDirection, IDelayDestroy//IPushGridPos, IDestroyFruitAnimStart, 
{
    public Vector2Int m_gridPos { get; private set; }

    public FruitData m_fruitData;

    public SpriteRenderer[] m_spriteArr;

    //public Transform gridText;

    //public GameObject m_GridText;


    // ���� �̵� ���� üũ
    public Vector2Int CheckMovableDirection(Vector2 dragDir)
    {
        // �밢���� �� üũ �ʿ�
        // �¿�� �巡�� ���� ��
        if (Mathf.Abs(dragDir.x) > Mathf.Abs(dragDir.y))
        {
            if (dragDir.x > 0) { return Vector2Int.right; }
            else { return Vector2Int.left; }
        }
        // ���Ϸ� �巡�� ���� ��
        else
        {
            if (dragDir.y > 0) { return Vector2Int.down; }
            else { return Vector2Int.up; }
        }

    }

    // ���� ��ǥ ����
    public void SetGridPosition(Vector2Int gridPos)
    {
        m_gridPos = gridPos;
    }

    public void DelayDestroy()
    {
        Invoke("PushFruit", 0.8f);
    }

    public void PushFruit()
    {
        ManagerManager.Instance.objectPoolManager.Return(gameObject);
    }

}
