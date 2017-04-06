using System;
using System.Collections.Generic;
using System.Reflection;

namespace Ark3
{
    public class CommandProcessor : ICommandProcessor
    {
        readonly Dictionary<Type, object> _commandHandlerFactories;
        readonly TypeInfo _commandHandlerFactoryGenericType = typeof(ICommandHandlerFactory<>).GetTypeInfo();

        public CommandProcessor()
        {
            _commandHandlerFactories = new Dictionary<Type, object>();
        }

        public void RegisterHandlerFactory<TCommand, TCommandHandlerFactory>(TCommandHandlerFactory factory)
            where TCommand : ICommand
            where TCommandHandlerFactory : ICommandHandlerFactory<TCommand>
        {
            Type commandType = typeof(TCommand);

            _commandHandlerFactories.Add(commandType, factory);
        }

        public object Execute(ICommand command)
        {
            Type commandType = command.GetType();
            object commandHandlerFactory;
            object commandHandler = null;

            if (_commandHandlerFactories.TryGetValue(commandType, out commandHandlerFactory))
            {
                TypeInfo commandHandlerFactoryType = _commandHandlerFactoryGenericType.MakeGenericType(commandType).GetTypeInfo();

                MethodInfo createMethod = commandHandlerFactoryType.GetDeclaredMethod("CreateHandler");
                commandHandler = createMethod.Invoke(commandHandlerFactory, null);
                
                MethodInfo executeMethod = commandHandler.GetType().GetTypeInfo().GetDeclaredMethod("Execute");
                executeMethod.Invoke(commandHandler, new[] { command });
            }

            return commandHandler;
        }
    }
}
