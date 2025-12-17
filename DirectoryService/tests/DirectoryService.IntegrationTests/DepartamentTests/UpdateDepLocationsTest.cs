using DirectoryService.IntegrationTests.Common;
using DirectoryService.IntegrationTests.Infrastructure;
using DirectoryServices.Application.Departaments.UpdateDepLocations;
using DirectoryServices.Contracts.Departaments;
using DirectoryServices.Entities;
using DirectoryServices.Entities.ValueObjects.Departaments;
using DirectoryServices.Entities.ValueObjects.Locations;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared.ResultPattern;

namespace DirectoryService.IntegrationTests.DepartamentTests
{
    public class UpdateDepLocationsTest : DirectoryBaseTests
    {
        protected AddToBase BaseAdding { get; private set; }

        public UpdateDepLocationsTest(DirectoryTestWebFactory factory)
            : base(factory)
        {
            BaseAdding = new AddToBase(factory);
        }

        [Fact]
        public async Task UpdateDepLocs_with_valid_data_should_succeed()
        {
            // arrange
            var cancellationToken = CancellationToken.None;

            Location loc = Location.Create(
                LocName.Create("loc1"),
                LocAdress.Create("city1", "street1", 1, "room1"),
                LocTimezone.Create("Europe/Moscow"),
                true);

            Location loc2 = Location.Create(
                LocName.Create("loc2"),
                LocAdress.Create("city2", "street2", 2, "room2"),
                LocTimezone.Create("Europe/Moscow"),
                true);

            Location loc3 = Location.Create(
                LocName.Create("loc3"),
                LocAdress.Create("city3", "street3", 1, "room3"),
                LocTimezone.Create("Europe/Moscow"),
                true);

            Location loc4 = Location.Create(
                LocName.Create("loc4"),
                LocAdress.Create("city4", "street4", 4, "room4"),
                LocTimezone.Create("Europe/Moscow"),
                true);

            await BaseAdding.AddLocationToBase([loc, loc2, loc3, loc4], cancellationToken); // локации в базу

            DepId newDepId = DepId.NewDepId();

            List<DepartmentLocation> currentDeplocs = [
                DepartmentLocation.Create(newDepId, LocId.GetCurrent(loc.Id.Value))];

            List<DepartmentLocation> newDeplocs = [
                DepartmentLocation.Create(newDepId, LocId.GetCurrent(loc2.Id.Value)),
                DepartmentLocation.Create(newDepId, LocId.GetCurrent(loc3.Id.Value)),
                DepartmentLocation.Create(newDepId, LocId.GetCurrent(loc4.Id.Value))];

            Departament dep = Departament.Create(
                newDepId,
                DepName.Create("Director"),
                DepIdentifier.Create("dir"),
                null,
                currentDeplocs,
                [],
                true);

            await BaseAdding.AddDepartamentToBase([dep], cancellationToken); // депы в базу

            // act
            Result<Guid> result = await ExecuteHandler((sut) =>
            {
                var command = new UpdateDepLocationsCommand(
                    dep.Id.Value,
                    new UpdateDepLocationsDto(newDeplocs.Select(dl => dl.LocationId.Value).ToArray()));

                return sut.Handle(command, cancellationToken);
            });

            // assert
            await ExecuteInDb(async dbContext =>
            {
                result.IsSuccess.Should().BeTrue();
                dep.Id.Value.Should().Be(result.Value);

                List<DepartmentLocation> depLocs = await dbContext.DepartmentLocations.Where(dl => dl.DepartamentId == dep.Id).ToListAsync(cancellationToken);

                depLocs.Should().NotBeEmpty();
                depLocs.Count.Should().Be(newDeplocs.Count);

                // сравнить списки по значению не получится из-за разных значений CreatedAt и UpdatedAt, вычленяем сеты с id
                var set1 = depLocs.Select(dl => new { dl.DepartamentId, dl.LocationId }).ToHashSet();
                var set2 = newDeplocs.Select(dl => new { dl.DepartamentId, dl.LocationId }).ToHashSet();

                set1.SetEquals(set2).Should().BeTrue();
            });
        }

        [Fact]
        public async Task UpdateDepLocs_with_incorrect_location_ids_should_fail()
        {
            // arrange
            var cancellationToken = CancellationToken.None;

            Location loc = Location.Create(
                LocName.Create("loc1"),
                LocAdress.Create("city1", "street1", 1, "room1"),
                LocTimezone.Create("Europe/Moscow"),
                true);

            Location loc2 = Location.Create(
                LocName.Create("loc2"),
                LocAdress.Create("city2", "street2", 2, "room2"),
                LocTimezone.Create("Europe/Moscow"),
                true);

            await BaseAdding.AddLocationToBase([loc, loc2], cancellationToken); // локации в базу

            DepId newDepId = DepId.NewDepId();

            List<DepartmentLocation> currentDeplocs = [
                DepartmentLocation.Create(newDepId, LocId.GetCurrent(loc.Id.Value))];

            List<DepartmentLocation> newDeplocs = [
                DepartmentLocation.Create(newDepId, LocId.GetCurrent(loc2.Id.Value))];

            Departament dep = Departament.Create(
                newDepId,
                DepName.Create("Director"),
                DepIdentifier.Create("dir"),
                null,
                currentDeplocs,
                [],
                true);

            await BaseAdding.AddDepartamentToBase([dep], cancellationToken); // депы в базу

            // act
            Result<Guid> result = await ExecuteHandler((sut) =>
            {
                var command = new UpdateDepLocationsCommand(
                    dep.Id.Value,
                    new UpdateDepLocationsDto([Guid.NewGuid(), Guid.NewGuid()]));

                return sut.Handle(command, cancellationToken);
            });

            // assert
            await ExecuteInDb(async dbContext =>
            {
                result.IsFailure.Should().BeTrue();
                result.Error.Message.Should().NotBeEmpty();
            });
        }

        [Fact]
        public async Task UpdateDepLocs_with_null_location_ids_should_fail()
        {
            // arrange
            var cancellationToken = CancellationToken.None;

            Location loc = Location.Create(
                LocName.Create("loc1"),
                LocAdress.Create("city1", "street1", 1, "room1"),
                LocTimezone.Create("Europe/Moscow"),
                true);

            Location loc2 = Location.Create(
                LocName.Create("loc2"),
                LocAdress.Create("city2", "street2", 2, "room2"),
                LocTimezone.Create("Europe/Moscow"),
                true);

            await BaseAdding.AddLocationToBase([loc, loc2], cancellationToken); // локации в базу

            DepId newDepId = DepId.NewDepId();

            List<DepartmentLocation> currentDeplocs = [
                DepartmentLocation.Create(newDepId, LocId.GetCurrent(loc.Id.Value))];

            List<DepartmentLocation> newDeplocs = [
                DepartmentLocation.Create(newDepId, LocId.GetCurrent(loc2.Id.Value))];

            Departament dep = Departament.Create(
                newDepId,
                DepName.Create("Director"),
                DepIdentifier.Create("dir"),
                null,
                currentDeplocs,
                [],
                true);

            await BaseAdding.AddDepartamentToBase([dep], cancellationToken); // депы в базу

            // act
            Result<Guid> result = await ExecuteHandler((sut) =>
            {
                var command = new UpdateDepLocationsCommand(
                    dep.Id.Value,
                    new UpdateDepLocationsDto(null));

                return sut.Handle(command, cancellationToken);
            });

            // assert
            await ExecuteInDb(async dbContext =>
            {
                result.IsFailure.Should().BeTrue();
                result.Error.Message.Should().NotBeEmpty();
            });
        }

        [Fact]
        public async Task UpdateDepLocs_with_incorrect_departament_id_should_fail()
        {
            // arrange
            var cancellationToken = CancellationToken.None;

            Location loc = Location.Create(
                LocName.Create("loc1"),
                LocAdress.Create("city1", "street1", 1, "room1"),
                LocTimezone.Create("Europe/Moscow"),
                true);

            Location loc2 = Location.Create(
                LocName.Create("loc2"),
                LocAdress.Create("city2", "street2", 2, "room2"),
                LocTimezone.Create("Europe/Moscow"),
                true);

            await BaseAdding.AddLocationToBase([loc, loc2], cancellationToken); // локации в базу

            DepId newDepId = DepId.NewDepId();

            List<DepartmentLocation> currentDeplocs = [
                DepartmentLocation.Create(newDepId, LocId.GetCurrent(loc.Id.Value))];

            List<DepartmentLocation> newDeplocs = [
                DepartmentLocation.Create(newDepId, LocId.GetCurrent(loc2.Id.Value))];

            Departament dep = Departament.Create(
                newDepId,
                DepName.Create("Director"),
                DepIdentifier.Create("dir"),
                null,
                currentDeplocs,
                [],
                true);

            await BaseAdding.AddDepartamentToBase([dep], cancellationToken); // депы в базу

            // act
            Result<Guid> result = await ExecuteHandler((sut) =>
            {
                var command = new UpdateDepLocationsCommand(
                    Guid.NewGuid(),
                    new UpdateDepLocationsDto(newDeplocs.Select(dl => dl.LocationId.Value).ToArray()));

                return sut.Handle(command, cancellationToken);
            });

            // assert
            await ExecuteInDb(async dbContext =>
            {
                result.IsFailure.Should().BeTrue();
                result.Error.Message.Should().NotBeEmpty();
            });
        }

        private async Task<T> ExecuteHandler<T>(Func<UpdateDepLocationsHandler, Task<T>> action)
        {
            await using var scope = Services.CreateAsyncScope();

            UpdateDepLocationsHandler sut = scope.ServiceProvider.GetRequiredService<UpdateDepLocationsHandler>(); // sut - system under test

            return await action(sut);
        }
    }
}