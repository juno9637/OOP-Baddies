using UnityEngine;

using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class UI_SimpleMenu : UI_Element
{
    #region Consts

    //Windows Palette
    private static readonly Color WIN_BACKGROUND = new Color(0.753f, 0.753f, 0.753f);
    private static readonly Color WIN_TITLE_BAR = new Color(0.0f, 0.0f, 0.502f);
    private static readonly Color WIN_TITLE_BAR_INACTIVE = new Color(0.502f, 0.502f, 0.502f);
    private static readonly Color WIN_TITLE_TEXT = Color.white;
    
    //Border Palette
    private static readonly Color WIN_BORDER_LIGHT = Color.white;
    private static readonly Color WIN_BORDER_DARK = new Color(0.502f, 0.502f, 0.502f);
    
    //Button Palette
    private static readonly Color WIN_BUTTON_FACE = new Color(0.753f, 0.753f, 0.753f);
    private static readonly Color WIN_BUTTON_TEXT = Color.black;
    
    //Elements
    private const string TITLE_BAR = "title-bar";
    private const string TITLE_LABEL = "title-label";
    private const string CLOSE_BUTTON = "close-button";
    private const string CONTENT_AREA = "content-area";
    private const string BUTTON_CONTAINER = "button-container";

    #endregion

    #region Variables

    //Config
    private string title;
    private int buttonCount;
    private float menuWidth;
    private float buttonHeight;
    private float buttonSpacing;
    private string[] buttonLabels;
    
    private bool isActive = true;

    // Visual Elements
    private VisualElement titleBar;
    private Label titleLabel;
    private Button quitButton;
    private VisualElement contentArea;
    private VisualElement buttonContainer;
    private List<Button> buttons;
    
    //Events
    private Action onCloseCallback;
    private List<Action> onClickCallbacks;

    #endregion

    #region Constructor
    
    public UI_SimpleMenu(
        string title = "Menu",
        int buttonCount = 3,
        string[] buttonLabels = null,
        float width = 250f,
        float buttonHeight = 26f,
        float buttonSpacing = 6f)
    {
        this.title = title;
        this.buttonCount = Mathf.Max(1, buttonCount);
        this.menuWidth = width;
        this.buttonHeight = buttonHeight;
        this.buttonSpacing = buttonSpacing;
        this.buttonLabels = buttonLabels ?? new[] { "Press" };
        
        this.buttons = new List<Button>();
        this.onClickCallbacks = new List<Action>();
        
        //Populate Callback List
        for (int i = 0; i < this.buttonCount; i++)
        {
            onClickCallbacks.Add(null);
        }
        
        Initialize();
    }

    #endregion

    #region Abstract Implementation

    protected override void BuildVisualTreeFile()
    {
        float titleBarHeight = 22f;
        float contentPadding = 12f;
        float totalButtonsHeight = (buttonCount * buttonHeight) + ((buttonCount - 1) * buttonSpacing);
        float totalHeight = titleBarHeight + (contentPadding * 2) + totalButtonsHeight + 8f;

        BeginRoot()
            .WithSize(menuWidth, totalHeight)
            .WithBackgroundColor(WIN_BACKGROUND);
        
        ApplyRaisedBorderTo(rootVisualElement);
        
        BuildInnerElements();
    }

    protected override void SetupInteractions()
    {
    }

    protected override void UpdateVisuals()
    {
        UpdateTitleBarState();
    }

    public override void SetButtonCallback(int index, Action callback)
    {
        if (index >= 0 && index < onClickCallbacks.Count)
        {
            onClickCallbacks[index] = callback;
        }
    }

    public override void SetClosedCallback(Action callback)
    {
        onCloseCallback = callback;
    }

    public override void SetText(string text)
    {
        titleLabel.text = text;
    }

    #endregion

    #region Visual Tree Construction

    private void BuildInnerElements()
    {
        BuildTitleBar();
        BuildMiddleButtonArea();
    }
    
    private void BuildTitleBar()
    {
        void BuildTitleContainer()
        {
            titleBar = new VisualElement();
            titleBar.name = TITLE_BAR;
            titleBar.style.flexDirection = FlexDirection.Row;
            titleBar.style.justifyContent = Justify.SpaceBetween;
            titleBar.style.alignItems = Align.Center;
            titleBar.style.height = 22f;
            titleBar.style.marginTop = 3f;
            titleBar.style.marginLeft = 3f;
            titleBar.style.marginRight = 3f;
            titleBar.style.paddingLeft = 4f;
            titleBar.style.paddingRight = 2f;
            titleBar.style.backgroundColor = new StyleColor(WIN_TITLE_BAR);
            
            rootVisualElement.Add(titleBar);
        }
        BuildTitleContainer();

        
        void BuildTitleElement()
        {
            titleLabel = new Label(title);
            titleLabel.name = TITLE_LABEL;
            titleLabel.style.color = new StyleColor(WIN_TITLE_TEXT);
            titleLabel.style.fontSize = 12f;
            titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            titleLabel.style.unityTextAlign = TextAnchor.MiddleLeft;
            titleLabel.style.flexGrow = 1f;
            titleLabel.style.overflow = Overflow.Hidden;
            titleLabel.style.textOverflow = TextOverflow.Ellipsis;
            
            titleBar.Add(titleLabel);
        }
        BuildTitleElement();

        
        void BuildQuitButton()
        {
            quitButton = new Button();
            quitButton.name = CLOSE_BUTTON;
            quitButton.text = "×";
            quitButton.style.width = 18f;
            quitButton.style.height = 16f;
            quitButton.style.fontSize = 12f;
            quitButton.style.unityFontStyleAndWeight = FontStyle.Bold;
            quitButton.style.unityTextAlign = TextAnchor.MiddleCenter;
            quitButton.style.paddingTop = 0;
            quitButton.style.paddingBottom = 0;
            quitButton.style.paddingLeft = 0;
            quitButton.style.paddingRight = 0;
            quitButton.style.marginLeft = 2f;
            quitButton.style.backgroundColor = new StyleColor(WIN_BUTTON_FACE);
            quitButton.style.color = new StyleColor(WIN_BUTTON_TEXT);
            
            ApplyRaisedBorderTo(quitButton, 1f);
            ApplyButtonBehaviorTo(quitButton);
            
            titleBar.Add(quitButton);
        }
        BuildQuitButton();
        
        if (quitButton != null) { quitButton.clicked += OnQuitClicked; }
    }
    
    private void BuildMiddleButtonArea()
    {
        void BuildMidContentContainer()
        {
            contentArea = new VisualElement();
            contentArea.name = CONTENT_AREA;
            contentArea.style.flexGrow = 1f;
            contentArea.style.marginTop = 6f;
            contentArea.style.marginBottom = 6f;
            contentArea.style.marginLeft = 6f;
            contentArea.style.marginRight = 6f;
            contentArea.style.paddingTop = 10f;
            contentArea.style.paddingBottom = 10f;
            contentArea.style.paddingLeft = 10f;
            contentArea.style.paddingRight = 10f;
        
            ApplySunkenBorderTo(contentArea);
            
            rootVisualElement.Add(contentArea);
        }
        BuildMidContentContainer();


        void BuildButtonContainer()
        {
            buttonContainer = new VisualElement();
            buttonContainer.name = BUTTON_CONTAINER;
            buttonContainer.style.flexDirection = FlexDirection.Column;
            buttonContainer.style.alignItems = Align.Stretch;
        
            contentArea.Add(buttonContainer);
        }
        BuildButtonContainer();

        void CreateButtons()
        {
            for (int i = 0; i < buttonCount; i++)
            {
                CreateButton(i, buttonLabels[i]);
            }
        }
        CreateButtons();

    }

    private void CreateButton(int buttonIndex, string buttonName)
    {
        var button = new Button();
        
        void BuildButtonElement()
        {
            button.name = $"button_{buttonIndex}";
            button.text = buttonName;
            button.style.height = buttonHeight;
            button.style.fontSize = 12f;
            button.style.unityTextAlign = TextAnchor.MiddleCenter;
            button.style.backgroundColor = new StyleColor(WIN_BUTTON_FACE);
            button.style.color = new StyleColor(WIN_BUTTON_TEXT);
            
            if (buttonIndex > 0)
            {
                button.style.marginTop = buttonSpacing;
            }
        
            ApplyRaisedBorderTo(button, 2f);
            ApplyButtonBehaviorTo(button);

            buttonContainer.Add(button);
            buttons.Add(button);
        }
        BuildButtonElement();

        button.clicked += () => OnButtonClicked(buttonIndex);
    }

    #endregion

    #region Windows Effects
    
    private void ApplyRaisedBorderTo(VisualElement element, float width = 2f)
    {
        element.style.borderTopColor = new StyleColor(WIN_BORDER_LIGHT);
        element.style.borderLeftColor = new StyleColor(WIN_BORDER_LIGHT);
        element.style.borderTopWidth = width;
        element.style.borderLeftWidth = width;
        
        element.style.borderBottomColor = new StyleColor(WIN_BORDER_DARK);
        element.style.borderRightColor = new StyleColor(WIN_BORDER_DARK);
        element.style.borderBottomWidth = width;
        element.style.borderRightWidth = width;
    }
    
    private void ApplySunkenBorderTo(VisualElement element, float width = 2f)
    {
        element.style.borderTopColor = new StyleColor(WIN_BORDER_DARK);
        element.style.borderLeftColor = new StyleColor(WIN_BORDER_DARK);
        element.style.borderTopWidth = width;
        element.style.borderLeftWidth = width;
        
        element.style.borderBottomColor = new StyleColor(WIN_BORDER_LIGHT);
        element.style.borderRightColor = new StyleColor(WIN_BORDER_LIGHT);
        element.style.borderBottomWidth = width;
        element.style.borderRightWidth = width;
    }
    
    private void ApplyPressedBorderTo(VisualElement element, float width = 2f)
    {
        ApplySunkenBorderTo(element, width);
    }
    
    private void ApplyButtonBehaviorTo(Button button)
    {
        float borderWidth = 2f;
        
        button.RegisterCallback<MouseDownEvent>(evt =>
        {
            if (evt.button == 0)
            {
                ApplyPressedBorderTo(button, borderWidth);
            }
        });

        button.RegisterCallback<MouseUpEvent>(evt =>
        {
            if (evt.button == 0)
            {
                ApplyRaisedBorderTo(button, borderWidth);
            }
        });

        button.RegisterCallback<MouseLeaveEvent>(evt =>
        {
            ApplyRaisedBorderTo(button, borderWidth);
        });
    }

    #endregion

    #region Event Handlers

    private void OnQuitClicked()
    {
        onCloseCallback?.Invoke();
        Hide();
    }

    private void OnButtonClicked(int index)
    {
        if (index >= 0 && index < onClickCallbacks.Count)
        {
            onClickCallbacks[index]?.Invoke();
        }
    }

    #endregion

    #region Visual Updates

    private void UpdateTitleBarState()
    {
        if (titleBar != null)
        {
            titleBar.style.backgroundColor = new StyleColor(
                isActive ? WIN_TITLE_BAR : WIN_TITLE_BAR_INACTIVE
            );
        }
    }

    #endregion

    #region Get/Set
    
    public void SetButtonText(int index, string text)
    {
        if (index >= 0 && index < buttons.Count)
        {
            buttons[index].text = text;
        }
    }
    
    public void SetTitle(string newTitle)
    {
        if (titleLabel != null)
        {
            titleLabel.text = newTitle;
        }
    }

    #endregion
}