using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ButtonClickTracker : ObservableBase
{
    public const string MSG_CLICK = "CLICK";

    private HashSet<string> trackedIds = new HashSet<string>();

    /// <summary>
    /// Tracks all buttons in a UI_Element (menu, popup, etc).
    /// </summary>
    public void Track(UI_Element element, string groupId)
    {
        var buttons = element.GetRootVisualElement().Query<Button>().ToList();
        
        for (int i = 0; i < buttons.Count; i++)
        {
            var button = buttons[i];
            if (button.text == "×") continue; // Skip close buttons
            
            string id = string.IsNullOrEmpty(button.text) 
                ? $"{groupId}-{i}" 
                : $"{groupId}-{Sanitize(button.text)}";
            
            Track(button, id);
        }
    }
    
    public void Track(Button button, string buttonId)
    {
        if (button == null || trackedIds.Contains(buttonId)) return;
        
        trackedIds.Add(buttonId);
        button.clicked += () => Notify($"{MSG_CLICK}:{buttonId}");
    }
    
    public string[] GetTrackedIds() => new List<string>(trackedIds).ToArray();

    private string Sanitize(string text) => text.ToLowerInvariant().Replace(" ", "-");
}
