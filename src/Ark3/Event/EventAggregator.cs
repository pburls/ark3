using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ark3.Event
{
    public class EventAggregator : IEventAggregator
    {
        private Dictionary<Type, EventHandlerCollection> _eventHandlers = new Dictionary<Type, EventHandlerCollection>();
        private Type _eventHandlerInterfaceType = typeof(IEventHandler<>);

        public void Subscribe<TEvent>(IEventHandler<TEvent> eventHandler) where TEvent : IEvent
        {
            Type eventHandlerInterfaceType = typeof(IEventHandler<TEvent>);
            Subscribe(eventHandlerInterfaceType, eventHandler);
        }

        public void SubscribeAll(object eventHandler)
        {
            Type eventHandlerType = eventHandler.GetType();
            var interfaces = eventHandlerType.GetTypeInfo().ImplementedInterfaces;
            var eventHandlerInterfaceTypes = interfaces.Where(t => t.GetTypeInfo().IsGenericType && t.GetGenericTypeDefinition() == _eventHandlerInterfaceType);

            foreach (var eventHandlerInterfaceType in eventHandlerInterfaceTypes)
            {
                Subscribe(eventHandlerInterfaceType, eventHandler);
            }
        }

        protected void Subscribe(Type eventHandlerInterfaceType, object eventHandler)
        {
            var eventType = eventHandlerInterfaceType.GetTypeInfo().GenericTypeArguments[0];
            EventHandlerCollection eventHandlerCollection = null;

            if (!_eventHandlers.TryGetValue(eventType, out eventHandlerCollection))
            {
                eventHandlerCollection = new EventHandlerCollection(eventHandlerInterfaceType);
                _eventHandlers.Add(eventType, eventHandlerCollection);
            }

            eventHandlerCollection.Add(eventHandler);
        }

        public void Unsubscribe<TEvent>(IEventHandler<TEvent> eventHandler) where TEvent : IEvent
        {
            Type eventHandlerInterfaceType = typeof(IEventHandler<TEvent>);
            Unsubscribe(eventHandlerInterfaceType, eventHandler);
        }

        public void UnsubscribeAll(object eventHandler)
        {
            Type eventHandlerType = eventHandler.GetType();
            var interfaces = eventHandlerType.GetTypeInfo().ImplementedInterfaces;
            var eventHandlerInterfaceTypes = interfaces.Where(t => t.GetTypeInfo().IsGenericType && t.GetGenericTypeDefinition() == _eventHandlerInterfaceType);

            foreach (var eventHandlerInterfaceType in eventHandlerInterfaceTypes)
            {
                Unsubscribe(eventHandlerInterfaceType, eventHandler);
            }
        }

        protected void Unsubscribe(Type eventHandlerInterfaceType, object eventHandler)
        {
            var eventType = eventHandlerInterfaceType.GetTypeInfo().GenericTypeArguments[0];
            EventHandlerCollection eventHandlerCollection = null;

            if (_eventHandlers.TryGetValue(eventType, out eventHandlerCollection))
            {
                eventHandlerCollection.Remove(eventHandler);
            }
        }

        public void PublishEvent<TEvent>(TEvent @event) where TEvent : IEvent
        {
            TypeInfo eventType = @event.GetType().GetTypeInfo();

            foreach (var type in _eventHandlers.Keys)
            {
                if (type.GetTypeInfo().IsAssignableFrom(eventType))
                {
                    var eventHandlerCollection = _eventHandlers[type];
                    eventHandlerCollection.ExecuteAll(@event);
                }
            }
        }

        class EventHandlerCollection
        {
            private List<object> _eventHandlers = new List<object>();
            private MethodInfo _handleMethod;

            public Type EventHandlerType { get; private set; }

            public EventHandlerCollection(Type eventHandlerType)
            {
                EventHandlerType = eventHandlerType;
                _handleMethod = eventHandlerType.GetTypeInfo().GetDeclaredMethod("Handle");
            }

            public void Add(object eventHandler)
            {
                _eventHandlers.Add(eventHandler);
            }

            public void Remove(object eventHandler)
            {
                _eventHandlers.Remove(eventHandler);
            }

            public void ExecuteAll(object @event)
            {
                foreach (var eventHandler in _eventHandlers)
                {
                    _handleMethod.Invoke(eventHandler, new[] { @event });
                }
            }
        }
    }
}
