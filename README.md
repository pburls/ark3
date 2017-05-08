Ark3
===
[![Build status](https://ci.appveyor.com/api/projects/status/7i3qkwtldknb1xlv/branch/master?svg=true)](https://ci.appveyor.com/project/pburls/ark3/branch/master)
[![NuGet](https://img.shields.io/nuget/v/Ark3.svg)](https://www.nuget.org/packages/Ark3/)

A simple Messaging framework that can help improve cohesion and reduce coupling in single threaded applications.

# Usage
## Event Aggregator
By creating a single Event Aggregator instance for the lifetime of the application, the components of an application can Subscribe and Publish Events. This can help reduce unnecessary coupling between components and improve testability and cohesion.
### Event Messages
You should create different event message objects to represent different events that take place in your application's lifetime.  
All event message objects sent on the Event Aggregator need to implement the `Ark3.Event.IEvent` interface.  
Event message objects should try carry additional properties to describe the event itself.
For example:
```
namespace Dewey.Manifest.Events
{
    public class ManifestFileFound : IEvent
    {
        public string FileName { get; private set; }

        public ManifestFileFound(string fileName)
        {
            FileName = fileName;
        }
    }
}
```
### Publishing an Event
An event can be publish to all subscribed components using the EventAggregator's `PublishEvent` action method.  
For example:
```
_eventAggregator.PublishEvent(new ManifestFilesFound(fileReader.FileName));
```
### Subscribing to Events of a specific Type
A component can subscribe with an instance of the Event Aggregator to be notified every time an event of a specific type is published on the Event Aggregator.  
The component object will need to implement the `IEventHandler<TEvent>` interface where `TEvent` is the type of the event that component is interested in listening too.  
Finally the component must register itself with the applications Event Aggregator instance as a listener of the specific event using the `Subscribe<TEvent>` action method.
For example:
```
_eventAggregator.Subscribe<GenerateGraphStarted>(this);
_eventAggregator.Subscribe<GenerateGraphResult>(this);
```
A simpler alternative for components that listen to multiple event types is to use the `SubscribeAll` action method.
```
_eventAggregator.SubscribeAll(this);
```
### Unsubscribing from Events
When a component is no longer interested in being notified when events of a specific type are being published, the `Unsubscribe<TEvent>` and `UnsubscribeAll` action methods can be used on the Event Aggregator.

## Command Processor
Publish Commands and Register Command Handlers to a single Command Processor.
