using System;
using System.Collections.Generic;

namespace Defong.Events
{
    public class EventAggregator
    {
        private Dictionary<Type, object> _eventWrappersDict = new Dictionary<Type, object>();

        public void Add<T>(Action<T> action) where T : BaseEvent
        {
            
            var wrapper = GetWrapper<T>();
            wrapper.AddListener(action);
        }
        public void Remove<T>(Action<T> action) where T : BaseEvent
        {
            var wrapper = GetWrapper<T>();
            wrapper?.RemoveListener(action);
        }
        public void SendEvent<T>(T eventData) where T : BaseEvent
        {
            var wrapper = GetWrapper<T>();
            wrapper?.Invoke(eventData);
        }
        
        public void SendEvent<T>() where T : BaseEvent
        {
            var wrapper = GetWrapper<T>();
            wrapper?.Invoke(default(T));
        }
        
        private EventWrapper<T> GetWrapper<T>() where T : BaseEvent
        {
            var type = typeof(T);
            if (!_eventWrappersDict.ContainsKey(type))
            {
                var wrapper = new EventWrapper<T>();
                _eventWrappersDict.Add(type, wrapper);
                return wrapper;
            }
            return _eventWrappersDict[type] as EventWrapper<T>;
        }
    }
}
