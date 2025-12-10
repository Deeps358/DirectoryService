using DirectoryServices.Application.Abstractions;
using DirectoryServices.Application.Database;
using DirectoryServices.Entities;
using Microsoft.Extensions.Logging;
using Shared.ResultPattern;

namespace DirectoryServices.Application.Departaments.ChangeParent
{
    public class ChangeParentHandler : ICommandHandler<Guid, ChangeParentCommand>
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

        public async Task<Result<Guid>> Handle(ChangeParentCommand command, CancellationToken cancellationToken)
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

            // далее проверю ребёнка и родителя 2 запросами, я не знаю в каком порядке БД выдаст мне массив сущностей
            Result<Departament[]> childDepResult = await _departamentsRepository.GetByIdAsync(
                [command.DepId],
                cancellationToken);

            if (childDepResult.IsFailure)
            {
                transactionScope.Rollback();
                return childDepResult.Error; // тут могут вернуться как ошибка записи из БД так и просто не найдено
            }

            child = childDepResult.Value[0];

            if (command.NewParent.ParentId.HasValue) // родителя может и не быть, не забыть
            {
                Result<Departament[]> parentDepResult = await _departamentsRepository.GetByIdAsync(
                [command.NewParent.ParentId.Value],
                cancellationToken);

                if (parentDepResult.IsFailure)
                {
                    transactionScope.Rollback();
                    return parentDepResult.Error; // тут могут вернуться как ошибка записи из БД так и просто не найдено
                }

                parent = parentDepResult.Value[0];
            }

            /* и ещё проверка что родителем указан не один из дочерних элементов ребёнка */

            /*
            * если путь родителя входит в путь ребёнка с 0 индекса
            * а у родителя ещё что-то остаётся в пути
            * значит родитель это дочерний элемент ребёнка
            */
            if (parent != null && parent.Path.Value.IndexOf(child.Path.Value) == 0)
            {
                return Error.Validation(
                    "departament.change_parent.child_parent_check",
                    ["Подразделение не может быть перенесено в своё дочернее подразделение!"]);
            }

            /* теперь надо поменять путь у всех детей через репозиторий */

            return child.Id.Value;
        }
    }
}