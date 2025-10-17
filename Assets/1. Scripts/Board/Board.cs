using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using static Unity.VisualScripting.Dependencies.Sqlite.SQLite3;
using static UnityEditor.PlayerSettings;

public interface IDestroyFruitsByBomb
{
    void DestroyFruitsByBomb(Transform target, ColorType colorType);
}

public interface ISwapFruit
{
    void SwapFruit(Vector2Int gridPos, Vector2Int dir);
}

public interface IInit
{
    void Init();
}

public class Board : MonoBehaviour, ISwapFruit, IDestroyFruitsByBomb, IInit
{
    [Header("컴포넌트 레퍼런스")]
    [SerializeField] FruitMovement m_fruitMovement;
    [SerializeField] SwappingFruits m_swappingFruits;
    [SerializeField] DestroytFruit m_destroyFruit;
    [SerializeField] CheckFruits m_checkFruits;
    [SerializeField] CreateFruit m_createFruit;
    [SerializeField] GetPosition m_getPosition;

    void Awake()
    {
        m_getPosition = new GetPosition(6, 9);
        Init();
    }

    // Start is called before the first m_frame update
    void Start()
    {
        m_createFruit.CreateInit();
    }

    // Update is called once per m_frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("[0, 0] : " + m_getPosition.m_fruits[0, 0]);
            Debug.Log("[1, 0] : " + m_getPosition.m_fruits[1, 0]);
        }
    }

    public void Init()
    {
        m_fruitMovement.Init(m_getPosition);
        m_fruitMovement.SetCreateFruit(m_createFruit);
        m_fruitMovement.SetDestroyFruit(m_destroyFruit);
        m_fruitMovement.SetSwappingFruits(m_swappingFruits);
        m_swappingFruits.Init(m_getPosition);
        m_destroyFruit.Init(m_getPosition);
        m_destroyFruit.SetCheckFruits(m_checkFruits);
        m_checkFruits.Init(m_getPosition);
        m_createFruit.Init(m_getPosition);
    }

    // 과일 스왑 Fruit 스크립트에서 마우스 이벤트 발생 시 호출
    public void SwapFruit(Vector2Int pos, Vector2Int dir)
    {
        // targetPos : 클릭을 뗀 타일 좌표
        Vector2Int targetPos = pos + dir;

        Fruit clickFruit = m_getPosition.m_fruits[pos.x, pos.y];
        Fruit changeFruit = m_getPosition.m_fruits[targetPos.x, targetPos.y];

        m_fruitMovement.m_changeFruitsOrgGridPos.Clear();

        m_fruitMovement.m_changeFruitsOrgGridPos.Add(0, new Vector2Int(pos.x, pos.y));
        m_fruitMovement.m_changeFruitsOrgGridPos.Add(1, new Vector2Int(targetPos.x, targetPos.y));

        Vector3 moveClickDir = m_getPosition.GetTilePosition(targetPos.x, targetPos.y);
        Vector3 moveChangeDir = m_getPosition.GetTilePosition(pos.x, pos.y);

        m_fruitMovement.m_checkCnt = 0;
        m_swappingFruits.m_checkSwap = false;

        m_fruitMovement.MoveFruit(clickFruit, changeFruit, m_fruitMovement.MoveDone);
        m_fruitMovement.MoveFruit(changeFruit, clickFruit, m_fruitMovement.MoveDone);
    }
    
    

    // 전체 타일에서 클릭한 폭탄의 색과 같은 과일들을 저장한 리스트
    List<Fruit> CheckUseBomb(ColorType colorType)
    {
        // colorType : 클릭한 폭탄의 색 타입
        List<Fruit> m_bombList = new List<Fruit>();
        for (int y = 0; y < m_getPosition.y_TileGridSize; y++)
        {
            for (int x = 0; x < m_getPosition.x_TileGridSize; x++)
            {
                if (m_getPosition.m_fruits[x, y].m_fruitData.colorType == colorType)
                {
                    m_bombList.Add(m_getPosition.m_fruits[x, y]);
                }
            }
        }
        return m_bombList;
    }


    

    public void DestroyFruitsByBomb(Transform target, ColorType colorType)
    {
        List<Fruit> list = new List<Fruit>();
        list = CheckUseBomb(colorType);

        // 파괴 애니메이션 실행
        m_destroyFruit.DestroyFruits(list, m_fruitMovement.MovingNewFruits);
    }
}
