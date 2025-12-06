using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
public class UI_SimplePopUp : UI_Element
{
    #region Windows Info

    private static Color WIN_BACKGROUND = new Color(0.753f, 0.753f, 0.753f);
    private static Color WIN_TITLE_BAR = new Color(0.0f, 0.0f, 0.502f);
    private static Color WIN_TITLE_TEXT = Color.white;
    private static Color WIN_BORDER_LIGHT = Color.white;
    private static Color WIN_BORDER_DARK = new Color(0.502f, 0.502f, 0.502f);
    private static Color WIN_BUTTON_FACE = new Color(0.753f, 0.753f, 0.753f);
    private static Color WIN_BUTTON_TEXT = Color.black;

    #endregion

    #region variables
    
    private string title;
    private string message;
    private string iconSymbol;
    private Color iconColor;
    private string[] buttonLabels;
    private float popupWidth;
    private float buttonWidth;
    private float buttonHeight;
    private float buttonSpacing;

    private VisualElement titleBar;
    private Label titleLabel;
    private Button closeButton;
    private VisualElement iconContainer;
    private Label messageLabel;
    private VisualElement buttonRow;
    private List<Button> buttons;

    private Action onCloseClicked;
    private List<Action> onButtonClicked;

    #endregion

    #region Constructor
    
    public UI_SimplePopUp(
        string title = "Popup",
        string message = "Message goes here.",
        string iconSymbol = "i",
        Color? iconColor = null,
        string[] buttonLabels = null,
        float width = 380f,
        float buttonWidth = 75f)
    {
        this.title = title;
        this.message = message;
        this.iconSymbol = iconSymbol;
        this.iconColor = iconColor ?? new Color(0.0f, 0.4f, 0.8f);
        this.buttonLabels = buttonLabels ?? new[] { "OK" };
        this.popupWidth = width;
        this.buttonWidth = buttonWidth;
        this.buttonHeight = 26f;
        this.buttonSpacing = 8f;

        this.buttons = new List<Button>();
        this.onButtonClicked = new List<Action>();

        void PopulateList()
        {
            for (int i = 0; i < this.buttonLabels.Length; i++)
            {
                onButtonClicked.Add(null);
            }
        }
        PopulateList();
        
        Initialize();
    }

    #endregion

    #region Abstract Implementation

    protected override void BuildVisualTreeFile()
    {
        float titleBarHeight = 22f;
        float contentHeight = 60f;
        float buttonRowHeight = buttonHeight + 20f;
        float totalHeight = titleBarHeight + contentHeight + buttonRowHeight + 30f;

        BeginRoot()
            .WithSize(popupWidth, totalHeight)
            .WithBackgroundColor(WIN_BACKGROUND);

        ApplyRaisedBorderTo(rootVisualElement);

        BuildTitleBar();
        BuildContentArea();
        BuildButtonRow();
    }

    protected override void SetupInteractions()
    {
        AddButtonVisualBehaviorTo(closeButton);
        foreach (Button button in buttons)
        {
            AddButtonVisualBehaviorTo(button);
        }

    }

    public override void SetText(string text)
    {
        SetButtonText(1, text);
    }

    protected override void UpdateVisuals()
    {
        // No dynamic updates needed
    }
    
    public override void SetButtonCallback(int index, Action callback)
    {
        if (index >= 0 && index < onButtonClicked.Count)
        {
            onButtonClicked[index] = callback;
        }
    }
    
    public override void SetClosedCallback(Action callback)
    {
        onCloseClicked = callback;
    }

    #endregion

    #region Visual Tree Construction

    private void BuildTitleBar()
    {
        void BuildTitleContainer()
        {
            titleBar = new VisualElement();
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

        void BuildTitleIcon()
        {
            var titleIcon = new VisualElement();
            titleIcon.style.width = 14f;
            titleIcon.style.height = 14f;
            titleIcon.style.marginRight = 4f;
            titleIcon.style.backgroundColor = new StyleColor(iconColor);
            ApplyRaisedBorderTo(titleIcon, 1f);
            titleBar.Add(titleIcon);
        }
        BuildTitleIcon();

        void BuildTitleText()
        {
            titleLabel = new Label(title);
            titleLabel.style.color = new StyleColor(WIN_TITLE_TEXT);
            titleLabel.style.fontSize = 12f;
            titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            titleLabel.style.unityTextAlign = TextAnchor.MiddleLeft;
            titleLabel.style.flexGrow = 1f;
            titleLabel.style.overflow = Overflow.Hidden;

            titleBar.Add(titleLabel);
        }
        BuildTitleText();

        void BuildCloseButton()
        {
            closeButton = new Button();
            closeButton.text = "×";
            closeButton.style.width = 18f;
            closeButton.style.height = 16f;
            closeButton.style.fontSize = 12f;
            closeButton.style.unityFontStyleAndWeight = FontStyle.Bold;
            closeButton.style.unityTextAlign = TextAnchor.MiddleCenter;
            closeButton.style.paddingTop = 0;
            closeButton.style.paddingBottom = 0;
            closeButton.style.paddingLeft = 0;
            closeButton.style.paddingRight = 0;
            closeButton.style.marginLeft = 2f;
            closeButton.style.backgroundColor = new StyleColor(WIN_BUTTON_FACE);
            closeButton.style.color = new StyleColor(WIN_BUTTON_TEXT);

            ApplyRaisedBorderTo(closeButton, 1f);
        
            if (closeButton != null)
            {
                closeButton.clicked += OnCloseButtonClicked;
            }

            titleBar.Add(closeButton);
        }
        BuildCloseButton();
        
    }

    private void BuildContentArea()
    {
        var contentArea = new VisualElement();
        void BuildMiddleContainer()
        {
            contentArea.style.flexDirection = FlexDirection.Row;
            contentArea.style.flexGrow = 1f;
            contentArea.style.paddingTop = 15f;
            contentArea.style.paddingBottom = 10f;
            contentArea.style.paddingLeft = 15f;
            contentArea.style.paddingRight = 15f;
            contentArea.style.alignItems = Align.FlexStart;

            rootVisualElement.Add(contentArea);
        }
        BuildMiddleContainer();

        void BuildIconContainer()
        {
            iconContainer = new VisualElement();
            iconContainer.style.width = 48f;
            iconContainer.style.height = 48f;
            iconContainer.style.marginRight = 15f;
            iconContainer.style.flexShrink = 0f;
            iconContainer.style.alignItems = Align.Center;
            iconContainer.style.justifyContent = Justify.Center;
            
            contentArea.Add(iconContainer);
        }
        BuildIconContainer();
        
        void BuildIconFrameAndLabel()
        {
            var iconFrame = new VisualElement();
            iconFrame.style.width = 44f;
            iconFrame.style.height = 44f;
            iconFrame.style.backgroundColor = new StyleColor(LightenColor());
            iconFrame.style.alignItems = Align.Center;
            iconFrame.style.justifyContent = Justify.Center;
            ApplyRaisedBorderTo(iconFrame, 2f);

            var iconLabel = new Label(iconSymbol);
            iconLabel.style.fontSize = 28f;
            iconLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            iconLabel.style.color = new StyleColor(iconColor);
            iconLabel.style.unityTextAlign = TextAnchor.MiddleCenter;

            iconFrame.Add(iconLabel);
            iconContainer.Add(iconFrame);
        }
        BuildIconFrameAndLabel();
        
        void BuildMessageContainer()
        {
            var messageContainer = new VisualElement();
            
            messageContainer.style.flexGrow = 1f;
            messageContainer.style.flexShrink = 1f;
            messageContainer.style.justifyContent = Justify.Center;
            
            contentArea.Add(messageContainer);

            messageLabel = new Label(message);
            messageLabel.style.fontSize = 12f;
            messageLabel.style.color = new StyleColor(Color.black);
            messageLabel.style.whiteSpace = WhiteSpace.Normal;
            messageLabel.style.unityTextAlign = TextAnchor.MiddleLeft;

            messageContainer.Add(messageLabel);
        }
        BuildMessageContainer();
    }

    private void BuildButtonRow()
    {
        var separator = new VisualElement();
        separator.style.height = 2f;
        separator.style.marginLeft = 6f;
        separator.style.marginRight = 6f;
        separator.style.marginBottom = 8f;
        ApplySunkenBorderTo(separator, 1f);

        rootVisualElement.Add(separator);
        
        buttonRow = new VisualElement();
        buttonRow.style.flexDirection = FlexDirection.Row;
        buttonRow.style.justifyContent = Justify.Center;
        buttonRow.style.alignItems = Align.Center;
        buttonRow.style.paddingBottom = 10f;
        buttonRow.style.paddingLeft = 10f;
        buttonRow.style.paddingRight = 10f;

        rootVisualElement.Add(buttonRow);
        
        for (int i = 0; i < buttonLabels.Length; i++)
        {
            CreateButton(i, buttonLabels[i]);
        }
    }

    private void CreateButton(int buttonIndex, string label)
    {        
        var button = new Button();
        void BuildButtonElement()
        {
            button.name = $"popup-button-{buttonIndex}";
            button.text = label;
            button.style.width = buttonWidth;
            button.style.height = buttonHeight;
            button.style.fontSize = 12f;
            button.style.unityTextAlign = TextAnchor.MiddleCenter;
            button.style.backgroundColor = new StyleColor(WIN_BUTTON_FACE);
            button.style.color = new StyleColor(WIN_BUTTON_TEXT);
            
            if (buttonIndex > 0)
            {
                button.style.marginLeft = buttonSpacing;
            }
        
            ApplyRaisedBorderTo(button, 2f);
            
            buttonRow.Add(button);
            buttons.Add(button);
        }
        BuildButtonElement();
        
        button.clicked += () => OnButtonClicked(buttonIndex);
    }

    #endregion

    #region Windows Effects

    private Color LightenColor()
    {
        return new Color(
            Mathf.Lerp(iconColor.r, 1f, 0.85f),
            Mathf.Lerp(iconColor.g, 1f, 0.85f),
            Mathf.Lerp(iconColor.b, 1f, 0.85f)
        );
    }

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

    private void AddButtonVisualBehaviorTo(Button button)
    {
        float borderWidth = 2f;

        button.RegisterCallback<MouseDownEvent>(evt =>
        {
            if (evt.button == 0)
            {
                ApplySunkenBorderTo(button, borderWidth);
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

    private void OnCloseButtonClicked()
    {
        onCloseClicked?.Invoke();
    }

    private void OnButtonClicked(int index)
    {
        if (index >= 0 && index < onButtonClicked.Count)
        {
            onButtonClicked[index]?.Invoke();
        }
    }

    #endregion

    #region GetSet
    
    private void SetButtonText(int index, string text)
    {
        if (index >= 0 && index < buttons.Count)
        {
            buttons[index].text = text;
        }
    }

    #endregion
}