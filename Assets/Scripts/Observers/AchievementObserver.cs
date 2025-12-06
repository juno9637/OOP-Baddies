using UnityEngine;


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
        //AchievementPopup popup = AchievementPopup.GetInstance();
        //popup.ShowPopup(text);
    }

}
