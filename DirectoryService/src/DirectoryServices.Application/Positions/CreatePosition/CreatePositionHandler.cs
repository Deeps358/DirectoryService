using DirectoryServices.Application.Abstractions;
using DirectoryServices.Application.Database;
using DirectoryServices.Application.Departaments;
using DirectoryServices.Contracts.Positions;
using DirectoryServices.Entities;
using DirectoryServices.Entities.ValueObjects.Departaments;
using DirectoryServices.Entities.ValueObjects.Positions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared.ResultPattern;

namespace DirectoryServices.Application.Positions.CreatePosition
{
    public class CreatePositionHandler : ICommandHandler<Guid, CreatePositionCommand>
    {
        private readonly ITransactionManager _transactionManager;
        private readonly IPositionsRepository _positionsRepository;
        private readonly IDepartamentsRepository _departamentsRepository;
        private readonly IValidator<CreatePositionDto> _validator;
        private readonly ILogger<CreatePositionHandler> _logger;

        public CreatePositionHandler(
            ITransactionManager transactionManager,
            IPositionsRepository positionsRepository,
            IDepartamentsRepository departamentsRepository,
            IValidator<CreatePositionDto> validator,
            ILogger<CreatePositionHandler> logger)
        {
            _transactionManager = transactionManager;
            _positionsRepository = positionsRepository;
            _departamentsRepository = departamentsRepository;
            _validator = validator;
            _logger = logger;
        }

        public async Task<Result<Guid>> Handle(CreatePositionCommand command, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(command.Position, cancellationToken);
            if (!validationResult.IsValid)
            {
                return GeneralErrors.InvalidFieldsError("position", validationResult.Errors.Select(e => e.ErrorMessage).ToArray());
            }

            PosId newPosId = PosId.NewPosId();
            PosName newPosName = PosName.Create(command.Position.Name);

            PosDescription newPosDesc = PosDescription.Create(command.Position.Description).Value;

            // проверка на существующие депы
            List<DepartmentPosition> depPositions = new List<DepartmentPosition>();

            Result<ITransactionScope> transactionScopeResult = await _transactionManager.BeginTransactionAsync(cancellationToken); // открытие транзакции
            if (transactionScopeResult.IsFailure)
            {
                return transactionScopeResult.Error;
            }

            using ITransactionScope transactionScope = transactionScopeResult.Value;

            Result<Departament[]> departaments = await _departamentsRepository.GetByIdAsync(
                command.Position.DepartmentIds.Distinct().ToArray(),
                cancellationToken);

            if (departaments.IsFailure)
            {
                transactionScope.Rollback();
                return departaments.Error; // тут могут вернуться как ошибка записи из БД так и просто не найдено
            }

            foreach (Guid departamentId in command.Position.DepartmentIds)
            {
                depPositions.Add(DepartmentPosition.Create(DepId.GetCurrent(departamentId), newPosId));
            }

            // создать саму позицию
            Position posResult = Position.Create(
                newPosId,
                newPosName,
                newPosDesc,
                depPositions,
                command.Position.IsActive)
                .Value;

            Result<Guid> newPos = await _positionsRepository.CreateAsync(posResult, cancellationToken);
            if (newPos.IsFailure)
            {
                transactionScope.Rollback();
                _logger.LogInformation($"Ошибка записи в базу: {newPos.Error}");
                return newPos.Error;
            }

            await _transactionManager.SaveChangesAsync(cancellationToken);

            var commitedResult = transactionScope.Commit();
            if (commitedResult.IsFailure)
            {
                transactionScope.Rollback();
                return commitedResult.Error;
            }

            _logger.LogInformation("Создана позиция с id {newPos.Value}", newPos.Value);

            return newPos.Value;
        }
    }
}
