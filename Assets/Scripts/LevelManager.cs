using System;
using UnityEngine;

public class ClickCounter : ObservableBase
{
    public const string MSG_COUNT = "COUNT";
    
    private int count;
    
    public int Count => count;
    
    public void Increment()
    {
        Debug.Log("Increment");
        count++;
        Notify($"{MSG_COUNT}:{count}");
    }
    
    public void Reset() => count = 0;
}

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
            NextLevel();
        }
    }
    
}
