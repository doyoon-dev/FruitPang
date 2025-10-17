using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReferenceManager : MonoBehaviour
{
    public static ReferenceManager Inst;

    public Transform Board;
    public Transform FruitTrans;
    public GameObject[] DestroyEffectPrefabs;
    public Canvas Canvas;
    public Point Point;
    public Pause Pause;
    public Timer Timer;
    public GameOver GameOver;

    // Start is called before the first m_frame update
    void Awake()
    {
        if (Inst == null)
        {
            Inst = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnGameSceneLoaded()
    {
        Board = FindObjectOfType<Board>().transform;
        Point = FindObjectOfType<Point>();
        Pause = FindObjectOfType<Pause>();
        Canvas = FindObjectOfType<Canvas>();
        Timer = FindObjectOfType<Timer>();
        GameOver = Canvas.GetComponentInChildren<GameOver>(true);
    }
}
