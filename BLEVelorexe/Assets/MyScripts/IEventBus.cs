using System;


    public interface IEventBus
    {
        void Subscribe<T>(Action<T> listener);
        void UnSubscribe<T>(Action<T> listener);
        void Publish<T>(T eventData);
    }
