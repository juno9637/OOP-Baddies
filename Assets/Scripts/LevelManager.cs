using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CloseButtonTracker : ObservableBase
{
    public const string MSG_CLOSED = "CLOSED";
    
    public void OnClosePressed()
    {
        Debug.Log("OnClosePressed");
        Notify(MSG_CLOSED);
    }
}

public class LevelManager : MonoBehaviour, IObserver
{
    [SerializeField] private UI_Controller uiController;
    [SerializeField] private List<GameObject> permanentObjects;
    
    private int currentLevel = 0;
    private int maxLevel = 4;
    
    private Action onLevelTransition;
    
    public int CurrentLevel => currentLevel;

    void Start()
    {
        StartGame();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            switch (currentLevel)
            {
                case 1:
                    uiController.CreateUI(UI_Type.Menu);
                    break;
                default:
                    uiController.CreateUI(UI_Type.ErrorMenu);
                    break;
            }
            
        }
    }
    
    public void StartGame()
    {
        currentLevel = 1;
        uiController.SetupLevel(currentLevel);
    }
    
    public void NextLevel()
    {
        currentLevel++;
        
        uiController.ClearUI();
        
        if (currentLevel > maxLevel)
        {
            //CloseGame
        }
        else
        {
            uiController.SetupLevel(CurrentLevel);
        }
    }
    
    public void OnNotify(string msg)
    {
        if (msg == CloseButtonTracker.MSG_CLOSED)
        {
            if (currentLevel == 1)
            {
                ClearSceneExcept(permanentObjects);
            }

        NextLevel();
        }
    }
    
    void ClearSceneExcept(List<GameObject> permanentObjects)
    {
        foreach (GameObject remove in GameObject.FindObjectsOfType<GameObject>())
        {
            if (remove == this.gameObject)
                continue;
            
            if (!permanentObjects.Contains(remove))
                Destroy(remove);
        }
    }
}
