using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;



public class CreateFruit : MonoBehaviour
{
    public Dictionary<int, List<int>> m_nullGridDic = new Dictionary<int, List<int>>();

    public Transform m_FruitPos;
    public Transform m_TilePos;

    public GameObject m_Tile;

    GetPosition m_getPos;

    Vector2Int[] m_patternShapeOffset = new Vector2Int[]
    {
        new Vector2Int(0, -2),       // Up
        new Vector2Int(0, 3),       // Down

        new Vector2Int(-2, 0),       // Left
        new Vector2Int(3, 0),       // Right
        
        new Vector2Int(-1, 1),      // LeftDown
        new Vector2Int(-1, -1),     // LeftUp
        new Vector2Int(2, 1),       // RightDown
        new Vector2Int(2, -1),       // RightUp

        new Vector2Int(-1, 2),      // LeftDown
        new Vector2Int(-1, -1),     // LeftUp
        new Vector2Int(1, 2),       // RightDown
        new Vector2Int(1, -1)       // RightUp
    };

    public enum PatternState
    {
        None = -1,

        Up,
        Down,

        Left,
        Right,

        LShapeWidth_LeftDown,
        LShapeWidth_LeftUp,
        LShapeWidth_RightDown,
        LShapeWidth_RightUp,

        LShapeHeight_LeftDown,
        LShapeHeight_LeftUp,
        LShapeHeight_RightDown,
        LShapeHeight_RightUp
    }

    public enum PatternShape
    {
        None,
        Vertical,
        Horizontal,
        WidthLShape,
        HeightLShape
    }


    public PoolKey[] FruitsPoolKey = new PoolKey[]
    {
        PoolKey.Blue1,
        PoolKey.Blue2,
        PoolKey.Green1,
        PoolKey.Green2,
        PoolKey.Orange1,
        PoolKey.Orange2,
        PoolKey.Red1,
        PoolKey.Red2,
        PoolKey.Yellow1,
        PoolKey.Yellow2
    };

    public PoolKey[] BombsPoolKey = new PoolKey[]
    {
        PoolKey.Blue_Bomb,
        PoolKey.Green_Bomb,
        PoolKey.Orange_Bomb,
        PoolKey.Red_Bomb,
        PoolKey.Yellow_Bomb
    };
    public GameObject[] DestroyEffectPrefabs;

    //public Fruit[,] m_fruits;

    //protected bool[,] m_checkFruit = new bool[x_TileGridSize, y_TileGridSize];
    bool m_checkBomb = false;


    // 초기화
    public void Init(GetPosition pos)
    {
        m_getPos = pos;
    }

    public void CreateInit()
    {
        CreateTile();
        CretateInitialFruits();
    }

    public void CreateTile()
    {
        for (int i = 0; i < m_getPos.y_TileGridSize; i++)
        {
            for (int j = 0; j < m_getPos.x_TileGridSize; j++)
            {
                GameObject obj = ManagerManager.Instance.objectPoolManager.Get(PoolKey.Tile);
                obj.transform.position = m_getPos.GetTilePosition(j, i);
            }
        }
    }

    // 과일 생성
    public Dictionary<int, List<Fruit>> CreateNewFruits()
    {
        // 과일 파괴되고 아래로 안내려오는 과일이 있음
        // movingFruitsDic : 파괴된 과일 윗쪽에 있는 과일들 저장
        Dictionary<int, List<Fruit>> movingFruitsDic = new Dictionary<int, List<Fruit>>();

        // 이 함수가 여러 번 호출되는 경우, 이전 호출의 데이터가 남아 있으면 타일 데이터가 꼬일 수 있음.
        // 함수 흐름상 데이터를 넣기 전에 초기화하는 게 더 직관적이고 안정적.
        // 혹시 중간에 예외가 발생하거나, 조기 리턴하게 되면 Clear()가 호출되지 않을 수 있음.
        m_nullGridDic.Clear();

        
        // 타일이 null 인 부분 저장
        for (int y = 0; y < m_getPos.y_TileGridSize; y++)
        {
            for (int x = 0; x < m_getPos.x_TileGridSize; x++)
            {
                if (m_getPos.m_fruits[x, y] == null)
                {
                    if (!m_nullGridDic.ContainsKey(x))
                    {
                        m_nullGridDic.Add(x, new List<int>());
                    }
                    m_nullGridDic[x].Add(y);
                }
            }
        }

        // 파괴된 과일 윗쪽에 있는 과일들 저장

        // Key 값만 사용할 때
        //foreach (int key in m_nullGridDic.Keys)

        // Key 값, Value 값 둘 다 사용할 때
        foreach (var kvp in m_nullGridDic)
        {
            int x = kvp.Key;
            int maxVal = kvp.Value.Max();

            movingFruitsDic.Add(x, new List<Fruit>());

            for (int y = maxVal - 1; y >= 0; y--)
            {
                Fruit f = m_getPos.m_fruits[x, y];
                if (f != null)
                {
                    movingFruitsDic[x].Add(f);
                }
            }
        }

        // 과일 생성
        for (int i = 0; i < m_getPos.x_TileGridSize; i++)
        {
            if (!m_nullGridDic.ContainsKey(i)) continue;
            
        }
        foreach (var kvp in m_nullGridDic)
        {
            int x = kvp.Key;
            int cnt = kvp.Value.Count;

            for (int i = 0; i < cnt; i++)
            {
                int rnd = 0;
                GameObject fruit = null;
                if (CheckBomb() && PickBomb())
                {
                    rnd = Random.Range(0, BombsPoolKey.Length);
                    fruit = ManagerManager.Instance.objectPoolManager.Get(BombsPoolKey[rnd]);
                }
                else
                {
                    rnd = Random.Range(0, FruitsPoolKey.Length);
                    fruit = ManagerManager.Instance.objectPoolManager.Get(FruitsPoolKey[rnd]);
                }
                
                fruit.transform.position = m_getPos.GetTilePosition(x, -(i + 1));
                fruit.transform.SetParent(m_FruitPos);
                Fruit f = fruit.GetComponent<Fruit>();
                movingFruitsDic[x].Add(f);
            }
        }
        m_checkBomb = false;

        return movingFruitsDic;
    }

    

    bool CheckBomb()
    {
        if(m_checkBomb) return false;
        int cnt = 0;
        for (int y = 0; y < m_getPos.y_TileGridSize; y++)
        {
            for (int x = 0; x < m_getPos.x_TileGridSize; x++)
            {
                if (m_getPos.m_fruits[x, y] != null)
                {
                    if (m_getPos.m_fruits[x, y].m_fruitData.fruitTypePoolKey == PoolKey.Blue_Bomb || m_getPos.m_fruits[x, y].m_fruitData.fruitTypePoolKey == PoolKey.Red_Bomb ||
                        m_getPos.m_fruits[x, y].m_fruitData.fruitTypePoolKey == PoolKey.Green_Bomb || m_getPos.m_fruits[x, y].m_fruitData.fruitTypePoolKey == PoolKey.Orange_Bomb ||
                        m_getPos.m_fruits[x, y].m_fruitData.fruitTypePoolKey == PoolKey.Yellow_Bomb)
                    {
                        cnt++;
                    }
                }
            }
        }
        m_checkBomb = true;
        if ( cnt == 0 ) return true;
        return false;
    }

    bool PickBomb()
    {
        int rnd = Random.Range(0, 100);
        if (rnd < 30)
        {
            return true;
        }
        return false;
    }


    Vector2Int GetOffset(PatternState state)
    {
        return m_patternShapeOffset[(int)state];
    }

    void CretateInitialFruits()
    {
        int check = 0;
        for (int i = 0; i < 4; i++)
        {
            bool isCreate = false;
            while (!isCreate && check < 50)
            {
                check++;

                int x = Random.Range(0, m_getPos.x_TileGridSize);
                int y = Random.Range(0, m_getPos.y_TileGridSize);

                isCreate = RandomCreate(x, y);
            }
        }

        for (int y = 0; y < 9; y++)
        {
            for (int x = 0; x < 6; x++)
            {
                if (m_getPos.m_fruits[x, y] == null)
                {
                    int rnd = Random.Range(0, FruitsPoolKey.Length);
                    PoolKey fruitType = FruitsPoolKey[rnd];
                    CreateFruitAt(new Vector2Int(x, y), fruitType);
                }
            }
        }
    }

    bool RandomCreate(int a, int b)
    {
        int rnd = Random.Range(1, System.Enum.GetValues(typeof(PatternShape)).Length);
        if (System.Enum.IsDefined(typeof(PatternShape), rnd))
            return PatternCheck(a, b, (PatternShape)rnd);
        return false;
    }

    public bool PatternCheck(int a, int b, PatternShape shape)
    {
        PatternState state = PatternState.None;
        switch (shape)
        {
            case PatternShape.Vertical:
                if ((a >= 0 && a <= 5) && (b >= 2 && b <= 5))
                {
                    state = RandomPatternState(PatternState.Left, PatternState.Right);
                }
                else if (b >= 0 && b <= 1) state = PatternState.Down;
                else if (b >= 6 && b <= 7) state = PatternState.Up;
                if (state == PatternState.None) return false;
                return CheckMatchablePlace(a, b, PatternShape.Vertical, state);


            case PatternShape.Horizontal:
                if (a == 2)
                {
                    state = RandomPatternState(PatternState.Left, PatternState.Right);
                }
                else if (a >= 0 && a <= 1) state = PatternState.Right;
                else if (a >= 3 && a <= 4) state = PatternState.Left;
                if (state == PatternState.None) return false;
                return CheckMatchablePlace(a, b, PatternShape.Horizontal, state);



            case PatternShape.WidthLShape:
                if ((a >= 1 && a <= 3) && (b >= 1 && b <= 7))
                {
                    int rnd = Random.Range(0, 4);
                    if (rnd == 0) state = PatternState.LShapeWidth_LeftUp;
                    else if (rnd == 1) state = PatternState.LShapeWidth_LeftDown;
                    else if (rnd == 2) state = PatternState.LShapeWidth_RightUp;
                    else state = PatternState.LShapeWidth_RightUp;
                }
                else if (b == 0)
                {
                    if (a == 0) state = PatternState.LShapeWidth_RightDown;
                    else if (a >= 1 && a <= 3)
                    {
                        state = RandomPatternState(PatternState.LShapeWidth_LeftDown, PatternState.LShapeWidth_RightDown);
                    }
                }
                else if ((a == 0 || a == 4) && (b >= 1 && b <= 7))
                {
                    if (a == 0)
                    {
                        state = RandomPatternState(PatternState.LShapeWidth_RightUp, PatternState.LShapeWidth_RightDown);
                    }
                    else
                    {
                        state = RandomPatternState(PatternState.LShapeWidth_LeftUp, PatternState.LShapeWidth_LeftDown);
                    }
                }
                else if ((a == 0 || a == 4) && b == 8)
                {
                    state = RandomPatternState(PatternState.LShapeWidth_RightUp, PatternState.LShapeWidth_LeftUp);
                }
                if (state == PatternState.None) return false;
                return CheckMatchablePlace(a, b, PatternShape.WidthLShape, state);

            case PatternShape.HeightLShape:
                if ((a >= 1 && a <= 4) && (b >= 1 && b <= 6))
                {
                    int rnd = Random.Range(0, 4);
                    if (rnd == 0) state = PatternState.LShapeHeight_LeftUp;
                    else if (rnd == 1) state = PatternState.LShapeHeight_LeftDown;
                    else if (rnd == 2) state = PatternState.LShapeHeight_RightUp;
                    else state = PatternState.LShapeHeight_RightUp;
                }
                else if (b == 0)
                {
                    if (a == 0) state = PatternState.LShapeHeight_RightDown;
                    else if (a >= 1 && a <= 4)
                    {
                        state = RandomPatternState(PatternState.LShapeHeight_LeftDown, PatternState.LShapeHeight_RightDown);
                    }
                    else if (a == 5) state = PatternState.LShapeHeight_LeftDown;
                }
                else if ((a == 0 || a == 5) && (b >= 1 && b <= 6))
                {
                    if (a == 0)
                    {
                        state = RandomPatternState(PatternState.LShapeHeight_RightUp, PatternState.LShapeHeight_RightDown);
                    }
                    else
                    {
                        state = RandomPatternState(PatternState.LShapeHeight_LeftUp, PatternState.LShapeHeight_LeftDown);
                    }
                }
                else if (b == 7)
                {
                    if (a == 0) state = PatternState.LShapeHeight_RightUp;
                    else if (a >= 1 && a <= 4)
                    {
                        state = RandomPatternState(PatternState.LShapeHeight_LeftUp, PatternState.LShapeHeight_RightUp);
                    }
                    else if (a == 5) state = PatternState.LShapeHeight_LeftUp;
                }
                if (state == PatternState.None) return false;
                return CheckMatchablePlace(a, b, PatternShape.HeightLShape, state);
        }
        return false;
    }

    PatternState RandomPatternState(PatternState trueState, PatternState falseState)
    {
        PatternState state = PatternState.None;
        int rnd = Random.Range(0, 2);
        state = rnd == 0 ? trueState : falseState;
        return state;
    }

    void CreateFruitAt(Vector2Int gridPos, PoolKey fruitType)
    {
        Vector3 pos = m_getPos.GetTilePosition(gridPos.x, gridPos.y);

        GameObject fruit = ManagerManager.Instance.objectPoolManager.Get(fruitType);
        m_getPos.m_fruits[gridPos.x, gridPos.y] = fruit.GetComponent<Fruit>();
        fruit.transform.position = pos;
        fruit.transform.SetParent(m_FruitPos);

        m_getPos.m_checkFruit[gridPos.x, gridPos.y] = false;
        ISetFruitGridPos ifi = fruit.GetComponent<ISetFruitGridPos>();
        if (ifi != null)
        {
            ifi.SetGridPosition(new Vector2Int(gridPos.x, gridPos.y));
        }
    }

    bool CheckMatchablePlace(int a, int b, PatternShape shape, PatternState state)
    {
        Vector2Int origin = new Vector2Int(a, b);
        Vector2Int[] coords;
        if (shape == PatternShape.Vertical) coords = new Vector2Int[] { origin, origin + Vector2Int.up, origin + GetOffset(state) };
        else if (shape == PatternShape.Horizontal) coords = new Vector2Int[] { origin, origin + Vector2Int.right, origin + GetOffset(state) };
        else if (shape == PatternShape.WidthLShape) coords = new Vector2Int[] { origin, origin + Vector2Int.right, origin + GetOffset(state) };
        else coords = new Vector2Int[] { origin, origin + Vector2Int.up, origin + GetOffset(state) };

        // 해당 위치에 과일이 존재할 때 false 리턴
        foreach (Vector2Int c in coords)
        {
            if (c.x < 0 || c.y < 0 || c.x > 5 || c.y > 8) return false;
            if (m_getPos.m_fruits[c.x, c.y] != null)
            {
                return false;
            }
        }

        int rnd = Random.Range(0, FruitsPoolKey.Length);
        PoolKey fruitType = FruitsPoolKey[rnd];

        foreach (Vector2Int c in coords)
        {
            CreateFruitAt(c, fruitType);
        }
        return true;
    }
}
