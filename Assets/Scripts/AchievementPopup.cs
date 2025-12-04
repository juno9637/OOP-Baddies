    using UnityEngine;
    using UnityEngine.UI;

    public class AchievementPopup : MonoBehaviour
    {
        private GameObject gameObject;
        private Text messageText;
        public static AchievementPopup popup;

        private AchievementPopup()
        {
            this.gameObject = new GameObject();
            this.messageText = gameObject.AddComponent<Text>();
            gameObject.SetActive(false); 
        }

        public static AchievementPopup getInstance() {
            if (popup == null) {
                popup = new AchievementPopup();
            }
            return popup;
        }

        public void ShowPopup(string message)
        {
            messageText.text = message;
            gameObject.SetActive(true);
        }

        public void HidePopup()
        {
            gameObject.SetActive(false);
        }
    }