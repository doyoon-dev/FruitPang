using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using static Unity.VisualScripting.Dependencies.Sqlite.SQLite3;

public interface IGameOver
{
    void GameOver();
}

public class GameManager : MonoBehaviour, IGameOver
{
    public static GameManager Instance;

    public GameOver m_GameOverObj;
    public LayerMask m_fruitMask;
    public LayerMask m_bombMask;

    public Vector3 m_dragStartPos;

    public Board m_board;
    GameObject m_hitObj;

    bool m_isDrag = false;
    bool m_isBomb = false;

    // Start is called before the first m_frame update
    void Awake()
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
    }

    // Update is called once per m_frame
    void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, m_fruitMask | m_bombMask);
                if (hit.collider != null)
                {
                    m_hitObj = hit.collider.gameObject;
                    // ((1 << 6) & 64)
                    m_dragStartPos = Input.mousePosition;
                    m_isDrag = true;
                    if ((1 << hit.collider.gameObject.layer & m_bombMask) != 0)
                    {
                        m_isBomb = true;
                    }
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                Vector3 dragEndPos = Input.mousePosition;
                Vector2 dragPos = dragEndPos - m_dragStartPos;

                Vector2Int dir = Vector2Int.zero;

                if (m_isBomb)
                {
                    IDestroyFruitsByBomb idfbb = m_board.GetComponent<IDestroyFruitsByBomb>();
                    if (idfbb != null)
                    {
                        if (Mathf.Approximately(dragPos.magnitude, 0.0f))
                        {
                            Fruit bomb = m_hitObj.GetComponent<Fruit>();

                            idfbb.DestroyFruitsByBomb(bomb.transform, bomb.m_fruitData.colorType);

                            m_isDrag = false;
                            m_isBomb = false;
                        }
                    }
                }

                if (!m_isDrag) { return; }

                m_isDrag = false;

                if (dragPos.magnitude < 0.5f) { return; }

                // 이동 방향 체크
                ICheckMovableDirection icmd = m_hitObj.GetComponent<ICheckMovableDirection>();
                if (icmd != null)
                {
                    dir = icmd.CheckMovableDirection(dragPos);
                }

                ISwapFruit isf = m_board.GetComponent<ISwapFruit>();
                if (isf != null)
                {
                    IGetFruitGridPos igfgp = m_hitObj.GetComponent<IGetFruitGridPos>();
                    if (igfgp != null)
                    {
                        isf.SwapFruit(igfgp.m_gridPos, dir);
                    }

                }
            }
#else
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, m_fruitMask | m_bombMask);
                    if (hit.collider != null)
                    {
                        m_hitObj = hit.collider.gameObject;
                        // ((1 << 6) & 64)
                        m_dragStartPos = touch.position;
                        m_isDrag = true;
                        if ((1 << hit.collider.gameObject.layer & m_bombMask) != 0)
                        {
                            m_isBomb = true;
                        }
                    }
                }
                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    Vector3 dragEndPos = touch.position;
                    Vector2 dragPos = dragEndPos - m_dragStartPos;

                    Vector2Int dir = Vector2Int.zero;

                    if (m_isBomb)
                    {
                        IDestroyFruitsByBomb idfbb = m_board.GetComponent<IDestroyFruitsByBomb>();
                        if (idfbb != null)
                        {
                            if (Mathf.Approximately(dragPos.magnitude, 0.0f))
                            {
                                Fruit bomb = m_hitObj.GetComponent<Fruit>();

                                idfbb.DestroyFruitsByBomb(bomb.transform, bomb.m_fruitData.colorType);

                                m_isDrag = false;
                                m_isBomb = false;
                            }
                        }
                    }

                    if (!m_isDrag) { return; }

                    m_isDrag = false;


                    ICheckMovableDirection icmd = m_hitObj.GetComponent<ICheckMovableDirection>();
                    if (icmd != null)
                    {
                        dir = icmd.CheckMovableDirection(dragPos);
                    }

                    ISwapFruit isf = m_board.GetComponent<ISwapFruit>();
                    if (isf != null)
                    {
                        IGetFruitGridPos igfgp = m_hitObj.GetComponent<IGetFruitGridPos>();
                        if (igfgp != null)
                        {
                            isf.SwapFruit(igfgp.m_gridPos, dir);
                        }

                    }
                }
            }
#endif
        }
    }

    public void GameOver()
    {
        m_GameOverObj.gameObject.SetActive(true);
    }

    public void RestartScene()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("LoadingScene");
    }

    public void OnGameSceneLoaded()
    {
        m_board = FindObjectOfType<Board>();
        m_GameOverObj = ManagerManager.Instance.refManager.Canvas.GetComponentInChildren<GameOver>(true);
    }
}
