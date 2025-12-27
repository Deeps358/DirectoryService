using DirectoryServices.Application.Abstractions;
using DirectoryServices.Application.Database;
using DirectoryServices.Entities;
using Microsoft.Extensions.Logging;
using Shared.ResultPattern;

namespace DirectoryServices.Application.Departaments.Commands.SoftDelete
{
    public class SoftDeleteHandler : ICommandHandler<Guid, SoftDeleteCommand>
    {
        private readonly ITransactionManager _transactionManager;
        private readonly IDepartamentsRepository _departamentsRepository;
        private readonly ILogger<SoftDeleteHandler> _logger;

        public SoftDeleteHandler(
                ITransactionManager transactionManager,
                IDepartamentsRepository departamentsRepository,
                ILogger<SoftDeleteHandler> logger)
        {
            _transactionManager = transactionManager;
            _departamentsRepository = departamentsRepository;
            _logger = logger;
        }

        public async Task<Result<Guid>> Handle(SoftDeleteCommand command, CancellationToken cancellationToken)
        {
            // открываем транзакцию
            Result<ITransactionScope> transactionScopeResult = await _transactionManager.BeginTransactionAsync(cancellationToken); // открытие транзакции
            if (transactionScopeResult.IsFailure)
            {
                return transactionScopeResult.Error;
            }

            using ITransactionScope transactionScope = transactionScopeResult.Value;

            // сначала проверить что id верный и залочить его
            Result<Departament> getDepResult = await _departamentsRepository.GetByIdWithLockAsync(command.DepId, cancellationToken);
            if(getDepResult.IsFailure)
            {
                transactionScope.Rollback();
                return getDepResult.Error;
            }

            // заблокирую ещё всех детей депа чтоб их не трогали
            CSharpFunctionalExtensions.UnitResult<Error> getChildrensResult = await _departamentsRepository.GetChildDepsWithLockAsync(getDepResult.Value.Path, cancellationToken);
            if(getChildrensResult.IsFailure)
            {
                transactionScope.Rollback();
                return getChildrensResult.Error;
            }

            // теперь надо поменять Path депа и часть всех детей на [DELETED]

            return command.DepId;
        }
    }
}