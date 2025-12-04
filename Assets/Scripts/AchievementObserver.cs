using UnityEngine;
using UnityEngine.UI;

public class AchievementObserver : MonoBehaviour, IObserver 
{
    private float popupDuration = 3f;

    public void OnNotify(string message)
    {
        Debug.Log($"Achievement Observer notified: {message}");
        ShowPopup($"Achievement: {message}");
    }

    private void ShowPopup(string text)
    {
        AchievementPopup popup = AchievementPopup.getInstance();
        popup.ShowPopup(text);
    }

}
