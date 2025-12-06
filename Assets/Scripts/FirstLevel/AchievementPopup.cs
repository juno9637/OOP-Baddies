using UnityEngine;
using UnityEngine.UI;

public class AchievementPopup : MonoBehaviour
{
    private static AchievementPopup instance;

    private GameObject popupObject;
    private Text messageText;

    private float displayTime = 3f;
    private float timer = 0f;

    void Awake()
    {
        // Singleton pattern - Unity doesn't allow private Awake()
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        CreateUI();
    }

    public static AchievementPopup GetInstance() {
        if (instance == null) {
            GameObject obj = new GameObject("AchievementPopupManager");
            instance = obj.AddComponent<AchievementPopup>();
        }

        return instance;
    }

    void Update()
    {
        if (popupObject != null && popupObject.activeSelf)
        {
            timer += Time.deltaTime;
            if (timer >= displayTime)
            {
                popupObject.SetActive(false);
            }
        }
    }

    private void CreateUI()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("AchievementCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }

        // === Create popup panel ===
        popupObject = new GameObject("AchievementPopup");
        popupObject.transform.SetParent(canvas.transform);

        RectTransform rect = popupObject.AddComponent<RectTransform>();
        Image bg = popupObject.AddComponent<Image>();

        bg.color = new Color(0, 0, 0, 0.75f); // translucent black background

        // Set size & position (top center)
        rect.sizeDelta = new Vector2(600, 100);
        rect.anchorMin = new Vector2(0.5f, 1f);
        rect.anchorMax = new Vector2(0.5f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = new Vector2(0, -50);

        // === Create Text child ===
        GameObject textObj = new GameObject("PopupText");
        textObj.transform.SetParent(popupObject.transform);

        messageText = textObj.AddComponent<Text>();
        messageText.fontSize = 32;
        messageText.color = Color.white;
        messageText.alignment = TextAnchor.MiddleCenter;
        
        popupObject.SetActive(false);
    }

    public void ShowPopup(string message)
    {
        if (messageText == null)
        {
            Debug.LogError("AchievementPopup: Error messageText not found");
            return;
        }

        messageText.text = message;
        popupObject.SetActive(true);
        timer = 0f;
    }
}
