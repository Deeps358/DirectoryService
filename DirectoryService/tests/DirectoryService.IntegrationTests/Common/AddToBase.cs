using CSharpFunctionalExtensions;
using DirectoryService.IntegrationTests.Infrastructure;
using DirectoryServices.Entities;
using DirectoryServices.Entities.ValueObjects.Departaments;
using DirectoryServices.Entities.ValueObjects.Locations;
using Shared.ResultPattern;

namespace DirectoryService.IntegrationTests.Common
{
    public class AddToBase : DirectoryBaseTests
    {
        public AddToBase(DirectoryTestWebFactory factory)
            : base(factory)
        {
        }

        public async Task<UnitResult<Error>> AddLocationToBase(Location[] locations, CancellationToken cancellationToken)
        {
            return await ExecuteInDb(async dbContext =>
            {
                await dbContext.Locations.AddRangeAsync(locations, cancellationToken);
                await dbContext.SaveChangesAsync();

                return UnitResult.Success<Error>();
            });
        }

        public async Task<UnitResult<Error>> AddDepartamentToBase(Departament[] departaments, CancellationToken cancellationToken)
        {
            return await ExecuteInDb(async dbContext =>
            {
                await dbContext.Departaments.AddRangeAsync(departaments, cancellationToken);
                await dbContext.SaveChangesAsync();

                return UnitResult.Success<Error>();
            });
        }
    }
}