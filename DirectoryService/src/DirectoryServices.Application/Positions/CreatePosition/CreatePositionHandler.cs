using DirectoryServices.Application.Abstractions;
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
        private readonly IPositionsRepository _positionsRepository;
        private readonly IDepartamentsRepository _departamentsRepository;
        private readonly IValidator<CreatePositionDto> _validator;
        private readonly ILogger<CreatePositionHandler> _logger;

        public CreatePositionHandler(
            IPositionsRepository positionsRepository,
            IDepartamentsRepository departamentsRepository,
            IValidator<CreatePositionDto> validator,
            ILogger<CreatePositionHandler> logger)
        {
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

            /*Result<Position> checkPosName = await _positionsRepository.GetByNameAsync(newPosName, cancellationToken);
            if (checkPosName.IsFailure)
            {
                return checkPosName.Error;
            }

            if (checkPosName.Value != null)
            {
                return Error.Conflict("position.duplicate.name", ["Такое имя позиции уже есть!"]);
            }*/

            PosDescription newPosDesc = PosDescription.Create(command.Position.Description).Value;

            // проверка на существующие депы
            List<DepartmentPosition> depPositions = new List<DepartmentPosition>();

            Result<Departament[]> departaments = await _departamentsRepository.GetByIdAsync(
                command.Position.DepartmentIds.Distinct().ToArray(),
                cancellationToken);

            if (departaments.IsFailure)
            {
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
                _logger.LogInformation($"Ошибка записи в базу: {newPos.Error}");
                return newPos.Error;
            }

            _logger.LogInformation("Создана позиция с id {newPos.Value}", newPos.Value);

            return newPos.Value;
        }
    }
}
