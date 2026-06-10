// ReSharper disable UnusedTypeParameter
namespace Slicent.Application.Commands;

public interface ICommand;

public interface ICommand<out TCommandResult> 
    : ICommand;