using UnityEngine;

using UnityEngine;
using UnityEngine.UIElements;
using System;

public enum UI_Type
{
    Slider,
    Menu,
    ErrorMenu,
    SpamPopup,
    UnclosingPopup
}

public class UI_Controller : MonoBehaviour
{
    [SerializeField] private LevelManager levelManager;
    
    private UIDocument uiDocument;
    private VisualElement root;

    // UI Elements
    private UI_Element volumeSlider;
    private UI_Element settingsMenu;
    private UI_Element SpamPopup;
    private UI_Element UnclosingPopup;
    
    private UI_Factory factory = new UI_Factory();
    
    private ClickCounter clickCounter = new ClickCounter();
    private CloseButtonTracker closeTracker = new CloseButtonTracker();

    void OnEnable()
    {
        clickCounter.Attach(levelManager);
        closeTracker.Attach(levelManager);
        
        uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;
    }

    public void SetupLevel(int level)
    {
        switch (level)
        {
            case 1:
                CreateUI(UI_Type.Menu);
                break;
            case 2:
                CreateUI(UI_Type.SpamPopup);
                CreateUI(UI_Type.Slider);
                break;
            case 3:
                CreateUI(UI_Type.UnclosingPopup);
        
                for (int i = 0; i < 10; i++)
                {
                    CreateUI(UI_Type.SpamPopup);
                }
                break;
            case 4:
                //SetupLevel4();
                break;
        }
    }

    public void ClearUI()
    {
        root.Clear();
    }

    public void CreateUI(UI_Type type)
    {
        switch (type)
        {
            case UI_Type.Slider:
                volumeSlider = factory.CreateSlider("Please Reach 100% to Quit", .6f, 350f);
                volumeSlider.SetClosedCallback(closeTracker.OnClosePressed);
        
                root.Add(volumeSlider.GetRootVisualElement());
        
                new UI_Builder(volumeSlider)
                    .RotateOnMouseDown()
                    .SetRandomInitialPosition()
                    .Build();
                
                break;
            case UI_Type.Menu:
                settingsMenu = factory.CreateMenu("Settings", 1, new string[] { "Quit?" });
                
                settingsMenu.SetClosedCallback(settingsMenu.Destroy);
                settingsMenu.SetButtonCallback(0, () => closeTracker.OnClosePressed());
                
                root.Add(settingsMenu.GetRootVisualElement());
        
                new UI_Builder(settingsMenu)
                    .Draggable()
                    .IsScalable()
                    .SetRandomInitialPosition()
                    .Build();
                break;
            case UI_Type.SpamPopup:
                SpamPopup = factory.CreatePopup("Warning",
                    "Closing the game is not advised!! \n Unauthorized quitting may lead to instability", "!", Color.yellowNice,
                    buttonLabels: new[] { "Ok" });
        
                SpamPopup.SetButtonCallback(0, SpamPopup.Hide);
                SpamPopup.SetClosedCallback(() => CreateUI(UI_Type.SpamPopup));
        
                root.Add(SpamPopup.GetRootVisualElement());
        
                new UI_Builder(SpamPopup)
                    .Draggable()
                    .SetRandomInitialPosition()
                    .Build();
                break;
            case UI_Type.UnclosingPopup:
                UnclosingPopup = factory.CreatePopup("Quit",
                    "Are you sure you want to quit the game?", "?", Color.softRed,
                    buttonLabels: new[] { "Yes", "No" });
        
                UnclosingPopup.SetButtonCallback(0, () => closeTracker.OnClosePressed());
                Debug.Log("Gay");
                UnclosingPopup.SetButtonCallback(1, () => UnclosingPopup.SetText("Error"));
                UnclosingPopup.SetClosedCallback(() => { 
                    UnclosingPopup.Hide();
                    CreateUI(UI_Type.UnclosingPopup); 
                });
        
                root.Add(UnclosingPopup.GetRootVisualElement());
        
                new UI_Builder(UnclosingPopup)
                    .Draggable()
                    .SetRandomInitialPosition()
                    .Build();
                break;
            case UI_Type.ErrorMenu:
                settingsMenu = factory.CreateMenu("Settings", 1, new string[] { "Quit?" });
                
                settingsMenu.SetClosedCallback(settingsMenu.Destroy);
                settingsMenu.SetButtonCallback(0, () => settingsMenu.SetText("Failed To Quit!"));
                
                root.Add(settingsMenu.GetRootVisualElement());
        
                new UI_Builder(settingsMenu)
                    .Draggable()
                    .IsScalable()
                    .SetRandomInitialPosition()
                    .Build();
                break;
        }
    }

    void DestroyUI(UI_Element element)
    {
        element.Destroy();
    }
}