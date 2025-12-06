interface IObservable
{
    void Attach(IObserver observer);
    void Notify(string message);
}