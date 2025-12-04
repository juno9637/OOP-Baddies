using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    private UIManagerScript UIManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playGameTwo();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void playGameTwo() {
        if (UIManager == null)
        {
            UIManager = GetComponent<UIManagerScript>();
            if (UIManager == null)
            {
                UIManager = gameObject.AddComponent<UIManagerScript>();
            }
        }
        UIManager.setScene(2);
    }
}
