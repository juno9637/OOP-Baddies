using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    private UIManagerScript UIManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UIManager = gameObject.AddComponent<UIManagerScript>();        
        playGameTwo();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void playGameTwo() {
        GameObject gameManagerObj = new GameObject("FightSceneManager");
        FightSceneManager fightManager = gameManagerObj.AddComponent<FightSceneManager>();
        AchievementObserver aObserver = new AchievementObserver();
        fightManager.Attach(aObserver);
        UIManager.UndertaleSceneSetup(fightManager);
        UIManager.cleanup();
    }
}
