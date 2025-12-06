using UnityEngine;
using UnityEngine.UIElements;
using System;
using Random = UnityEngine.Random;

public class UI_Builder
{
    private readonly VisualElement element;
    private readonly UI_Element uiElement;

    #region Constructors

    /// <summary>
    /// Create a builder for a UI_Element's root visual element.
    /// This is the preferred way to use UI_Builder.
    /// </summary>
    public UI_Builder(UI_Element uiElement)
    {
        this.uiElement = uiElement;
        this.element = uiElement.GetRootVisualElement();
    }

    /// <summary>
    /// Create a builder for a raw VisualElement.
    /// Use this for applying behaviors to standalone visual elements.
    /// </summary>
    public UI_Builder(VisualElement element)
    {
        this.element = element;
        this.uiElement = null;
    }

    #endregion

    #region Behavior Methods
    
    public UI_Builder Draggable()
    {
        element.AddToClassList("draggable");
        element.style.position = Position.Absolute;
        element.AddManipulator(new DragManipulator());
        return this;
    }
    
    public UI_Builder BouncingScreenSaver(float speedX = 2f, float speedY = 2f)
    {
        float vx = speedX;
        float vy = speedY;
        element.style.position = Position.Absolute;

        element.schedule.Execute(() =>
        {
            var parent = element.parent;
            if (parent == null) return;

            float currentX = element.style.left.value.value;
            float currentY = element.style.top.value.value;
            float newX = currentX + vx;
            float newY = currentY + vy;

            if (newX < 0 || newX + element.layout.width > parent.layout.width)
            {
                vx = -vx;
                newX = Mathf.Clamp(newX, 0, Mathf.Max(0, parent.layout.width - element.layout.width));
            }
            
            if (newY < 0 || newY + element.layout.height > parent.layout.height)
            {
                vy = -vy;
                newY = Mathf.Clamp(newY, 0, Mathf.Max(0, parent.layout.height - element.layout.height));
            }

            element.style.left = new Length(newX, LengthUnit.Pixel);
            element.style.top = new Length(newY, LengthUnit.Pixel);
        }).Every(16);

        return this;
    }
    
    public UI_Builder Rotate(float degreesPerSecond = 90f)
    {
        float angle = 0f;

        element.schedule.Execute(() =>
        {
            angle += degreesPerSecond * 0.016f;
            if (angle > 360f) angle -= 360f;

            element.style.rotate = new StyleRotate(new Rotate(new Angle(angle)));
        }).Every(16);

        return this;
    }
    
    public UI_Builder RotateOnMouseDown()
    {
        element.AddToClassList("rotatable");
        element.AddManipulator(new RotateManipulator());

        // Set transform origin to center
        element.style.transformOrigin = new TransformOrigin(
            new Length(50, LengthUnit.Percent),
            new Length(50, LengthUnit.Percent)
        );

        return this;
    }
    
    public UI_Builder IsScalable(float minScale = 0.5f, float maxScale = 3f, float zoomStep = 0.1f)
    {
        element.RegisterCallback<WheelEvent>(evt =>
        {
            float zoomAmount = evt.delta.y < 0 ? zoomStep : -zoomStep;
            var currentScale = element.resolvedStyle.scale.value;

            float newScaleX = Mathf.Clamp(currentScale.x + zoomAmount, minScale, maxScale);
            float newScaleY = Mathf.Clamp(currentScale.y + zoomAmount, minScale, maxScale);

            element.style.scale = new StyleScale(new Scale(new Vector2(newScaleX, newScaleY)));

            evt.StopPropagation();
        });

        return this;
    }
    
    public UI_Builder SetInitialPosition(float left, float top)
    {
        element.style.position = Position.Absolute;
        element.style.left = left;
        element.style.top = top;
        return this;
    }
    
    public UI_Builder SetRandomInitialPosition()
    {
        element.style.position = Position.Absolute;
        element.style.left = Random.Range(10, 350);
        element.style.top = Random.Range(10, 350);;
        return this;
    }
    
    public UI_Builder OnClick(Action callback)
    {
        if (element is Button button)
        {
            button.clicked += callback;
        }
        else
        {
            element.AddManipulator(new Clickable(callback));
        }

        return this;
    }

    #endregion

    #region Build
    
    public UI_Element BuildElement()
    {
        return uiElement;
    }
    
    public VisualElement Build()
    {
        return element;
    }

    #endregion
    
}

#region Manipulators

/// <summary>
/// Manipulator that allows dragging an element by mouse.
/// </summary>
public class DragManipulator : MouseManipulator
{
    private Vector2 startMousePosition;
    private Vector2 startElementPosition;
    private bool isDragging;

    public DragManipulator()
    {
        activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
    }

    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<MouseDownEvent>(OnMouseDown);
        target.RegisterCallback<MouseMoveEvent>(OnMouseMove);
        target.RegisterCallback<MouseUpEvent>(OnMouseUp);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
        target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
        target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
    }

    private void OnMouseDown(MouseDownEvent evt)
    {
        if (CanStartManipulation(evt))
        {
            isDragging = true;
            startMousePosition = evt.mousePosition;

            startElementPosition = new Vector2(
                target.style.left.value.value,
                target.style.top.value.value
            );

            target.AddToClassList("dragging");
            target.CaptureMouse();
            evt.StopPropagation();
        }
    }

    private void OnMouseMove(MouseMoveEvent evt)
    {
        if (isDragging)
        {
            Vector2 delta = evt.mousePosition - startMousePosition;

            target.style.left = new Length(startElementPosition.x + delta.x, LengthUnit.Pixel);
            target.style.top = new Length(startElementPosition.y + delta.y, LengthUnit.Pixel);

            evt.StopPropagation();
        }
    }

    private void OnMouseUp(MouseUpEvent evt)
    {
        if (isDragging && CanStopManipulation(evt))
        {
            isDragging = false;
            target.RemoveFromClassList("dragging");
            target.ReleaseMouse();
            evt.StopPropagation();
        }
    }
}

/// <summary>
/// Manipulator that allows rotating an element by mouse drag.
/// </summary>
public class RotateManipulator : MouseManipulator
{
    private bool isRotating;
    private float currentRotation = 0f;
    private float lastMouseAngle = 0f;

    public RotateManipulator()
    {
        activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
    }

    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<MouseDownEvent>(OnMouseDown);
        target.RegisterCallback<MouseMoveEvent>(OnMouseMove);
        target.RegisterCallback<MouseUpEvent>(OnMouseUp);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
        target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
        target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
    }

    private void OnMouseDown(MouseDownEvent evt)
    {
        if (CanStartManipulation(evt))
        {
            isRotating = true;
            target.AddToClassList("rotating");
            target.CaptureMouse();

            var currentStyle = target.resolvedStyle.rotate;
            currentRotation = currentStyle.angle.value;

            // Initialize last mouse angle
            Vector2 elementCenter = target.worldBound.center;
            float deltaX = evt.mousePosition.x - elementCenter.x;
            float deltaY = evt.mousePosition.y - elementCenter.y;
            lastMouseAngle = Mathf.Atan2(deltaY, deltaX) * Mathf.Rad2Deg;

            evt.StopPropagation();
        }
    }

    private void OnMouseMove(MouseMoveEvent evt)
    {
        if (isRotating)
        {
            RotateRelativeToMouse(evt.mousePosition);
            evt.StopPropagation();
        }
    }

    private void OnMouseUp(MouseUpEvent evt)
    {
        if (isRotating && CanStopManipulation(evt))
        {
            isRotating = false;
            target.RemoveFromClassList("rotating");
            target.ReleaseMouse();
            evt.StopPropagation();
        }
    }

    private void RotateRelativeToMouse(Vector2 mousePosition)
    {
        Vector2 elementCenter = target.worldBound.center;

        float deltaX = mousePosition.x - elementCenter.x;
        float deltaY = mousePosition.y - elementCenter.y;

        float currentMouseAngle = Mathf.Atan2(deltaY, deltaX) * Mathf.Rad2Deg;
        float angleDelta = Mathf.DeltaAngle(lastMouseAngle, currentMouseAngle);

        currentRotation += angleDelta;
        target.style.rotate = new StyleRotate(new Rotate(new Angle(currentRotation)));

        lastMouseAngle = currentMouseAngle;
    }
}

#endregion