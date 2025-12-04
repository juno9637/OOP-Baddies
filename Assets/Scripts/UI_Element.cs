using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class UI_Element
{
    #region Fields

    protected VisualElement rootVisualElement;

    protected bool isVisible = true;
    protected bool isEnabled = true;
    protected bool isInitialized;

    protected string elementID;
    protected Vector2 position;
    protected Vector2 size;
    
    protected Action onShown;
    protected Action onHidden;
    protected Action onDestroyed;
    
    private Stack<VisualElement> containerStack;
    private VisualElement currentElement;
    private Dictionary<string, VisualElement> namedElements;

    #endregion

    #region Constructor and Initialization

    protected UI_Element()
    {
        elementID = Guid.NewGuid().ToString();
        containerStack = new Stack<VisualElement>();
        namedElements = new Dictionary<string, VisualElement>();

        CreateRootElement();
    }

    protected virtual void Initialize()
    {
        if (isInitialized) return;

        BuildVisualTreeFile();
        SetupInteractions();
        UpdateVisuals();

        isInitialized = true;
    }

    private void CreateRootElement()
    {
        rootVisualElement = new VisualElement
        {
            name = GetType().Name + "_" + elementID
        };

        rootVisualElement.style.flexGrow = 1f;
        rootVisualElement.style.flexDirection = FlexDirection.Column;

        containerStack.Clear();
        containerStack.Push(rootVisualElement);
        currentElement = rootVisualElement;
    }
    

    #endregion

    #region Abstract Methods
    
    protected abstract void BuildVisualTreeFile();
    
    protected abstract void SetupInteractions();
    
    protected abstract void UpdateVisuals();
    
    public abstract void SetButtonCallback(int index, Action callback);

    public abstract void SetClosedCallback(Action callback);

    #endregion

    #region Inner Builder - Container Methods
    
    protected ElementBuilder BeginRoot()
    {
        containerStack.Clear();
        containerStack.Push(rootVisualElement);
        currentElement = rootVisualElement;
        return new ElementBuilder(this, rootVisualElement);
    }
    
    protected ElementBuilder BeginContainer(string name = null, params string[] classNames)
    {
        var container = new VisualElement();
        ConfigureElement(container, name, classNames);

        GetCurrentParent().Add(container);
        containerStack.Push(container);
        currentElement = container;

        return new ElementBuilder(this, container);
    }
    
    protected ElementBuilder BeginRow(string name = null, params string[] classNames)
    {
        return BeginContainer(name, classNames).WithFlexDirection(FlexDirection.Row);
    }
    
    protected ElementBuilder BeginColumn(string name = null, params string[] classNames)
    {
        return BeginContainer(name, classNames).WithFlexDirection(FlexDirection.Column);
    }
    
    protected ElementBuilder EndContainer()
    {
        if (containerStack.Count > 1)
        {
            containerStack.Pop();
            currentElement = containerStack.Peek();
        }

        return new ElementBuilder(this, currentElement);
    }

    #endregion

    #region Inner Builder -> Adds elements to current container
    
    protected ElementBuilder AddLabel(string text, string name = null, params string[] classNames)
    {
        var label = new Label(text);
        ConfigureElement(label, name, classNames);

        GetCurrentParent().Add(label);
        currentElement = label;

        return new ElementBuilder(this, label);
    }
    
    protected ElementBuilder AddButton(string text, Action onClick = null, string name = null, params string[] classNames)
    {
        var button = new Button(onClick) { text = text };
        ConfigureElement(button, name, classNames);

        GetCurrentParent().Add(button);
        currentElement = button;

        return new ElementBuilder(this, button);
    }
    
    protected ElementBuilder AddElement(string name = null, params string[] classNames)
    {
        var child = new VisualElement();
        ConfigureElement(child, name, classNames);

        GetCurrentParent().Add(child);
        currentElement = child;

        return new ElementBuilder(this, child);
    }
    
    protected ElementBuilder AddChild(VisualElement child, string name = null)
    {
        if (!string.IsNullOrEmpty(name))
        {
            child.name = name;
            namedElements[name] = child;
        }

        GetCurrentParent().Add(child);
        currentElement = child;

        return new ElementBuilder(this, child);
    }
    
    protected ElementBuilder AddTextField(string label = null, string defaultValue = "", string name = null, params string[] classNames)
    {
        var textField = new TextField(label) { value = defaultValue };
        ConfigureElement(textField, name, classNames);

        GetCurrentParent().Add(textField);
        currentElement = textField;

        return new ElementBuilder(this, textField);
    }
    
    protected ElementBuilder AddToggle(string label = null, bool defaultValue = false, string name = null, params string[] classNames)
    {
        var toggle = new Toggle(label) { value = defaultValue };
        ConfigureElement(toggle, name, classNames);

        GetCurrentParent().Add(toggle);
        currentElement = toggle;

        return new ElementBuilder(this, toggle);
    }
    
    protected ElementBuilder AddImage(Texture2D texture = null, string name = null, params string[] classNames)
    {
        var image = new Image { image = texture };
        ConfigureElement(image, name, classNames);

        GetCurrentParent().Add(image);
        currentElement = image;

        return new ElementBuilder(this, image);
    }

    #endregion

    #region Element Lookup
    
    protected T GetElement<T>(string name) where T : VisualElement
    {
        if (namedElements.TryGetValue(name, out var element))
        {
            return element as T;
        }

        return rootVisualElement.Q<T>(name);
    }
    
    protected T Query<T>(string name = null) where T : VisualElement
    {
        return string.IsNullOrEmpty(name)
            ? rootVisualElement.Q<T>()
            : rootVisualElement.Q<T>(name);
    }
    
    protected List<T> QueryAll<T>(string className = null) where T : VisualElement
    {
        var list = new List<T>();
        rootVisualElement.Query<T>(null, className).ForEach(e => list.Add(e));
        return list;
    }

    #endregion

    #region Visibility & State

    public virtual VisualElement GetRootVisualElement() => rootVisualElement;

    public virtual void Show()
    {
        if (!isVisible)
        {
            isVisible = true;
            rootVisualElement.style.display = DisplayStyle.Flex;
            OnShow();
        }
    }

    public virtual void Hide()
    {
        if (isVisible)
        {
            isVisible = false;
            rootVisualElement.style.display = DisplayStyle.None;
            OnHide();
        }
    }

    public virtual void ToggleVisibility()
    {
        if (isVisible) Hide();
        else Show();
    }

    public virtual void SetEnabled(bool enabled)
    {
        isEnabled = enabled;
        rootVisualElement.SetEnabled(enabled);
    }

    public bool IsVisible => isVisible;
    public bool IsEnabled => isEnabled;

    #endregion

    #region Lifecycle Callbacks

    protected virtual void OnShow() => onShown?.Invoke();
    protected virtual void OnHide() => onHidden?.Invoke();
    protected virtual void OnDestroy() => onDestroyed?.Invoke();

    public void SetOnShown(Action callback) => onShown = callback;
    public void SetOnHidden(Action callback) => onHidden = callback;
    public void SetOnDestroyed(Action callback) => onDestroyed = callback;

    public virtual void Destroy()
    {
        OnDestroy();
        rootVisualElement?.RemoveFromHierarchy();
        rootVisualElement = null;
        namedElements.Clear();
        containerStack.Clear();
    }

    #endregion

    #region Convenience Methods
    
    public virtual void SetSize(float width, float height)
    {
        rootVisualElement.style.width = width;
        rootVisualElement.style.height = height;
    }
    
    public virtual void SetPosition(float left, float top)
    {
        rootVisualElement.style.position = Position.Absolute;
        rootVisualElement.style.left = left;
        rootVisualElement.style.top = top;
    }
    
    public virtual void AddUSSClass(string className) => rootVisualElement.AddToClassList(className);
    
    public virtual void RemoveUSSClass(string className) => rootVisualElement.RemoveFromClassList(className);

    #endregion

    #region Private Helpers

    private VisualElement GetCurrentParent()
    {
        return containerStack.Count > 0 ? containerStack.Peek() : rootVisualElement;
    }

    protected void ConfigureElement(VisualElement element, string name, string[] classNames)
    {
        if (!string.IsNullOrEmpty(name))
        {
            element.name = name;
            namedElements[name] = element;
        }

        if (classNames != null)
        {
            foreach (var className in classNames)
            {
                if (!string.IsNullOrEmpty(className))
                {
                    element.AddToClassList(className);
                }
            }
        }
    }
    
    internal Stack<VisualElement> GetContainerStack() => containerStack;
    
    internal void SetCurrentElement(VisualElement element) => currentElement = element;
    
    internal Dictionary<string, VisualElement> GetNamedElements() => namedElements;

    #endregion
}

#region Inner UXML Builder

public class ElementBuilder
{
    private readonly UI_Element owner;
    private readonly VisualElement element;

    internal ElementBuilder(UI_Element owner, VisualElement element)
    {
        this.owner = owner ?? throw new ArgumentNullException(nameof(owner));
        this.element = element ?? throw new ArgumentNullException(nameof(element));
    }

    #region Size & Position

    public ElementBuilder WithSize(float width, float height)
    {
        element.style.width = width;
        element.style.height = height;
        return this;
    }

    public ElementBuilder WithWidth(float width)
    {
        element.style.width = width;
        return this;
    }

    public ElementBuilder WithWidth(Length width)
    {
        element.style.width = width;
        return this;
    }

    public ElementBuilder WithHeight(float height)
    {
        element.style.height = height;
        return this;
    }

    public ElementBuilder WithHeight(Length height)
    {
        element.style.height = height;
        return this;
    }

    public ElementBuilder WithMinSize(float minWidth, float minHeight)
    {
        element.style.minWidth = minWidth;
        element.style.minHeight = minHeight;
        return this;
    }

    public ElementBuilder WithMaxSize(float maxWidth, float maxHeight)
    {
        element.style.maxWidth = maxWidth;
        element.style.maxHeight = maxHeight;
        return this;
    }

    public ElementBuilder WithPosition(float left, float top)
    {
        element.style.position = Position.Absolute;
        element.style.left = left;
        element.style.top = top;
        return this;
    }

    public ElementBuilder WithAbsolutePosition()
    {
        element.style.position = Position.Absolute;
        return this;
    }

    public ElementBuilder WithRelativePosition()
    {
        element.style.position = Position.Relative;
        return this;
    }

    #endregion

    #region Spacing

    public ElementBuilder WithPadding(float padding)
    {
        element.style.paddingTop = padding;
        element.style.paddingBottom = padding;
        element.style.paddingLeft = padding;
        element.style.paddingRight = padding;
        return this;
    }

    public ElementBuilder WithPadding(float vertical, float horizontal)
    {
        element.style.paddingTop = vertical;
        element.style.paddingBottom = vertical;
        element.style.paddingLeft = horizontal;
        element.style.paddingRight = horizontal;
        return this;
    }

    public ElementBuilder WithPadding(float top, float right, float bottom, float left)
    {
        element.style.paddingTop = top;
        element.style.paddingRight = right;
        element.style.paddingBottom = bottom;
        element.style.paddingLeft = left;
        return this;
    }

    public ElementBuilder WithMargin(float margin)
    {
        element.style.marginTop = margin;
        element.style.marginBottom = margin;
        element.style.marginLeft = margin;
        element.style.marginRight = margin;
        return this;
    }

    public ElementBuilder WithMargin(float vertical, float horizontal)
    {
        element.style.marginTop = vertical;
        element.style.marginBottom = vertical;
        element.style.marginLeft = horizontal;
        element.style.marginRight = horizontal;
        return this;
    }

    public ElementBuilder WithMargin(float top, float right, float bottom, float left)
    {
        element.style.marginTop = top;
        element.style.marginRight = right;
        element.style.marginBottom = bottom;
        element.style.marginLeft = left;
        return this;
    }

    public ElementBuilder WithMarginBottom(float margin)
    {
        element.style.marginBottom = margin;
        return this;
    }

    public ElementBuilder WithMarginTop(float margin)
    {
        element.style.marginTop = margin;
        return this;
    }

    public ElementBuilder WithMarginLeft(float margin)
    {
        element.style.marginLeft = margin;
        return this;
    }

    public ElementBuilder WithMarginRight(float margin)
    {
        element.style.marginRight = margin;
        return this;
    }

    #endregion

    #region Flexbox

    public ElementBuilder WithFlexDirection(FlexDirection direction)
    {
        element.style.flexDirection = direction;
        return this;
    }

    public ElementBuilder WithFlexGrow(float grow)
    {
        element.style.flexGrow = grow;
        return this;
    }

    public ElementBuilder WithFlexShrink(float shrink)
    {
        element.style.flexShrink = shrink;
        return this;
    }

    public ElementBuilder WithFlexBasis(Length basis)
    {
        element.style.flexBasis = basis;
        return this;
    }

    public ElementBuilder WithFlexWrap(Wrap wrap)
    {
        element.style.flexWrap = wrap;
        return this;
    }

    public ElementBuilder WithJustifyContent(Justify justify)
    {
        element.style.justifyContent = justify;
        return this;
    }

    public ElementBuilder WithAlignItems(Align align)
    {
        element.style.alignItems = align;
        return this;
    }

    public ElementBuilder WithAlignSelf(Align align)
    {
        element.style.alignSelf = align;
        return this;
    }

    public ElementBuilder WithAlignContent(Align align)
    {
        element.style.alignContent = align;
        return this;
    }

    public ElementBuilder Centered()
    {
        element.style.alignItems = Align.Center;
        element.style.justifyContent = Justify.Center;
        return this;
    }

    public ElementBuilder CenteredHorizontally()
    {
        element.style.alignItems = Align.Center;
        return this;
    }

    public ElementBuilder CenteredVertically()
    {
        element.style.justifyContent = Justify.Center;
        return this;
    }

    public ElementBuilder SpaceBetween()
    {
        element.style.justifyContent = Justify.SpaceBetween;
        return this;
    }

    #endregion

    #region Background & Colors

    public ElementBuilder WithBackgroundColor(Color color)
    {
        element.style.backgroundColor = new StyleColor(color);
        return this;
    }

    public ElementBuilder WithBackgroundColor(float r, float g, float b, float a = 1f)
    {
        element.style.backgroundColor = new StyleColor(new Color(r, g, b, a));
        return this;
    }

    public ElementBuilder WithBackgroundImage(Texture2D texture)
    {
        element.style.backgroundImage = new StyleBackground(texture);
        return this;
    }

    public ElementBuilder WithTextColor(Color color)
    {
        element.style.color = new StyleColor(color);
        return this;
    }

    public ElementBuilder WithOpacity(float opacity)
    {
        element.style.opacity = opacity;
        return this;
    }

    #endregion

    #region Border

    public ElementBuilder WithBorderRadius(float radius)
    {
        element.style.borderTopLeftRadius = radius;
        element.style.borderTopRightRadius = radius;
        element.style.borderBottomLeftRadius = radius;
        element.style.borderBottomRightRadius = radius;
        return this;
    }

    public ElementBuilder WithBorderRadius(float topLeft, float topRight, float bottomRight, float bottomLeft)
    {
        element.style.borderTopLeftRadius = topLeft;
        element.style.borderTopRightRadius = topRight;
        element.style.borderBottomRightRadius = bottomRight;
        element.style.borderBottomLeftRadius = bottomLeft;
        return this;
    }

    public ElementBuilder WithBorderWidth(float width)
    {
        element.style.borderTopWidth = width;
        element.style.borderRightWidth = width;
        element.style.borderBottomWidth = width;
        element.style.borderLeftWidth = width;
        return this;
    }

    public ElementBuilder WithBorderColor(Color color)
    {
        element.style.borderTopColor = color;
        element.style.borderRightColor = color;
        element.style.borderBottomColor = color;
        element.style.borderLeftColor = color;
        return this;
    }

    public ElementBuilder WithBorder(float width, Color color)
    {
        WithBorderWidth(width);
        WithBorderColor(color);
        return this;
    }

    #endregion

    #region Typography

    public ElementBuilder WithFontSize(float size)
    {
        element.style.fontSize = size;
        return this;
    }

    public ElementBuilder WithBold()
    {
        element.style.unityFontStyleAndWeight = FontStyle.Bold;
        return this;
    }

    public ElementBuilder WithItalic()
    {
        element.style.unityFontStyleAndWeight = FontStyle.Italic;
        return this;
    }

    public ElementBuilder WithBoldItalic()
    {
        element.style.unityFontStyleAndWeight = FontStyle.BoldAndItalic;
        return this;
    }

    public ElementBuilder WithNormalFont()
    {
        element.style.unityFontStyleAndWeight = FontStyle.Normal;
        return this;
    }

    public ElementBuilder WithTextAlign(TextAnchor align)
    {
        element.style.unityTextAlign = align;
        return this;
    }

    public ElementBuilder WithWhiteSpace(WhiteSpace whiteSpace)
    {
        element.style.whiteSpace = whiteSpace;
        return this;
    }

    public ElementBuilder WithLetterSpacing(float spacing)
    {
        element.style.letterSpacing = spacing;
        return this;
    }

    #endregion

    #region Overflow

    public ElementBuilder WithOverflow(Overflow overflow)
    {
        element.style.overflow = overflow;
        return this;
    }

    public ElementBuilder WithOverflowHidden()
    {
        element.style.overflow = Overflow.Hidden;
        return this;
    }

    public ElementBuilder WithOverflowVisible()
    {
        element.style.overflow = Overflow.Visible;
        return this;
    }

    #endregion

    #region Classes & Naming

    public ElementBuilder WithClass(string className)
    {
        element.AddToClassList(className);
        return this;
    }

    public ElementBuilder WithClasses(params string[] classNames)
    {
        foreach (var className in classNames)
        {
            element.AddToClassList(className);
        }

        return this;
    }

    public ElementBuilder WithName(string name)
    {
        element.name = name;
        owner.GetNamedElements()[name] = element;
        return this;
    }

    public ElementBuilder WithTooltip(string tooltip)
    {
        element.tooltip = tooltip;
        return this;
    }

    #endregion

    #region Events

    public ElementBuilder OnClick(Action callback)
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

    public ElementBuilder OnMouseDown(EventCallback<MouseDownEvent> callback)
    {
        element.RegisterCallback<MouseDownEvent>(callback);
        return this;
    }

    public ElementBuilder OnMouseUp(EventCallback<MouseUpEvent> callback)
    {
        element.RegisterCallback<MouseUpEvent>(callback);
        return this;
    }

    public ElementBuilder OnMouseMove(EventCallback<MouseMoveEvent> callback)
    {
        element.RegisterCallback<MouseMoveEvent>(callback);
        return this;
    }

    public ElementBuilder OnMouseEnter(EventCallback<MouseEnterEvent> callback)
    {
        element.RegisterCallback<MouseEnterEvent>(callback);
        return this;
    }

    public ElementBuilder OnMouseLeave(EventCallback<MouseLeaveEvent> callback)
    {
        element.RegisterCallback<MouseLeaveEvent>(callback);
        return this;
    }

    public ElementBuilder OnValueChanged<T>(EventCallback<ChangeEvent<T>> callback)
    {
        element.RegisterCallback<ChangeEvent<T>>(callback);
        return this;
    }

    #endregion

    #region Container Navigation
    
    public ElementBuilder BeginContainer(string name = null, params string[] classNames)
    {
        var container = new VisualElement();
        
        if (!string.IsNullOrEmpty(name))
        {
            container.name = name;
            owner.GetNamedElements()[name] = container;
        }
        
        if (classNames != null)
        {
            foreach (var className in classNames)
            {
                if (!string.IsNullOrEmpty(className))
                {
                    container.AddToClassList(className);
                }
            }
        }
        
        var stack = owner.GetContainerStack();
        var parent = stack.Count > 0 ? stack.Peek() : element;
        parent.Add(container);
        
        stack.Push(container);
        owner.SetCurrentElement(container);

        return new ElementBuilder(owner, container);
    }
    
    public ElementBuilder BeginRow(string name = null, params string[] classNames)
    {
        return BeginContainer(name, classNames).WithFlexDirection(FlexDirection.Row);
    }
    
    public ElementBuilder BeginColumn(string name = null, params string[] classNames)
    {
        return BeginContainer(name, classNames).WithFlexDirection(FlexDirection.Column);
    }
    
    public ElementBuilder EndContainer()
    {
        var stack = owner.GetContainerStack();

        if (stack.Count > 1)
        {
            stack.Pop();
            
            var parent = stack.Peek();
            owner.SetCurrentElement(parent);
            return new ElementBuilder(owner, parent);
        }
        
        return this;
    }
    
    public ElementBuilder AddLabel(string text, string name = null, params string[] classNames)
    {
        var label = new Label(text);

        if (!string.IsNullOrEmpty(name))
        {
            label.name = name;
            owner.GetNamedElements()[name] = label;
        }

        foreach (var className in classNames)
        {
            if (!string.IsNullOrEmpty(className))
            {
                label.AddToClassList(className);
            }
        }

        GetCurrentParent().Add(label);
        owner.SetCurrentElement(label);

        return new ElementBuilder(owner, label);
    }
    
    public ElementBuilder AddButton(string text, Action onClick = null, string name = null, params string[] classNames)
    {
        var button = new Button(onClick) { text = text };

        if (!string.IsNullOrEmpty(name))
        {
            button.name = name;
            owner.GetNamedElements()[name] = button;
        }

        foreach (var className in classNames)
        {
            if (!string.IsNullOrEmpty(className))
            {
                button.AddToClassList(className);
            }
        }

        GetCurrentParent().Add(button);
        owner.SetCurrentElement(button);

        return new ElementBuilder(owner, button);
    }
    
    public ElementBuilder AddElement(string name = null, params string[] classNames)
    {
        var child = new VisualElement();

        if (!string.IsNullOrEmpty(name))
        {
            child.name = name;
            owner.GetNamedElements()[name] = child;
        }

        foreach (var className in classNames)
        {
            if (!string.IsNullOrEmpty(className))
            {
                child.AddToClassList(className);
            }
        }

        GetCurrentParent().Add(child);
        owner.SetCurrentElement(child);

        return new ElementBuilder(owner, child);
    }
    
    public ElementBuilder AddChild(VisualElement child)
    {
        GetCurrentParent().Add(child);
        owner.SetCurrentElement(child);
        return new ElementBuilder(owner, child);
    }

    #endregion

    #region Element Access
    
    public VisualElement GetElement() => element;
    
    public T GetElement<T>() where T : VisualElement => element as T;
    
    public ElementBuilder StoreAs(out VisualElement reference)
    {
        reference = element;
        return this;
    }
    
    public ElementBuilder StoreAs<T>(out T reference) where T : VisualElement
    {
        reference = element as T;
        return this;
    }

    #endregion

    #region Private Helpers

    private VisualElement GetCurrentParent()
    {
        var stack = owner.GetContainerStack();
        return stack.Count > 0 ? stack.Peek() : element;
    }

    #endregion
}

#endregion