using Ark3.Command;
using Moq;
using Xunit;

namespace Ark3.Test.Command
{
    public class CommandProcessorTest
    {
        CommandProcessor _commandProcessor;

        public CommandProcessorTest()
        {
            _commandProcessor = new CommandProcessor();
        }

        [Fact]
        public void Test_ExecuteCommand_Invokes_CommandHandlerFactory_Create_For_CommandType()
        {
            //Given
            var command = new CommandA();
            var mockCommandHandlerFactory = new Mock<ICommandHandlerFactory<CommandA>>();
            _commandProcessor.RegisterHandlerFactory(mockCommandHandlerFactory.Object);

            //When
            var result = _commandProcessor.Execute(command);

            //Then
            mockCommandHandlerFactory.Verify(x => x.CreateHandler(), Times.Once);
        }

        [Fact]
        public void Test_ExecuteCommand_Invokes_CommandHandler_Execute_with_Command()
        {
            //Given
            var command = new CommandA();
            var mockCommandHandlerFactory = new Mock<ICommandHandlerFactory<CommandA>>();
            var mockCommandHandler = new Mock<ICommandHandler<CommandA>>();
            mockCommandHandlerFactory.Setup(x => x.CreateHandler()).Returns(mockCommandHandler.Object);

            _commandProcessor.RegisterHandlerFactory(mockCommandHandlerFactory.Object);

            //When
            var result = _commandProcessor.Execute(command);

            //Then
            mockCommandHandler.Verify(x => x.Execute(command), Times.Once);
        }

        [Fact]
        public void Test_ExecuteCommand_Invokes_AmbiguousCommandHandler_Execute_with_Command()
        {
            //Given
            var command = new CommandA();
            var mockCommandHandlerA = new Mock<ICommandHandler<CommandA>>();
            var mockCommandHandlerB = new Mock<ICommandHandler<CommandB>>();

            var ambiguousCommandHandlerFactory = new AmbiguousCommandHandlerFactory(mockCommandHandlerA.Object, mockCommandHandlerB.Object);

            _commandProcessor.RegisterHandlerFactory<CommandA>(ambiguousCommandHandlerFactory);
            _commandProcessor.RegisterHandlerFactory<CommandB>(ambiguousCommandHandlerFactory);

            //When
            var result = _commandProcessor.Execute(command);

            //Then
            mockCommandHandlerA.Verify(x => x.Execute(command), Times.Once);
            mockCommandHandlerB.Verify(x => x.Execute(It.IsAny<CommandB>()), Times.Never);
        }
    }
}
