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
            * 2. Лочим по path протухшие депы и их детей (передача массивом)
            * 3. Лочим родителей протухших депов по массиву parent_id (метод есть)
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

            // п.2 лок старых депов и их детей
            DepPath[] paths = depsForDel.Select(d => d.Path).ToArray();

            var lockDepsWithChildsResult = await _departamentsRepository.LockDepsAndChildsAsync(paths, cancellationToken);
            if(lockDepsWithChildsResult.IsFailure)
            {
                transactionScope.Rollback();
                return lockDepsWithChildsResult.Error;
            }

            // п.3 Лочим родителей протухших депов по массиву parent_id
            Departament[] newParents = null;

            DepId?[] parentsIds = depsForDel
                .Where(d => d.ParentId != null)
                .Select(d => d.ParentId)
                .ToArray();

            if(parentsIds != null)
            {
                var lockParentsResult = await _departamentsRepository.GetByIdsWithLockAsync(parentsIds, cancellationToken);
                if(lockParentsResult.IsFailure)
                {
                    transactionScope.Rollback();
                    return lockParentsResult.Error;
                }

                newParents = lockParentsResult.Value;
            }

            // п.4 Получаем прямых потомков (1 уровня) депов из массива
            DepId[] ids = depsForDel.Select(d => d.Id).ToArray();
            Departament[]? depsChilds = await _departamentsRepository.GetDepsChildrensFirstLevelById(ids, cancellationToken);

            // п.5 Каждого ребёнка из списка с его детьми переносим в "деда" удаляемого подразделения
            foreach (var depChild in depsChilds)
            {
                DepPath curDepPath = depChild.Path;

                int curStart = curDepPath.Value.LastIndexOf(".");
                DepPath parentPath = DepPath.GetCurrent(curDepPath.Value.Remove(curStart));

                int parentStart = parentPath.Value.LastIndexOf(".");
                DepPath grandParentPath = null;
                if(parentStart != -1)
                {
                    grandParentPath = DepPath.GetCurrent(parentPath.Value.Remove(parentStart));
                }

                Result<int> moveDepsResult = await _departamentsRepository.MoveDepWithChildernsAsync(
                    curDepPath,
                    parentPath,
                    grandParentPath,
                    newParents?.Where(d => d.Path == grandParentPath).Select(d => d.Id).FirstOrDefault(),
                    cancellationToken);

                if (moveDepsResult.IsFailure)
                {
                    transactionScope.Rollback();
                    return moveDepsResult.Error;
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