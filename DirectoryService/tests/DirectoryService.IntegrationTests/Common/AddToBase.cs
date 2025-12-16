using DirectoryService.IntegrationTests.Infrastructure;
using DirectoryServices.Entities;
using DirectoryServices.Entities.ValueObjects.Departaments;
using DirectoryServices.Entities.ValueObjects.Locations;

namespace DirectoryService.IntegrationTests.Common
{
    public class AddToBase : DirectoryBaseTests
    {
        public AddToBase(DirectoryTestWebFactory factory)
            : base(factory)
        {
        }

        public async Task<LocId> AddLocationToBase(LocName locName, LocAdress locAdress, LocTimezone locTimezone, bool isActive)
        {
            return await ExecuteInDb(async dbContext =>
            {
                Location location = Location.Create(
                    locName,
                    locAdress,
                    locTimezone,
                    isActive);

                dbContext.Locations.Add(location);
                await dbContext.SaveChangesAsync();

                return location.Id;
            });
        }

        public async Task<DepId> AddDepartamentToBase(
            DepId depId,
            DepName depName,
            DepIdentifier depIdentifier,
            Departament? parent,
            IEnumerable<DepartmentLocation> locations,
            IEnumerable<DepartmentPosition> positions,
            bool isActive)
        {
            return await ExecuteInDb(async dbContext =>
            {
                Departament departament = Departament.Create(
                    depId,
                    depName,
                    depIdentifier,
                    parent,
                    locations,
                    positions,
                    isActive);

                dbContext.Departaments.Add(departament);
                await dbContext.SaveChangesAsync();

                return departament.Id;
            });
        }
    }
}