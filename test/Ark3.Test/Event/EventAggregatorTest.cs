using Ark3.Event;
using Moq;
using Xunit;

namespace Ark3.Test.Event
{
    public class EventAggregatorTest
    {
        EventAggregator eventAggregator;

        public EventAggregatorTest()
        {
            eventAggregator = new EventAggregator();
        }

        [Fact]
        public void Test_PublishEvent_Invokes_Event_Handlers_Subscribed_For_The_Events_Type()
        {
            //Given
            var @event = new TestEventA();
            var mockTestEventHandler = new Mock<IEventHandler<TestEventA>>();
            eventAggregator.Subscribe(mockTestEventHandler.Object);

            //When
            eventAggregator.PublishEvent(@event);

            //Then
            mockTestEventHandler.Verify(x => x.Handle(@event), Times.Once);
        }

        [Fact]
        public void Test_PublishEvent_Invokes_Event_Handlers_Subscribed_For_The_Events_Inherited_Types()
        {
            //Given
            var @event = new TestEventA();
            var mockBaseEventHandler = new Mock<IEventHandler<TestEventBase>>();
            var mockIEventHandler = new Mock<IEventHandler<IEvent>>();
            eventAggregator.Subscribe(mockBaseEventHandler.Object);
            eventAggregator.Subscribe(mockIEventHandler.Object);

            //When
            eventAggregator.PublishEvent(@event);

            //Then
            mockBaseEventHandler.Verify(x => x.Handle(@event), Times.Once);
            mockIEventHandler.Verify(x => x.Handle(@event), Times.Once);
        }

        [Fact]
        public void Test_PublishEvent_Only_Invokes_Event_Handlers_Subscribed_For_The_Events_Inherited_Types()
        {
            //Given
            var @event = new TestEventB();
            var mockBaseEventHandler = new Mock<IEventHandler<TestEventBase>>();
            var mockIEventHandler = new Mock<IEventHandler<IEvent>>();
            eventAggregator.Subscribe(mockBaseEventHandler.Object);
            eventAggregator.Subscribe(mockIEventHandler.Object);

            //When
            eventAggregator.PublishEvent(@event);

            //Then
            mockBaseEventHandler.Verify(x => x.Handle(It.IsAny<TestEventBase>()), Times.Never);
            mockIEventHandler.Verify(x => x.Handle(@event), Times.Once);
        }

        [Fact]
        public void Test_PublishEvent_Invokes_Event_Handlers_Subscribed_For_The_Events_Type_Using_SubscribeAll()
        {
            //Given
            var @event = new TestEventA();
            var mockTestEventHandler = new Mock<IEventHandler<TestEventA>>();
            eventAggregator.SubscribeAll(mockTestEventHandler.Object);

            //When
            eventAggregator.PublishEvent(@event);

            //Then
            mockTestEventHandler.Verify(x => x.Handle(@event), Times.Once);
        }

        [Fact]
        public void Test_PublishEvent_Invokes_Event_Handlers_Subscribed_For_The_Events_Type_When_Publishing_Event_With_Base_Type()
        {
            //Given
            var @event = new TestEventA();
            var eventBase = @event as TestEventBase;
            var mockTestEventHandler = new Mock<IEventHandler<TestEventA>>();
            eventAggregator.SubscribeAll(mockTestEventHandler.Object);

            //When
            eventAggregator.PublishEvent(eventBase);

            //Then
            mockTestEventHandler.Verify(x => x.Handle(@event), Times.Once);
        }

        [Fact]
        public void Test_PublishEvent_Does_Not_Invoke_EventHandler_After_Unsubscribing_For_Event_Type()
        {
            //Given
            var @event = new TestEventA();
            var mockTestEventHandler = new Mock<IEventHandler<TestEventA>>();
            eventAggregator.Subscribe(mockTestEventHandler.Object);

            //When
            eventAggregator.PublishEvent(@event);
            eventAggregator.Unsubscribe(mockTestEventHandler.Object);
            eventAggregator.PublishEvent(@event);

            //Then
            mockTestEventHandler.Verify(x => x.Handle(@event), Times.Once);
        }

        [Fact]
        public void Test_PublishEvent_Does_Not_Invoke_EventHandler_After_Unsubscribing_For_All_Event_Types()
        {
            //Given
            var @event = new TestEventA();
            var mockTestEventHandler = new Mock<IEventHandler<TestEventA>>();
            eventAggregator.Subscribe(mockTestEventHandler.Object);

            //When
            eventAggregator.PublishEvent(@event);
            eventAggregator.UnsubscribeAll(mockTestEventHandler.Object);
            eventAggregator.PublishEvent(@event);

            //Then
            mockTestEventHandler.Verify(x => x.Handle(@event), Times.Once);
        }
    }
}
