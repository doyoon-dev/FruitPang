using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetPosition
{
    public int x_TileGridSize = 6;
    public int y_TileGridSize = 9;
    public float tileSize = 0.8f;

    public Fruit[,] m_fruits;
    public bool[,] m_checkFruit;

    // 새로 추가
    public GetPosition(int x, int y)
    {
        x_TileGridSize = x;
        y_TileGridSize = y;
        m_fruits = new Fruit[x, y];
        m_checkFruit = new bool[x, y];
    }

    // 생성할 타일의 위치 가져오기
    public Vector3 GetTilePosition(int x, int y)
    {
        Vector3 centerPos = new Vector3((tileSize / 2) * (x_TileGridSize - 1), (tileSize / 2) * (y_TileGridSize - 1));
        return new Vector3(x * tileSize - centerPos.x, (y * -tileSize + centerPos.y) - 1);
    }

    // CheckFruits에서 옮겨옴
    public bool IsBounds(int x, int y)
    {
        return x >= 0 && y >= 0 && x < x_TileGridSize && y < y_TileGridSize;
    }
}
