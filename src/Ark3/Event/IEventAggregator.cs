using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ark3.Event
{
    public interface IEventAggregator
    {
        void Subscribe<TEvent>(IEventHandler<TEvent> eventHandler) where TEvent : IEvent;
        void SubscribeAll(object eventHandler);
        void Unsubscribe<TEvent>(IEventHandler<TEvent> eventHandler) where TEvent : IEvent;
        void UnsubscribeAll(object eventHandler);
        void PublishEvent<TEvent>(TEvent @event) where TEvent : IEvent;
    }
}
