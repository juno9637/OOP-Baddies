using System.Collections.Generic;
using UnityEngine;

public abstract class ObservableBase : IObservable
{
    private List<IObserver> observers = new List<IObserver>();

    public void Attach(IObserver observer)
    {
        if (observer != null && !observers.Contains(observer))
        {
            observers.Add(observer);
        }
    }

    public void Detach(IObserver observer)
    {
        if (observer != null)
        {
            observers.Remove(observer);
        }
    }

    public void Notify(string msg)
    {
        foreach (var observer in observers.ToArray())
        {
            observer.OnNotify(msg);
        }
    }
    
    public int ObserverCount => observers.Count;
    
    public void ClearObservers()
    {
        observers.Clear();
    }
}
