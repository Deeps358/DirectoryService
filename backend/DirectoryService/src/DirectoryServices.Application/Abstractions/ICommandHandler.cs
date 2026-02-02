using CSharpFunctionalExtensions;
using Shared.ResultPattern;

namespace DirectoryServices.Application.Abstractions
{
    public interface ICommand;

    public interface ICommandHandler<TResponse, in TCommand>
        where TCommand : ICommand
    {
        Task<Shared.ResultPattern.Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken);
    }

    public interface ICommandHandler<in TCommand>
        where TCommand : ICommand
    {
        Task<UnitResult<Error>> Handle(TCommand command, CancellationToken cancellationToken);
    }
}
