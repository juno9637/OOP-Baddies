using System;
using UnityEngine;
using UnityEngine.UIElements;

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
    [SerializeField] private GameObject _uiDoc;
    
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
                WipeSceneButKeep(_uiDoc);
            }

        NextLevel();
        }
    }
    
    void WipeSceneButKeep(GameObject keep)
    {
        foreach (GameObject go in GameObject.FindObjectsOfType<GameObject>())
        {
            if (go == this.gameObject) return;
            
            if(go != keep)
                Destroy(go);
        }
    }
}
