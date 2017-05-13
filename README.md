Ark3
===
[![Build status](https://ci.appveyor.com/api/projects/status/7i3qkwtldknb1xlv/branch/master?svg=true)](https://ci.appveyor.com/project/pburls/ark3/branch/master)
[![NuGet](https://img.shields.io/nuget/v/Ark3.svg)](https://www.nuget.org/packages/Ark3/)

A simple Messaging framework that can help improve cohesion and reduce coupling in single threaded applications.

# Usage
## Event Aggregator
By creating a single Event Aggregator instance for the lifetime of an application, the different components of the application can then Subscribe and Publish Events to each other. This can help reduce unnecessary coupling between components and improve testability and cohesion.
### Event Messages
You should create different event message objects to represent different events that take place in your application's lifetime.  
All event message objects sent on the Event Aggregator need to implement the `Ark3.Event.IEvent` interface.  
Event message objects should try carry additional properties to describe the event itself.
For example:
```
public class ManifestFileFound : IEvent
{
    public string FileName { get; private set; }

    public ManifestFileFound(string fileName)
    {
        FileName = fileName;
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
Components can also subscribe to inherited types and still be notified if derived event types are published. This offers a intuitive OO approach to event messaging in a system.  
The component object will need to implement the `Ark3.Event.IEventHandler<TEvent>` interface where `TEvent` is the type of the event that component is interested in listening for.  
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
Commands are a different type of message that help improve cohesion and reduce coupling in a system. The fundamental difference is that commands can only be handled by one command handler. In other words, only one command handler can be register with the Command Processor for a specific type of command.
Only one instance of the Command Processor should be created for the lifetime of an application.
Any command handlers should be registered with the command processor when the application starts.
### Command Messages
The different commands that your application can process should be represented as Command objects.  
A command object must implement the `Ark3.Command.ICommand` interface.  
The command message should try capture all the information needed to process the command. For example:
```
public class DeployCommand : ICommand
{
    public string ComponentName { get; private set; }

    public bool DeployDependencies { get; private set; }

    DeployCommand(string componentName, bool deployDependencies)
    {
        ComponentName = componentName;
        DeployDependencies = deployDependencies;
    }
}
```
### Executing a Command
If any component in an application needs a command executed, it will need to create a command message object with the necessary details and invoke the `Execute(ICommand command)` action on the Command Processor instance of the application.
For example:
```
commandProcessor.Execute(new DeployCommand("exampleName", true));
```
### Command Handler
A component that is responsible for the execution of specific commands must implement the `Ark3.Command.ICommandHandler<in TCommand>` interface, where `TCommand` is the type of command message the handler is responsible for executing.
When processing a command, a command handler should publish events describing the outcome of the command execution process.
### Command Handler Factory
Because each command handler may require different dependencies to complete the execution of the command, the Command Processor requires factories to create instances of the command handlers.
A Command Handler Factory should implement the `Ark3.Command.ICommandHandlerFactory<in TCommand>` interface.  
The example Command Handler Factory below shows how Command Handler instances are created with all its required dependencies using an Inversion of Control (IoC) container.
```
class DeployCommandHandlerFactory : ICommandHandlerFactory<DeployCommand>
{
    readonly Container _container;

    public DeployCommandHandlerFactory(Container container)
    {
        _container = container;
    }

    public ICommandHandler<DeployCommand> CreateHandler()
    {
        return _container.GetInstance<DeployCommandHandler>();
    }
}
```
### Registering a Command Handler
Each command handler must be registered with the Command Processor so that it knows which command handler to invoke when a command is given to it to be executed.
This is done by using the `RegisterHandlerFactory<TCommand>(ICommandHandlerFactory<TCommand> factory)` action on the Command Processor instance of the application.  
This is usually done at the start up of the application.
