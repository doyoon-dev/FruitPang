using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public interface IOnGameSceneLoaded
{
    void OnGameSceneLoaded();
}

public class ManagerManager : MonoBehaviour, IOnGameSceneLoaded
{
    public static ManagerManager Instance = null;

    public ReferenceManager refManager;
    public GameManager gameManager;
    public ObjectPoolManager objectPoolManager;
    public ScoreManager scoreManager;
    public SoundManager soundManager;
    public UIManager uiManager;
    public ButtonManager buttonManager;

    // Start is called before the first frame update
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

    public void OnGameSceneLoaded()
    {
        refManager.OnGameSceneLoaded();
        scoreManager.OnGameSceneLoaded();
        gameManager.OnGameSceneLoaded();
        soundManager.OnGameSceneLoaded();
        buttonManager.OnGameSceneLoaded();
    }
}
