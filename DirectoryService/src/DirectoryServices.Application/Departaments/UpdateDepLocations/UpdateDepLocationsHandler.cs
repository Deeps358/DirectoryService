using DirectoryServices.Application.Abstractions;
using DirectoryServices.Application.Database;
using DirectoryServices.Application.Locations;
using DirectoryServices.Contracts.Departaments;
using DirectoryServices.Entities;
using DirectoryServices.Entities.ValueObjects.Locations;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Shared.ResultPattern;

namespace DirectoryServices.Application.Departaments.UpdateDepLocations
{
    public class UpdateDepLocationsHandler : ICommandHandler<Guid, UpdateDepLocationsCommand>
    {
        private readonly ITransactionManager _transactionManager;
        private readonly IDepartamentsRepository _departamentsRepository;
        private readonly ILocationsRepository _locationsRepository;
        private readonly IValidator<UpdateDepLocationsDto> _validator;
        private readonly ILogger<UpdateDepLocationsHandler> _logger;

        public UpdateDepLocationsHandler(
            ITransactionManager transactionManager,
            IDepartamentsRepository departamentsRepository,
            ILocationsRepository locationsRepository,
            IValidator<UpdateDepLocationsDto> validator,
            ILogger<UpdateDepLocationsHandler> logger)
        {
            _transactionManager = transactionManager;
            _departamentsRepository = departamentsRepository;
            _locationsRepository = locationsRepository;
            _validator = validator;
            _logger = logger;
        }

        public async Task<Result<Guid>> Handle(UpdateDepLocationsCommand command, CancellationToken cancellationToken)
        {
            /* проверим что массив локаций валиден */

            ValidationResult validationResult = await _validator.ValidateAsync(command.Locations);
            if(!validationResult.IsValid)
            {
                return GeneralErrors.InvalidFieldsError("departament", validationResult.Errors.Select(e => e.ErrorMessage).ToArray());
            }

            /* открываем транзакцию */

            Result<ITransactionScope> transactionScopeResult = await _transactionManager.BeginTransactionAsync(cancellationToken); // открытие транзакции
            if (transactionScopeResult.IsFailure)
            {
                return transactionScopeResult.Error;
            }

            using ITransactionScope transactionScope = transactionScopeResult.Value;

            /* проверить что переданные локации существуют */

            Result<Location[]> locations = await _locationsRepository.GetByIdAsync(
                command.Locations.LocationsIds.Distinct().ToArray(),
                cancellationToken);

            if (locations.IsFailure)
            {
                transactionScope.Rollback();
                return locations.Error; // тут могут вернуться как ошибка записи из БД так и просто не найдено
            }

            /* получим саму сущность подразделения из БД */

            Result<Departament[]> depResult = await _departamentsRepository.GetByIdAsync(
                [command.DepId],
                cancellationToken);

            if(depResult.IsFailure)
            {
                transactionScope.Rollback();
                return depResult.Error; // тут могут вернуться как ошибка записи из БД так и просто не найдено
            }

            Departament departament = depResult.Value[0]; // по идее вернётся массив из 1 объекта

            /* удалю связи со старыми локациями */

            await _departamentsRepository.DeleteLocationsByDepAsync(departament.Id, cancellationToken);

            /* добавлю новые */

            List<DepartmentLocation> deplocs = new List<DepartmentLocation>();

            foreach (Guid locationId in command.Locations.LocationsIds.Distinct()) // не забыть почистить от дубликатов
            {
                deplocs.Add(DepartmentLocation.Create(departament.Id, LocId.GetCurrent(locationId)));
            }

            departament.UpdateLocations(deplocs);

            var saveResult = await _transactionManager.SaveChangesAsync(cancellationToken);
            if (saveResult.IsFailure)
            {
                transactionScope.Rollback();
                return saveResult.Error;
            }

            CSharpFunctionalExtensions.UnitResult<Error> commitedResult = transactionScope.Commit();
            if (commitedResult.IsFailure)
            {
                transactionScope.Rollback();
                return commitedResult.Error;
            }

            _logger.LogInformation("Обновлены локации у подразделения с id {departament.Id.Value}", departament.Id.Value);

            return departament.Id.Value;
        }
    }
}