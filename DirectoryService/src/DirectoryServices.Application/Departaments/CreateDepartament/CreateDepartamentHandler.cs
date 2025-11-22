using DirectoryServices.Application.Abstractions;
using DirectoryServices.Application.Locations;
using DirectoryServices.Contracts.Departaments;
using DirectoryServices.Entities;
using DirectoryServices.Entities.ValueObjects.Departaments;
using DirectoryServices.Entities.ValueObjects.Locations;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared.ResultPattern;

namespace DirectoryServices.Application.Departaments.CreateDepartament
{
    public class CreateDepartamentHandler : ICommandHandler<Guid, CreateDepartamentCommand>
    {
        private readonly IDepartamentsRepository _departamentsRepository;
        private readonly ILocationsRepository _locationsRepository;
        private readonly IValidator<CreateDepartamentDto> _validator;
        private readonly ILogger<CreateDepartamentHandler> _logger;

        public CreateDepartamentHandler(
            IDepartamentsRepository departamentsRepository,
            ILocationsRepository locationsRepository,
            IValidator<CreateDepartamentDto> validator,
            ILogger<CreateDepartamentHandler> logger)
        {
            _departamentsRepository = departamentsRepository;
            _locationsRepository = locationsRepository;
            _validator = validator;
            _logger = logger;
        }

        public async Task<Result<Guid>> Handle(CreateDepartamentCommand command, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(command.Departament);
            if(!validationResult.IsValid)
            {
                return GeneralErrors.InvalidFieldsError("departament", validationResult.Errors.Select(e => e.ErrorMessage).ToArray());
            }

            var newDepId = DepId.NewDepId();
            var newDepName = DepName.Create(command.Departament.Name);
            var newDepIdentifier = DepIdentifier.Create(command.Departament.Identifier);

            /*
             * TODO:
             * если гид родителя не пустой, то проверить что он существует
             * не пустой и существует? вернём родителя
             * проверить что локации переданы правильно
             */

            Departament parent = null;

            if (command.Departament.ParentId.HasValue)
            {
                Result<Departament[]> requestedParent = await _departamentsRepository.GetByIdAsync([command.Departament.ParentId.Value], cancellationToken);
                if (requestedParent.IsFailure)
                {
                    return requestedParent.Error;
                }

                parent = requestedParent.Value[0];

                if (parent.Identifier == newDepIdentifier)
                {
                    return Error.Conflict("departament.conflict.identifier", ["Идентификатор родителя и ребёнка не должны совпадать!"]);
                }
            }

            List<DepartmentLocation> deplocs = new List<DepartmentLocation>();

            Result<Location[]> locations = await _locationsRepository.GetByIdAsync(command.Departament.LocationsIds, cancellationToken);
            if (locations.IsFailure)
            {
                return locations.Error; // тут могут вернуться как ошибка записи из БД так и просто не найдено
            }

            foreach (Guid locationId in command.Departament.LocationsIds)
            {
                deplocs.Add(DepartmentLocation.Create(newDepId, LocId.GetCurrent(locationId)));
            }

            // ещё не забыть подразделения

            Departament depResult = Departament.Create(
                newDepId,
                newDepName,
                newDepIdentifier,
                parent,
                deplocs,
                null,
                command.Departament.IsActive)
                .Value;

            Result<Guid> newDep = await _departamentsRepository.CreateAsync(depResult, cancellationToken);
            if (newDep.IsFailure)
            {
                _logger.LogInformation($"Ошибка записи в базу: {newDep.Error}");
                return newDep.Error;
            }

            _logger.LogInformation("Создано подразделение с id {newDep.Value}", newDep.Value);

            return newDep.Value;
        }
    }
}
