using DirectoryServices.Application.Abstractions;
using DirectoryServices.Application.Database;
using DirectoryServices.Entities;
using DirectoryServices.Entities.ValueObjects.Departaments;
using Microsoft.Extensions.Logging;
using Shared.ResultPattern;

namespace DirectoryServices.Application.Departaments.Commands.ChangeParent
{
    public class ChangeParentHandler : ICommandHandler<int, ChangeParentCommand>
    {
        private readonly ITransactionManager _transactionManager;
        private readonly IDepartamentsRepository _departamentsRepository;
        private readonly ILogger<ChangeParentHandler> _logger;

        public ChangeParentHandler(
            ITransactionManager transactionManager,
            IDepartamentsRepository departamentsRepository,
            ILogger<ChangeParentHandler> logger)
        {
            _transactionManager = transactionManager;
            _departamentsRepository = departamentsRepository;
            _logger = logger;
        }

        public async Task<Result<int>> Handle(ChangeParentCommand command, CancellationToken cancellationToken)
        {
            /* открываем транзакцию */

            Result<ITransactionScope> transactionScopeResult = await _transactionManager.BeginTransactionAsync(cancellationToken); // открытие транзакции
            if (transactionScopeResult.IsFailure)
            {
                return transactionScopeResult.Error;
            }

            using ITransactionScope transactionScope = transactionScopeResult.Value;

            /* проверить что переданные ребёнок и родитель существуют */

            if(command.DepId == command.NewParent.ParentId) // только проверим что они не равны, а то в валидаторе это не получится
            {
                transactionScope.Rollback();
                return Error.Validation("departament.changeparent.equal", ["Id родителя и ребёнка совпадают"], "parentId");
            }

            Departament parent = null;
            Departament child = null;

            // далее проверю ребёнка и родителя 2 запросами с блокировкой, я не знаю в каком порядке БД выдаст мне массив сущностей
            Result<Departament> childDepResult = await _departamentsRepository.GetByIdWithLockAsync(command.DepId, cancellationToken);

            if (childDepResult.IsFailure)
            {
                transactionScope.Rollback();
                return childDepResult.Error; // тут могут вернуться как ошибка чтения из БД так и просто не найдено
            }

            child = childDepResult.Value;

            /* заблокирую ещё всех детей депа чтоб их не трогали */

            CSharpFunctionalExtensions.UnitResult<Error> getChildrensResult = await _departamentsRepository.GetChildDepsWithLockAsync(child.Path, cancellationToken);

            if(getChildrensResult.IsFailure)
            {
                transactionScope.Rollback();
                return getChildrensResult.Error;
            }

            if (command.NewParent.ParentId.HasValue) // родителя может и не быть, не забыть
            {
                Result<Departament> parentDepResult = await _departamentsRepository.GetByIdWithLockAsync(command.NewParent.ParentId.Value, cancellationToken);

                if (parentDepResult.IsFailure)
                {
                    transactionScope.Rollback();
                    return parentDepResult.Error; // тут могут вернуться как ошибка чтения из БД так и просто не найдено
                }

                parent = parentDepResult.Value;
            }

            /* и ещё проверка что родителем указан не один из дочерних элементов ребёнка */

            /*
            * если путь родителя входит в путь ребёнка с 0 индекса
            * а у родителя ещё что-то остаётся в пути
            * значит родитель это дочерний элемент ребёнка
            */
            if (parent != null && parent.Path.Value.IndexOf(child.Path.Value) == 0)
            {
                transactionScope.Rollback();
                return Error.Validation(
                    "departament.change_parent.child_parent_check",
                    ["Подразделение не может быть перенесено в своё дочернее подразделение!"]);
            }

            /* теперь надо поменять путь у всех детей через репозиторий */

            int index = child.Path.Value.LastIndexOf('.');
            int countToRemove = child.Path.Value.Length - index;
            string curParentPath =
                index == -1
                ? string.Empty
                : child.Path.Value.Remove(index, countToRemove); // получу так чтоб не делать ещё запрос в БД на родителя

            Result<int> affectedRowsResult = await _departamentsRepository.MoveDepWithChildernsAsync(
                child.Path,
                DepPath.GetCurrent(curParentPath),
                parent?.Path,
                command.NewParent.ParentId.HasValue ? DepId.GetCurrent(command.NewParent.ParentId.Value) : null,
                cancellationToken);

            if (affectedRowsResult.IsFailure)
            {
                transactionScope.Rollback();
                return affectedRowsResult.Error;
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

            _logger.LogInformation("У подразделения с id {child.Id.Value} изменился родитель. Всего изменён путь у {affectedRows} подразделений", child.Id.Value, affectedRowsResult);

            return affectedRowsResult;
        }
    }
}