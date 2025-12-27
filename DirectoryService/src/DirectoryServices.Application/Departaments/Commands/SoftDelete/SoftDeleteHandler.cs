using DirectoryServices.Application.Abstractions;
using DirectoryServices.Application.Database;
using DirectoryServices.Entities;
using DirectoryServices.Entities.ValueObjects.Departaments;
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

            Departament dep = getDepResult.Value;

            // заблокирую ещё всех детей депа чтоб их не трогали
            CSharpFunctionalExtensions.UnitResult<Error> getChildrensResult = await _departamentsRepository.GetChildDepsWithLockAsync(dep.Path, cancellationToken);
            if(getChildrensResult.IsFailure)
            {
                transactionScope.Rollback();
                return getChildrensResult.Error;
            }

            // теперь надо поменять Path депа и часть всех детей на [DELETED]
            string deleteMark = "__DELETED__";
            var softDeleteResult = await _departamentsRepository.SoftDeleteWithChildrensAsync(
                dep.Path,
                DepPath.GetCurrent(dep.Path.Value + deleteMark),
                cancellationToken);

            if(softDeleteResult.IsFailure)
            {
                transactionScope.Rollback();
                return softDeleteResult.Error;
            }

            CSharpFunctionalExtensions.UnitResult<Error> saveResult = await _transactionManager.SaveChangesAsync(cancellationToken);
            if(saveResult.IsFailure)
            {
                transactionScope.Rollback();
                return saveResult.Error;
            }

            var commitedResult = transactionScope.Commit();
            if (commitedResult.IsFailure)
            {
                return commitedResult.Error;
            }

            _logger.LogInformation("Произошёл soft Delete подразделения с id = {command.DepId}", command.DepId);

            return command.DepId;
        }
    }
}