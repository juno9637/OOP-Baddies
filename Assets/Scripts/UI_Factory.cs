using System;
using UnityEngine;

public enum UI_Type
{
    Slider,
    Menu,
    Button,
    Popup
}

public class UI_Factory : MonoBehaviour
{
    public UI_Factory()
    {
        
    }
    
    public UI_Element CreateSlider(string title,
        float defaultValue = .75f,
        float width = 400f,
        Action<float> valueChangedCallback = null,
        float trackHeight = 8f,
        float thumbSize = 20f)
    {
        return new UI_SoundSlider(title, defaultValue, width, valueChangedCallback, trackHeight, thumbSize);
    }

    public UI_Element CreateMenu(string title,
        int buttonCount = 3,
        string[] label = null,
        float width = 250f,
        float buttonHeight = 26f,
        float buttonSpacing = 6f)
    {
        return new UI_SimpleMenu(title, buttonCount, label, width, buttonHeight, buttonSpacing);
    }

    public UI_Element CreatePopup(string title,
        string message,
        string iconSymbol,
        Color? iconColor,
        string[] buttonLabels,
        float width = 380f,
        float buttonWidth = 75f)
    {
        return new UI_SimplePopUp(title, message, iconSymbol, iconColor, buttonLabels, width, buttonWidth);
    }
}
