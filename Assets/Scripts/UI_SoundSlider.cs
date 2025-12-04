using System;
using UnityEngine;
using UnityEngine.UIElements;

public class UI_SoundSlider : UI_Element
{
    #region Constants

    private const string TITLE_LABEL = "title-label";
    private const string VALUE_LABEL = "value-label";
    private const string SLIDER_TRACK = "slider-track";
    private const string BACKGROUND_BAR = "background-bar";
    private const string FILLED_BAR = "filled-bar";
    private const string SLIDER_THUMB = "slider-thumb";

    // Windows Pallette
    private static readonly Color WIN_BACKGROUND = new Color(0.753f, 0.753f, 0.753f);
    private static readonly Color WIN_TITLE_BAR = new Color(0.0f, 0.0f, 0.502f);
    private static readonly Color WIN_BORDER_LIGHT = Color.white;
    private static readonly Color WIN_BORDER_DARK = new Color(0.502f, 0.502f, 0.502f);
    private static readonly Color WIN_BUTTON_FACE = new Color(0.753f, 0.753f, 0.753f);
    private static readonly Color WIN_TEXT = Color.black;
    private static readonly Color WIN_TRACK_BG = new Color(0.875f, 0.875f, 0.875f);

    #endregion

    #region Variables
    
    //Config
    private string title;
    private float trackWidth;
    private float trackHeight;
    private float thumbSize;
    private float minValue;
    private float maxValue;

    //State
    private float currentValue;
    private bool isDragging;
    private bool fillByRotate;
    private bool isRotating;
    
    //Event
    private Action<float> onValueChanged;

    //Visual Elements
    private Label titleLabel;
    private Label valueLabel;
    private VisualElement sliderTrack;
    private VisualElement sliderThumb;
    private VisualElement backgroundBar;
    private VisualElement filledBar;

    #endregion

    #region Constructor

    public UI_SoundSlider(
        string title = "Volume",
        float defaultValue = 0.75f,
        float width = 400f,
        Action<float> valueChangedCallback = null,
        float trackHeight = 8f,
        float thumbSize = 20f,
        bool fillByRotate = true)
    {
        this.title = title;
        this.trackHeight = trackHeight;
        this.thumbSize = thumbSize;
        this.fillByRotate = fillByRotate;
        this.currentValue = Mathf.Clamp01(defaultValue);
        this.trackWidth = width - 100f;
        this.minValue = 0f;
        this.maxValue = 1f;
        this.onValueChanged = valueChangedCallback;
        
        Initialize();
    }

    #endregion

    #region Abstract Implementation

    protected override void BuildVisualTreeFile()
    {
        BeginRoot()
            .WithFlexDirection(FlexDirection.Column)
            .WithWidth(trackWidth + 100f)
            .WithBackgroundColor(WIN_BACKGROUND)
            .WithPadding(10)
            
            //Title Row
            .BeginRow(name: "header-row")
                .WithJustifyContent(Justify.SpaceBetween)
                .WithMarginBottom(8)
                .AddLabel(title, TITLE_LABEL)
                    .WithFontSize(12)
                    .WithBold()
                    .WithTextColor(WIN_TEXT)
                    .StoreAs(out titleLabel)
                .AddLabel(FormatValue(currentValue), VALUE_LABEL)
                    .WithFontSize(12)
                    .WithTextColor(WIN_TEXT)
                    .StoreAs(out valueLabel)
            .EndContainer()
            
            //Slidertrack Container
            .BeginContainer(SLIDER_TRACK)
                .WithWidth(trackWidth)
                .WithHeight(trackHeight + thumbSize)
                .WithRelativePosition()
                .StoreAs(out sliderTrack)

                //Background Bar
                .AddElement(BACKGROUND_BAR)
                    .WithAbsolutePosition()
                    .WithPosition(0, thumbSize / 2f - trackHeight / 2f)
                    .WithWidth(new Length(100, LengthUnit.Percent))
                    .WithHeight(trackHeight)
                    .WithBackgroundColor(WIN_TRACK_BG)
                    .StoreAs(out backgroundBar)

                //Filled Bar
                .AddElement(FILLED_BAR)
                    .WithAbsolutePosition()
                    .WithPosition(2, thumbSize / 2f - trackHeight / 2f + 2)
                    .WithWidth(new Length(currentValue * 100, LengthUnit.Percent))
                    .WithHeight(trackHeight - 4)
                    .WithBackgroundColor(WIN_TITLE_BAR)
                    .StoreAs(out filledBar)

                //Thumb
                .AddElement(SLIDER_THUMB)
                    .WithAbsolutePosition()
                    .WithSize(thumbSize, thumbSize)
                    .WithBackgroundColor(WIN_BUTTON_FACE)
                    .StoreAs(out sliderThumb)

            .EndContainer();
        
        ApplyRaisedBorderTo(rootVisualElement);
        ApplySunkenBorderTo(backgroundBar, 1f);
        ApplyRaisedBorderTo(sliderThumb);
        
        UpdateThumbPosition();
    }

    protected override void SetupInteractions()
    {
        SetupRootRotation();
        SetupThumbDragging();
        SetupTrackClickToJump();
    }

    protected override void UpdateVisuals()
    {
        UpdateFilledBar();
        UpdateThumbPosition();
        UpdateValueLabel();
    }

    public override void SetClosedCallback(Action callback)
    {
        throw new NotImplementedException();
    }

    public override void SetButtonCallback(int index, Action callback)
    {
        throw new NotImplementedException();
    }

    #endregion

    #region Interaction Setup

    private void SetupRootRotation()
    {
        rootVisualElement.RegisterCallback<MouseDownEvent>(OnRootDown);
        rootVisualElement.RegisterCallback<MouseMoveEvent>(OnRootMove);
        rootVisualElement.RegisterCallback<MouseUpEvent>(OnRootUp);
    }
    
    private void SetupThumbDragging()
    {
        sliderThumb.RegisterCallback<MouseDownEvent>(OnThumbMouseDown);
        sliderThumb.RegisterCallback<MouseMoveEvent>(OnThumbMouseMove);
        sliderThumb.RegisterCallback<MouseUpEvent>(OnThumbMouseUp);
    }
    
    private void SetupTrackClickToJump()
    {
        sliderTrack.RegisterCallback<MouseDownEvent>(OnTrackClick);
    }

    #region Root events

    private void OnRootDown(MouseDownEvent evt)
    {
        if (evt.button != 0) return;
        
        isRotating = true;
        rootVisualElement.CaptureMouse();
        
        evt.StopPropagation();
    }
    
    private void OnRootMove(MouseMoveEvent evt)
    {
        if (!isRotating) return;
        
        UpdateValueFromRotation();
        
        evt.StopPropagation();
    }

    private void OnRootUp(MouseUpEvent evt)
    {
        if(!isRotating || evt.button != 0) return;
        isRotating = false;
        rootVisualElement.ReleaseMouse();

        evt.StopPropagation();
    }

    #endregion
    
    #region Thumb Mouse Events

    private void OnThumbMouseDown(MouseDownEvent evt)
    {
        if (evt.button != 0) return;
        
        isDragging = true;
        sliderThumb.CaptureMouse();
        
        ApplySunkenBorderTo(sliderThumb);
        sliderThumb.style.scale = new StyleScale(new Scale(Vector2.one * 0.95f));
        
        evt.StopPropagation();
    }

    private void OnThumbMouseMove(MouseMoveEvent evt)
    {
        if (!isDragging) return;
        
        if(!fillByRotate){UpdateValueFromMousePosition(evt.mousePosition);}
        
        evt.StopPropagation();
    }

    private void OnThumbMouseUp(MouseUpEvent evt)
    {
        if(!isDragging || evt.button != 0) return;
        isDragging = false;
        sliderThumb.ReleaseMouse();
        
        ApplyRaisedBorderTo(sliderThumb);
        sliderThumb.style.scale = new StyleScale(new Scale(Vector2.one));

        evt.StopPropagation();
    }

    #endregion
    

    private void OnTrackClick(MouseDownEvent evt)
    {
        if (evt.button != 0) return;
        if (evt.target == sliderThumb) return;
        
        UpdateValueFromMousePosition(evt.mousePosition);
        evt.StopPropagation();
    }

    #endregion

    #region UpdateValues

    private void UpdateValueFromMousePosition(Vector2 mousePosition)
    {
        Vector2 trackWorldPosition = sliderTrack.worldBound.position;
        float relativeX = mousePosition.x - trackWorldPosition.x;
        relativeX = Mathf.Clamp(relativeX, 0, trackWidth);

        float updateValue = relativeX / trackWidth;
        UpdateSetFillValue(updateValue);
    }

    private void UpdateValueFromRotation()
    {
        float rotation = rootVisualElement.resolvedStyle.rotate.angle.value; //% 360
        rotation = (rotation % 360f + 360f) % 360f;
        
        float radians = rotation * Mathf.Deg2Rad;
        float gravityT = 0.5f * (1f - Mathf.Cos(radians));
        
        float relativeFill = gravityT * trackWidth;
        
        Debug.Log($"Fill: {relativeFill}");
        UpdateSetFillValue(relativeFill);
    }

    private void UpdateSetFillValue(float newValue)
    {
        newValue = Mathf.Clamp(newValue, minValue,maxValue);

        if (Mathf.Abs(newValue - currentValue) > .001f)
        {
            currentValue = newValue;
            UpdateVisuals();
            onValueChanged?.Invoke(currentValue);
        }
    }

    #endregion

    #region Update Visuals

    private void UpdateFilledBar()
    {
        if (filledBar == null) return;
        filledBar.style.width = new Length(currentValue * 100, LengthUnit.Percent);
    }

    private void UpdateThumbPosition()
    {
        if (sliderThumb == null) return;
        
        float thumbX = (currentValue * trackWidth);
        sliderThumb.style.left = thumbX;
        sliderThumb.style.top = 0;
    }

    private void UpdateValueLabel()
    {
        if (valueLabel == null) return;
        valueLabel.text = FormatValue(currentValue);
    }
    
    private string FormatValue(float value) => $"{value * 100:F0}%";

    #endregion

    #region Getters And Setters
    
    public void SetTitle(string newTitle)
    {
        if (titleLabel != null)
        {
            titleLabel.text = newTitle;
        }
    }
    
    public string GetTitle() => titleLabel?.text ?? title;
    
    public void SetFillColor(Color color)
    {
        if (filledBar != null)
        {
            filledBar.style.backgroundColor = new StyleColor(color);
        }
    }
    
    public void SetBackgroundColor(Color color)
    {
        if (backgroundBar != null)
        {
            backgroundBar.style.backgroundColor = new StyleColor(color);
        }
    }
    
    public void SetThumbColor(Color color)
    {
        if (sliderThumb != null)
        {
            sliderThumb.style.backgroundColor = new StyleColor(color);
        }
    }

    #endregion
    
    #region WindowsEffects

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

    #endregion
}