using System;
using System.Collections.Generic;

public class EventBus : IEventBus
{

    public static EventBus Instance { get; } = new EventBus();
    private EventBus() { } // dışarıdan oluşturulmasın diye private ctor

    private Dictionary<Type, Delegate> _eventTable = new();


    public void Subscribe<T>(Action<T> listener)
    {
        if (!_eventTable.ContainsKey(typeof(T)))
            _eventTable[typeof(T)] = null;

        _eventTable[typeof(T)] = (Action<T>)_eventTable[typeof(T)] + listener;
    }

    public void UnSubscribe<T>(Action<T> listener)
    {
        if (_eventTable.ContainsKey(typeof(T)))
            _eventTable[typeof(T)] = (Action<T>)_eventTable[typeof(T)] - listener;
    }

    public void Publish<T>(T eventData)
    {
        if (_eventTable.TryGetValue(typeof(T), out var action))
        {
            ((Action<T>)action)?.Invoke(eventData);
        }
    }


}
