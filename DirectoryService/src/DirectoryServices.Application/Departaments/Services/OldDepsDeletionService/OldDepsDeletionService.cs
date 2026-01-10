using DirectoryServices.Application.Database;
using DirectoryServices.Entities;
using DirectoryServices.Entities.ValueObjects.Departaments;
using Microsoft.Extensions.Logging;
using Shared.ResultPattern;

namespace DirectoryServices.Application.Departaments.Services.OldDepsDeletionService
{
    public class OldDepsDeletionService : IDeletionService
    {
        private readonly ITransactionManager _transactionManager;
        private readonly IDepartamentsRepository _departamentsRepository;
        private readonly ILogger<OldDepsDeletionService> _logger;

        public OldDepsDeletionService(
            ITransactionManager transactionManager,
            IDepartamentsRepository departamentsRepository,
            ILogger<OldDepsDeletionService> logger)
        {
            _transactionManager = transactionManager;
            _departamentsRepository = departamentsRepository;
            _logger = logger;
        }

        public async Task<CSharpFunctionalExtensions.UnitResult<Error>> Start(CancellationToken cancellationToken)
        {
            /*
            * 1. Смотрим на дату deleted_at > 1 мес, получаем список (вдруг их больше одного сразу)
            * далее п. 2-5 в цикле
            * 2. Лочим по id протухший деп и его детей (метод есть)
            * 3. Лочим родителя протухшего депа по parent_id (метод есть)
            * 4. Получаем детей депа
            * 5. Каждого ребёнка 1 уровня просто переносим в родителя протухшего депа (тоже цикл, метод есть)
            * 6. Удаляем протухшие депы
            */
            Result<ITransactionScope> transactionScopeResult = await _transactionManager.BeginTransactionAsync(cancellationToken); // открытие транзакции
            if (transactionScopeResult.IsFailure)
            {
                return transactionScopeResult.Error;
            }

            using ITransactionScope transactionScope = transactionScopeResult.Value;

            /*п.1*/

            Departament[]? depsForDel = await _departamentsRepository.GetDepsForHardDelete(cancellationToken);
            if(depsForDel == null || depsForDel.Length == 0)
            {
                transactionScope.Rollback();
                _logger.LogInformation("Старых депов не обнаружено, сервис идёт спать");
                return CSharpFunctionalExtensions.UnitResult.Success<Error>();
            }

            /*п.2-п.6*/

            foreach (Departament dep in depsForDel)
            {
                // п.2
                var lockDepWithChildsResult = await _departamentsRepository.GetChildDepsWithLockAsync(dep.Path, cancellationToken);
                if(lockDepWithChildsResult.IsFailure)
                {
                    transactionScope.Rollback();
                    return lockDepWithChildsResult.Error;
                }

                Departament newParent = null;

                // п.3
                if(dep.ParentId != null)
                {
                    var lockParentResult = await _departamentsRepository.GetByIdWithLockAsync(dep.ParentId.Value, cancellationToken);
                    if(lockParentResult.IsFailure)
                    {
                        transactionScope.Rollback();
                        return lockParentResult.Error;
                    }

                    newParent = lockParentResult.Value;
                }

                // п.4
                Departament[]? depsChilds = await _departamentsRepository.GetDepsChildrensById(dep.Id, cancellationToken);

                // п.5
                foreach (var depChild in depsChilds)
                {
                    Result<int> moveDepsResult = await _departamentsRepository.MoveDepWithChildernsAsync(
                        depChild.Path,
                        dep.Path,
                        newParent?.Path,
                        newParent?.Id,
                        cancellationToken);

                    if (moveDepsResult.IsFailure)
                    {
                        transactionScope.Rollback();
                        return moveDepsResult.Error;
                    }
                }
            }

            // п.6
            CSharpFunctionalExtensions.UnitResult<Error> delOldDeps = await _departamentsRepository.HardDeleteDep(
                depsForDel.Select(d => d.Id).ToArray(),
                cancellationToken);

            if (delOldDeps.IsFailure)
            {
                transactionScope.Rollback();
                return delOldDeps.Error;
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
                transactionScope.Rollback();
                return commitedResult.Error;
            }

            _logger.LogInformation("Сервис по полному удалению депов отработал успешно");

            return CSharpFunctionalExtensions.UnitResult.Success<Error>();
        }
    }

    public interface IDeletionService
    {
        Task<CSharpFunctionalExtensions.UnitResult<Error>> Start(CancellationToken cancellationToken);
    }
}