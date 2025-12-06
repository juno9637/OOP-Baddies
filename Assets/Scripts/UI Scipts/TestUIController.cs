using UnityEngine;

using UnityEngine;
using UnityEngine.UIElements;
using System;

/// <summary>
/// Example MonoBehaviour demonstrating the refactored UI builder patterns.
/// Attach this to a GameObject with a UIDocument component.
/// </summary>
public class TestUIController : MonoBehaviour
{
    private UIDocument uiDocument;
    private VisualElement root;

    // UI Elements
    private UI_Element masterVolumeSlider;
    private UI_Element simpleMenu;
    private UI_Element questionPopup;
    
    private UI_Factory factory = new UI_Factory();

    void OnEnable()
    {
        uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;

        CreateUI();
    }

    private void CreateUI()
    {
        masterVolumeSlider = factory.CreateSlider("Master Volume", .8f, 350f);
        
        root.Add(masterVolumeSlider.GetRootVisualElement());
        
        new UI_Builder(masterVolumeSlider)
            .RotateOnMouseDown()
            .SetInitialPosition(50, 50)
            .Build();

        // ============================================================
        simpleMenu = factory.CreateMenu("Simple Menu", 1);
        root.Add(simpleMenu.GetRootVisualElement());
        
        new UI_Builder(simpleMenu)
            .Draggable()
            .IsScalable()
            .SetInitialPosition(450, 50)
            .Build();

        
        // =========================================

        questionPopup = factory.CreatePopup("Confirm Action",
            "Are you sure you want to delete this item?\nThis action cannot be undone.", "?", new Color(0f, 0.5f, 0f),
            buttonLabels: new[] { "Yes", "No" });
        
        questionPopup.SetButtonCallback(0, () => Debug.Log("Item deleted!"));
        questionPopup.SetButtonCallback(1, () => Debug.Log("Deletion cancelled"));
        questionPopup.SetClosedCallback(questionPopup.Hide);
        
        root.Add(questionPopup.GetRootVisualElement());
        
        new UI_Builder(questionPopup)
            .Draggable()
            .SetInitialPosition(300, 350)
            .Build();
    }
}




















// /// <summary>
// /// Example custom UI_Element demonstrating the inner builder pattern.
// /// Shows how to create complex UI layouts using the fluent API.
// /// </summary>
// public class UI_SimplePanel : UI_Element
// {
//     private const string TITLE_LABEL = "panel-title";
//     private const string CLOSE_BUTTON = "close-button";
//     private const string CONTENT_CONTAINER = "content-container";
//
//     private readonly string title;
//     private Label titleLabel;
//     private Button closeButton;
//     private VisualElement contentContainer;
//
//     public UI_SimplePanel(string title = "Panel")
//     {
//         this.title = title;
//         
//         Initialize();
//         // Base constructor calls Initialize()
//     }
//
//     protected override void BuildVisualTree()
//     {
//         BeginRoot()
//             .WithSize(300, 200)
//             .WithBackgroundColor(0.15f, 0.15f, 0.15f, 0.95f)
//             .WithBorderRadius(8)
//             .WithBorder(1, new Color(0.3f, 0.3f, 0.3f))
//             .WithFlexDirection(FlexDirection.Column)
//
//             // Header
//             .BeginRow("header")
//                 .WithBackgroundColor(0.2f, 0.2f, 0.2f)
//                 .WithPadding(10)
//                 .WithJustifyContent(Justify.SpaceBetween)
//                 .WithAlignItems(Align.Center)
//                 .WithBorderRadius(8, 8, 0, 0)
//
//                 .AddLabel(title, TITLE_LABEL)
//                     .WithFontSize(16)
//                     .WithBold()
//                     .WithTextColor(Color.white)
//                     .StoreAs(out titleLabel)
//
//                 .AddButton("×", OnCloseClicked, CLOSE_BUTTON)
//                     .WithSize(24, 24)
//                     .WithBackgroundColor(0.4f, 0.15f, 0.15f)
//                     .WithBorderRadius(4)
//                     .WithTextColor(Color.white)
//                     .StoreAs(out closeButton)
//
//             .EndContainer()
//
//             // Content area
//             .BeginContainer(CONTENT_CONTAINER)
//                 .WithFlexGrow(1)
//                 .WithPadding(15)
//                 .WithFlexDirection(FlexDirection.Column)
//                 .StoreAs(out contentContainer)
//
//                 .AddLabel("Panel content goes here.")
//                     .WithTextColor(new Color(0.7f, 0.7f, 0.7f))
//
//                 .AddButton("Action Button", OnActionClicked)
//                     .WithMarginTop(10)
//                     .WithBackgroundColor(0.2f, 0.5f, 0.8f)
//                     .WithTextColor(Color.white)
//                     .WithBorderRadius(4)
//                     .WithPadding(8, 16)
//
//             .EndContainer();
//     }
//
//     protected override void SetupInteractions()
//     {
//         // Interactions are set up inline via the builder (OnClick callbacks)
//     }
//
//     protected override void UpdateVisuals()
//     {
//         // No dynamic visuals to update in this simple example
//     }
//
//     private void OnCloseClicked()
//     {
//         Debug.Log("Close clicked");
//         Hide();
//     }
//
//     private void OnActionClicked()
//     {
//         Debug.Log("Action clicked!");
//     }
//     
//     public void AddToContent(VisualElement element)
//     {
//         contentContainer?.Add(element);
//     }
//     
//     public void SetTitle(string newTitle)
//     {
//         if (titleLabel != null)
//         {
//             titleLabel.text = newTitle;
//         }
//     }
// }





// /// <summary>
// /// Example: Creating a completely custom control with complex interactions.
// /// </summary>
// public class UI_ColorPicker : UI_Element
// {
//     private VisualElement colorPreview;
//     private UI_SoundSlider redSlider;
//     private UI_SoundSlider greenSlider;
//     private UI_SoundSlider blueSlider;
//
//     private Color currentColor = Color.white;
//     private Action<Color> onColorChanged;
//
//     public UI_ColorPicker(Action<Color> colorChangedCallback = null)
//     {
//         onColorChanged = colorChangedCallback;
//     }
//
//     protected override void BuildVisualTree()
//     {
//         BeginRoot()
//             .WithSize(320, 280)
//             .WithBackgroundColor(0.12f, 0.12f, 0.12f, 0.98f)
//             .WithBorderRadius(10)
//             .WithPadding(15)
//             .WithFlexDirection(FlexDirection.Column)
//
//             // Title
//             .AddLabel("Color Picker")
//                 .WithFontSize(18)
//                 .WithBold()
//                 .WithTextColor(Color.white)
//                 .WithMarginBottom(15)
//
//             // Color preview
//             .AddElement("color-preview")
//                 .WithHeight(50)
//                 .WithBackgroundColor(currentColor)
//                 .WithBorderRadius(6)
//                 .WithMarginBottom(15)
//                 .StoreAs(out colorPreview);
//
//         // Create sliders (they self-initialize)
//         redSlider = new UI_SoundSlider("Red", currentColor.r, 280, OnRedChanged);
//         redSlider.SetFillColor(new Color(0.8f, 0.2f, 0.2f));
//         rootVisualElement.Add(redSlider.GetRootVisualElement());
//
//         greenSlider = new UI_SoundSlider("Green", currentColor.g, 280, OnGreenChanged);
//         greenSlider.SetFillColor(new Color(0.2f, 0.8f, 0.2f));
//         rootVisualElement.Add(greenSlider.GetRootVisualElement());
//
//         blueSlider = new UI_SoundSlider("Blue", currentColor.b, 280, OnBlueChanged);
//         blueSlider.SetFillColor(new Color(0.2f, 0.2f, 0.8f));
//         rootVisualElement.Add(blueSlider.GetRootVisualElement());
//     }
//
//     protected override void SetupInteractions()
//     {
//         // Sliders handle their own interactions
//     }
//
//     protected override void UpdateVisuals()
//     {
//         if (colorPreview != null)
//         {
//             colorPreview.style.backgroundColor = new StyleColor(currentColor);
//         }
//     }
//
//     private void OnRedChanged(float value)
//     {
//         currentColor.r = value;
//         UpdateVisuals();
//         onColorChanged?.Invoke(currentColor);
//     }
//
//     private void OnGreenChanged(float value)
//     {
//         currentColor.g = value;
//         UpdateVisuals();
//         onColorChanged?.Invoke(currentColor);
//     }
//
//     private void OnBlueChanged(float value)
//     {
//         currentColor.b = value;
//         UpdateVisuals();
//         onColorChanged?.Invoke(currentColor);
//     }
//
//     public Color GetColor() => currentColor;
//
//     public void SetColor(Color color)
//     {
//         currentColor = color;
//         redSlider?.SetValue(color.r);
//         greenSlider?.SetValue(color.g);
//         blueSlider?.SetValue(color.b);
//         UpdateVisuals();
//     }
// }
