using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;

public class CreateMatchSystem : MonoBehaviour
{
    public GameObject m_tile;
    GameObject m_checkSameFruit;
    public Vector2Int m_firstWidthVector = Vector2Int.zero;

    Vector2Int[] m_verticalShapeOffset = new Vector2Int[]
    {
        new Vector2Int(0, 3),       // Down
        new Vector2Int(0, -2)       // Up
    };

    Vector2Int[] m_horizontalShapeOffset = new Vector2Int[]
    {
        new Vector2Int(3, 0),       // Right
        new Vector2Int(-2, 0)       // Left
    };

    Vector2Int[] m_LshapeWidthOffset = new Vector2Int[]
    {
        new Vector2Int(-1, 1),      // LeftDown
        new Vector2Int(-1, -1),     // LeftUp
        new Vector2Int(2, 1),       // RightDown
        new Vector2Int(2, -1)       // RightUp
    };

    Vector2Int[] m_LshapeHeightOffset = new Vector2Int[]
    {
        new Vector2Int(-1, 2),      // LeftDown
        new Vector2Int(-1, -1),     // LeftUp
        new Vector2Int(1, 2),       // RightDown
        new Vector2Int(1, -1)       // RightUp
    };



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

    // Start is called before the first frame update
    void Start()
    {
        CreateTile();
        Create();
        //RandomPick();
        //PickPattern(1, 1);
        //PickPattern(3, 1);
        //PickPattern(1, 6);
        //PickPattern(4, 6);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    Vector2Int GetOffset(PatternState state)
    {
        return m_patternShapeOffset[(int)state];
    }


    public void CreateTile()
    {
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                GameObject obj = ManagerManager.Instance.objectPoolManager.Get(PoolKey.Tile);
                //obj.transform.position = GetTilePosition(j, i);
            }
        }
    }

    bool[,] occupied;

    void Create()
    {
        occupied = new bool[6, 9];
        int check = 0;
        for (int i = 0; i < 4; i++)
        {
            bool isCreate = false;
            while (!isCreate && check < 100)
            {
                check++;

                int x = Random.Range(0, 6);
                int y = Random.Range(0, 9);

                isCreate = RandomCreate(x, y);
            }
        }

        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                if (!occupied[j, i])
                {
                    int rnd = Random.Range(0, FruitsPoolKey.Length);
                    PoolKey fruitType = FruitsPoolKey[rnd];
                    CreateFruitAt(new Vector2Int(j, i), fruitType);
                    occupied[j, i] = true;
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
                if ((a >= 0 && a <= 5) && (b >= 2 && b <=5))
                {
                    state = RandomPatternState(PatternState.Left, PatternState.Right);
                    //int rnd = Random.Range(0, 2);
                    //state = rnd == 0 ? PatternState.Up : PatternState.Down;
                }
                else if (b >= 0 && b <= 1) state = PatternState.Down;
                else if (b >= 6 && b <= 7) state = PatternState.Up;
                //VerticalShapePattern(a, b, state);
                //break;
                if (state == PatternState.None) return false;
                return CheckMatchablePlace(a, b, PatternShape.Vertical, state);


            case PatternShape.Horizontal:
                if (a == 2)
                {
                    state = RandomPatternState(PatternState.Left, PatternState.Right);
                    //int rnd = Random.Range(0, 2);
                    //state = rnd == 0 ? PatternState.Left : PatternState.Right;
                }
                else if (a >= 0 && a <= 1) state = PatternState.Right;
                else if (a >= 3 && a <= 4) state = PatternState.Left;
                //HorizontalShapePattern(a, b, state);
                //break;
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
                        //int rnd = Random.Range(0, 2);
                        //state = rnd == 0 ? PatternState.LShapeWidth_LeftDown : PatternState.LShapeWidth_RightDown;
                    }
                }
                else if ((a == 0 || a == 4) && (b >= 1 && b <= 7))
                {
                    if (a == 0)
                    {
                        state = RandomPatternState(PatternState.LShapeWidth_RightUp, PatternState.LShapeWidth_RightDown);
                        //int rnd = Random.Range(0, 2);
                        //state = rnd == 0 ? PatternState.LShapeWidth_RightUp : PatternState.LShapeWidth_RightDown;
                    }
                    else
                    {
                        state = RandomPatternState(PatternState.LShapeWidth_LeftUp, PatternState.LShapeWidth_LeftDown);
                        //int rnd = Random.Range(0, 2);
                        //state = rnd == 0 ? PatternState.LShapeWidth_LeftUp : PatternState.LShapeWidth_LeftDown;
                    }
                }
                else if ((a == 0 || a == 4) && b == 8)
                {
                    state = RandomPatternState(PatternState.LShapeWidth_RightUp, PatternState.LShapeWidth_LeftUp);
                    //state = a == 0 ? PatternState.LShapeWidth_RightUp : PatternState.LShapeWidth_LeftUp;
                    //if (a == 0) state = PatternState.LShapeWidth_RightUp;
                    //else if(a == 4) state = PatternState.LShapeWidth_LeftUp;
                }
                //LShapeWidthPattern(a, b, state);
                //break;
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
                        //int rnd = Random.Range(0, 2);
                        //state = rnd == 0 ? PatternState.LShapeHeight_LeftDown : PatternState.LShapeHeight_RightDown;
                    }
                    else if (a == 5) state = PatternState.LShapeHeight_LeftDown;
                }
                else if ((a == 0 || a == 5) && (b >= 1 && b <= 6))
                {
                    if (a == 0)
                    {
                        state = RandomPatternState(PatternState.LShapeHeight_RightUp, PatternState.LShapeHeight_RightDown);
                        //int rnd = Random.Range(0, 2);
                        //state = rnd == 0 ? PatternState.LShapeHeight_RightUp : PatternState.LShapeHeight_RightDown;
                    }
                    else
                    {
                        state = RandomPatternState(PatternState.LShapeHeight_LeftUp, PatternState.LShapeHeight_LeftDown);
                        //int rnd = Random.Range(0, 2);
                        //state = rnd == 0 ? PatternState.LShapeHeight_LeftUp : PatternState.LShapeHeight_LeftDown;
                    }
                }
                else if (b == 7)
                {
                    if (a == 0) state = PatternState.LShapeHeight_RightUp;
                    else if (a >= 1 && a <= 4)
                    {
                        state = RandomPatternState(PatternState.LShapeHeight_LeftUp, PatternState.LShapeHeight_RightUp);
                        //int rnd = Random.Range(0, 2);
                        //if (rnd == 0) state = PatternState.LShapeHeight_LeftUp;
                        //else state = PatternState.LShapeHeight_RightUp;
                    }
                    else if (a == 5) state = PatternState.LShapeHeight_LeftUp;
                }
                //LShapeHeightPattern(a, b, state);
                //break;
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

    #region VerticalShapePattern 가이드라인
    // 첫 생성되는 과일의 x좌표가 0~5 일 때만 생성 2 <= y <= 5
    // 첫 생성되는 과일의 y좌표가 0, 1 일 때 아래쪽만 생성
    // 첫 생성되는 과일의 y좌표가 6, 7 일 때 윗쪽만 생성
    // 첫 생성되는 과일의 y좌표는 8이 되면 안 됨
    #endregion
    void VerticalShapePattern(int a, int b, PatternState state)
    {
        
        int rnd = Random.Range(0, FruitsPoolKey.Length);
        PoolKey fruitType = FruitsPoolKey[rnd];
        Vector2Int origin = new Vector2Int(a, b);

        CreateFruitAt(origin, fruitType);

        CreateFruitAt(origin + Vector2Int.up, fruitType);

        Vector2Int offset = GetOffset(state);
        CreateFruitAt(origin + offset, fruitType);
    }

    #region HorizontalShapePattern 가이드라인
    // 첫 생성되는 과일의 x좌표가 2 일 때만 생성 0 <= y <= 8
    // 첫 생성되는 과일이 x좌표는 0, 1일 때 오른쪽만 생성
    // 첫 생성되는 과일이 x좌표는 3, 4일 때 왼쪽만 생성
    // 첫 생성되는 과일의 x좌표는 5가 되면 안 됨 (첫 생성 과일 오른쪽에 위치한곳에 같은 과일을 생성하기 때문)
    #endregion
    void HorizontalShapePattern(int a, int b, PatternState state)
    {
        int rnd = Random.Range(0, FruitsPoolKey.Length);
        PoolKey fruitType = FruitsPoolKey[rnd];
        Vector2Int origin = new Vector2Int(a, b);

        CreateFruitAt(origin, fruitType);

        CreateFruitAt(origin + Vector2Int.right, fruitType);

        Vector2Int offset = GetOffset(state);
        CreateFruitAt(origin + offset, fruitType);
    }

    #region LShapeWidthPattern 가이드라인
    // 첫 생성되는 과일의 x좌표가 1, 2, 3 일 때만 생성 1 <= y <= 7 (왼쪽 위/아래, 오른쪽 위/아래 다 가능)

    // 첫 생성 과일이 y = 0 에 생성 될 때
    // 첫 생성되는 과일의 좌표가 (0, 0) 일 때 생성 (2, 1) 만 생성 가능 (오른쪽 대각선 아래)
    // 첫 생성되는 과일의 x좌표가 1 ~ 3 일 때 x = 1 일 때 (0, 1) or (3, 1) / x = 2 일 때 (1, 1) or (4, 1) / x = 3 일 때 (2, 1) or (5, 1) 만 생성 가능 : 맨 윗 라인 (왼쪽 아래, 오른쪽 아래만 생성)
    // 첫 생성되는 과일의 x좌표가 4 일 때 (3, 1) 만 생성 가능

    // 첫 생성되는 과일의 x = 0 이고 y좌표가 1 ~ 7 일 때 오른쪽 위/아래 만 생성 가능
    // 첫 생성되는 과일의 x = 4 이고 y좌표가 1 ~ 7 일 때 왼쪽 위/아래 만 생성 가능

    // 첫 생성 과일이 y = 8 에 생성 될 때
    // 첫 생성되는 과일의 좌표가 (0, 8) 일 때 생성 (2, 7) 만 생성 가능 (오른쪽 위)
    // 첫 생성되는 과일의 x좌표가 4 일 때 (3, 7) 만 생성 가능 (왼쪽 위)

    // 첫 생성되는 과일의 x좌표는 5가 되면 안됨
    #endregion
    void LShapeWidthPattern(int a, int b, PatternState state)
    {
        int rnd = Random.Range(0, FruitsPoolKey.Length);
        PoolKey fruitType = FruitsPoolKey[rnd];
        Vector2Int origin = new Vector2Int(a, b);

        CreateFruitAt(origin, fruitType);

        CreateFruitAt(origin + Vector2Int.right, fruitType);

        Vector2Int offset = GetOffset(state);
        CreateFruitAt(origin + offset, fruitType);
    }

    #region LShapeHeightPattern 가이드라인
    // 첫 생성되는 과일의 x좌표가 1, 2, 3, 4 일 때만 생성 1 <= y <= 6 (왼쪽 위/아래, 오른쪽 위/아래 다 가능)

    // 첫 생성 과일 y = 0 일 때
    // 첫 생성되는 과일의 좌표가 (0, 0) 일 때 생성 (1, 2) 만 생성 가능 (오른쪽 아래)
    // 첫 생성되는 과일의 x좌표가 1 ~ 4 일 때 x = 1 일 때 (0, 2) or (2, 2) / x = 2 일 때 (1, 2) or (3, 2) / x = 3 일 때 (2, 2) or (4, 2) / x = 4 일 때 (3, 2) or (5, 2) 만 생성 가능 : 맨 윗 라인 (왼쪽 아래, 오른쪽 아래만 가능)
    // 첫 생성되는 과일의 x좌표가 5 일 때 (4, 2) 만 생성 가능 (왼쪽 아래)

    // 첫 생성되는 과일의 x = 0 이고 y좌표가 1 ~ 6 일 때 오른쪽 위/아래 만 생성 가능
    // 첫 생성되는 과일의 x = 5 이고 y좌표가 1 ~ 6 일 때 왼쪽 위/아래 만 생성 가능

    // 첫 생성되는 과일 y = 7 일 때
    // 첫 생성되는 과일의 좌표가 (0, 7) 일 때 생성 (1, 6) 만 생성 가능 (오른쪽 위)
    // 첫 생성되는 과일의 x좌표가 1 ~ 4 / y = 7 일 때 x = 1 일 때 (0, 6) or (2, 6) / x = 2 일 때 (1, 6) or (3, 6) / x = 3 일 때 (2, 6) or (4, 6) / x = 4 일 때 (3, 6) or (5, 6) 만 생성 가능 : 맨 아랫 라인 (왼쪽 위, 오른쪽 위만 가능)
    // 첫 생성되는 과일의 x좌표가 5 일 때 (4, 6) 만 생성 가능 (왼쪽 위)

    // 첫 생성되는 과일의 x좌표는 8이 되면 안됨
    #endregion
    void LShapeHeightPattern(int a, int b, PatternState state)
    {
        int rnd = Random.Range(0, FruitsPoolKey.Length);
        PoolKey fruitType = FruitsPoolKey[rnd];
        Vector2Int origin = new Vector2Int(a, b);

        CreateFruitAt(origin, fruitType);

        CreateFruitAt(origin + Vector2Int.up, fruitType);

        Vector2Int offset = GetOffset(state);
        CreateFruitAt(origin + offset, fruitType);
    }

    void CreateFruitAt(Vector2Int gridPos, PoolKey fruitType)
    {
        //Vector3 pos = GetTilePosition(gridPos.x, gridPos.y);

        //GameObject fruit = ManagerManager.Instance.objectPoolManager.Get(fruitType);
        //fruit.transform.position = pos;
    }

    bool CheckMatchablePlace(int a, int b, PatternShape shape, PatternState state)
    {
        Vector2Int origin = new Vector2Int(a, b);
        Vector2Int[] coords;
        if (shape == PatternShape.Vertical) coords = new Vector2Int[] { origin, origin + Vector2Int.up, origin + GetOffset(state) };
        else if(shape == PatternShape.Horizontal) coords = new Vector2Int[] { origin, origin + Vector2Int.right, origin + GetOffset(state) };
        else if (shape == PatternShape.WidthLShape) coords = new Vector2Int[] { origin, origin + Vector2Int.right, origin + GetOffset(state) };
        else coords = new Vector2Int[] { origin, origin + Vector2Int.up, origin + GetOffset(state) };

        foreach (Vector2Int c in coords)
        {
            if (c.x < 0 || c.y < 0 || c.x > 5 || c.y > 8) return false;
            //if (m_fruits[a, b] != null) return false;
            if (occupied[c.x, c.y])
            {
                return false;
            }
        }

        int rnd = Random.Range(0, FruitsPoolKey.Length);
        PoolKey fruitType = FruitsPoolKey[rnd];

        foreach (Vector2Int c in coords)
        {
            CreateFruitAt(c, fruitType);
            //m_fruits[a, b] = Fruit;
            occupied[c.x, c.y] = true;
        }
        return true;
    }



    #region 연습
    public void CreateWidthShapeTile(int a, int b)
    {
        m_firstWidthVector = new Vector2Int(a, b);
        for (int y = b; y < b + 2; y++)
        {
            for (int x = a; x < a+3; x++)
            {
                //GameObject obj = Instantiate(m_tile);
                //obj.transform.position = GetTilePosition(x, y);
                CheckSameFruit(x, y);
            }
        }
        m_checkSameFruit = null;
        m_firstWidthVector = Vector2Int.zero;
    }

    public void CreateHeightShapeTile(int a, int b)
    {
        m_firstWidthVector = new Vector2Int(a, b);
        for (int y = b; y < b + 3; y++)
        {
            for (int x = a; x < a + 2; x++)
            {
                //GameObject obj = Instantiate(m_tile);
                //obj.transform.position = GetTilePosition(x, y);
                CheckSameFruit(x, y);
            }
        }
        m_checkSameFruit = null;
        m_firstWidthVector = Vector2Int.zero;
    }

    public void CheckSameFruit(int x, int y)
    {
        int rnd = Random.Range(0, FruitsPoolKey.Length);
        Vector2Int v = new Vector2Int(x, y);
        if (m_checkSameFruit == null)
        {
            GameObject obj = ManagerManager.Instance.objectPoolManager.Get(FruitsPoolKey[rnd]);
            //obj.transform.position = GetTilePosition(x, y);
            m_checkSameFruit = obj;
        }
        else
        {
            if (CreatableFruitPos(m_firstWidthVector, v))
            {
                Fruit f = m_checkSameFruit.GetComponent<Fruit>();
                GameObject obj = ManagerManager.Instance.objectPoolManager.Get(f.m_fruitData.fruitTypePoolKey);
                //obj.transform.position = GetTilePosition(x, y);
            }
            else
            {
                GameObject obj = ManagerManager.Instance.objectPoolManager.Get(FruitsPoolKey[rnd]);
                //obj.transform.position = GetTilePosition(x, y);
            }
        }
    }

    public bool CreatableFruitPos(Vector2Int org, Vector2Int target)
    {
        Vector2Int[] offsets = new Vector2Int[]
        {
            new Vector2Int(-1, -1),
            new Vector2Int(-1, 1),
            new Vector2Int(2, 1),
            new Vector2Int(2, -1),
            new Vector2Int(-1, 2),
            new Vector2Int(1, 2),
            new Vector2Int(1, -1),

            new Vector2Int(1, 0)
        };

        // 가로 무조건 만들어져야하는 첫 생성 과일의 바로 옆 위치
        Vector2Int h = new Vector2Int(org.x + 1, org.y);

        // 세로 무조건 만들어져야하는 첫 생성 과일의 바로 아래 위치
        Vector2Int i = new Vector2Int(org.x, org.y + 1);

        return offsets.Any(offset => org + offset == target);

        // return offsets.Any(offset => org + offset == target) 아래 foreach문과 같은 의미

        //foreach (var offset in offsets)
        //{
        //    if(org+offset == target)
        //        return true;
        //}
        //return false;
    }
    #endregion
}
