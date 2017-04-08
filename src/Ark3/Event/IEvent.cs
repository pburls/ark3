namespace Ark3.Event
{
    public interface IEvent
    {
    }

    public interface IEventHandler<in TEvent> where TEvent : IEvent
    {
        void Handle(TEvent @event);
    }
}
