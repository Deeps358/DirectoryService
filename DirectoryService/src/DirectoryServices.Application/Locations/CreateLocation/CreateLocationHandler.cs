using DirectoryServices.Application.Abstractions;
using DirectoryServices.Application.Database;
using DirectoryServices.Contracts.Locations;
using DirectoryServices.Entities;
using DirectoryServices.Entities.ValueObjects.Locations;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared.ResultPattern;

namespace DirectoryServices.Application.Locations.CreateLocation
{
    public class CreateLocationHandler : ICommandHandler<Guid, CreateLocationCommand>
    {
        private readonly ITransactionManager _transactionManager;
        private readonly ILocationsRepository _locationsRepository;
        private readonly IValidator<CreateLocationDto> _validator;
        private readonly ILogger<CreateLocationHandler> _logger;

        public CreateLocationHandler(
            ITransactionManager transactionManager,
            ILocationsRepository locationsRepository,
            IValidator<CreateLocationDto> validator,
            ILogger<CreateLocationHandler> logger)
        {
            _transactionManager = transactionManager;
            _locationsRepository = locationsRepository;
            _validator = validator;
            _logger = logger;
        }

        public async Task<Result<Guid>> Handle(CreateLocationCommand command, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(command.Location, cancellationToken);
            if (!validationResult.IsValid)
            {
                return GeneralErrors.InvalidFieldsError("location", validationResult.Errors.Select(e => e.ErrorMessage).ToArray());
            }

            Result<LocName> newLocName = LocName.Create(command.Location.Name);

            Result<LocAdress> newLocAdress = LocAdress.Create(
                command.Location.Adress.City,
                command.Location.Adress.Street,
                command.Location.Adress.Building,
                command.Location.Adress.Room);

            Result<LocTimezone> newTimeZone = LocTimezone.Create(command.Location.Timezone);

            Result<Location> locResult = Location.Create(
                newLocName,
                newLocAdress,
                newTimeZone,
                command.Location.isActive);

            Result<ITransactionScope> transactionScopeResult = await _transactionManager.BeginTransactionAsync(cancellationToken); // открытие транзакции
            if (transactionScopeResult.IsFailure)
            {
                return transactionScopeResult.Error;
            }

            using ITransactionScope transactionScope = transactionScopeResult.Value;

            Result<Guid> newLocation = await _locationsRepository.CreateAsync(locResult.Value, cancellationToken);

            if (newLocation.IsFailure)
            {
                transactionScope.Rollback();
                _logger.LogInformation($"Ошибка записи в базу: {newLocation.Error}");
                return newLocation.Error!;
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

            _logger.LogInformation("Создана локация с id {newLocation.Value}", newLocation.Value);

            return newLocation;
        }
    }
}
