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
            LocId locId = await BaseAdding.AddLocationToBase(
                LocName.Create("loc1"),
                LocAdress.Create("city1", "street1", 1, "room1"),
                LocTimezone.Create("Europe/Moscow"),
                true);

            LocId locId2 = await BaseAdding.AddLocationToBase(
                LocName.Create("loc2"),
                LocAdress.Create("city2", "street2", 2, "room2"),
                LocTimezone.Create("Europe/Moscow"),
                true);

            LocId locId3 = await BaseAdding.AddLocationToBase(
                LocName.Create("loc3"),
                LocAdress.Create("city3", "street3", 1, "room3"),
                LocTimezone.Create("Europe/Moscow"),
                true);

            LocId locId4 = await BaseAdding.AddLocationToBase(
                LocName.Create("loc4"),
                LocAdress.Create("city4", "street4", 4, "room4"),
                LocTimezone.Create("Europe/Moscow"),
                true);

            DepId newDepId = DepId.NewDepId();

            List<DepartmentLocation> currentDeplocs = [
                DepartmentLocation.Create(newDepId, LocId.GetCurrent(locId.Value))];

            List<DepartmentLocation> newDeplocs = [
                DepartmentLocation.Create(newDepId, LocId.GetCurrent(locId2.Value)),
                DepartmentLocation.Create(newDepId, LocId.GetCurrent(locId3.Value)),
                DepartmentLocation.Create(newDepId, LocId.GetCurrent(locId4.Value))];

            DepId depId = await BaseAdding.AddDepartamentToBase(
                newDepId,
                DepName.Create("Director"),
                DepIdentifier.Create("dir"),
                null,
                currentDeplocs,
                [],
                true);

            CancellationToken cancellationToken = CancellationToken.None;

            // act
            Result<Guid> result = await ExecuteHandler((sut) =>
            {
                var command = new UpdateDepLocationsCommand(
                    depId.Value,
                    new UpdateDepLocationsDto(newDeplocs.Select(dl => dl.LocationId.Value).ToArray()));

                return sut.Handle(command, cancellationToken);
            });

            // assert
            await ExecuteInDb(async dbContext =>
            {
                result.IsSuccess.Should().BeTrue();
                depId.Value.Should().Be(result.Value);

                List<DepartmentLocation> depLocs = await dbContext.DepartmentLocations.Where(dl => dl.DepartamentId == depId).ToListAsync(cancellationToken);

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
            LocId locId = await BaseAdding.AddLocationToBase(
                LocName.Create("loc1"),
                LocAdress.Create("city1", "street1", 1, "room1"),
                LocTimezone.Create("Europe/Moscow"),
                true);

            LocId locId2 = await BaseAdding.AddLocationToBase(
                LocName.Create("loc2"),
                LocAdress.Create("city2", "street2", 2, "room2"),
                LocTimezone.Create("Europe/Moscow"),
                true);

            DepId newDepId = DepId.NewDepId();

            List<DepartmentLocation> currentDeplocs = [
                DepartmentLocation.Create(newDepId, LocId.GetCurrent(locId.Value))];

            List<DepartmentLocation> newDeplocs = [
                DepartmentLocation.Create(newDepId, LocId.GetCurrent(locId2.Value))];

            DepId depId = await BaseAdding.AddDepartamentToBase(
                newDepId,
                DepName.Create("Director"),
                DepIdentifier.Create("dir"),
                null,
                currentDeplocs,
                [],
                true);

            CancellationToken cancellationToken = CancellationToken.None;

            // act
            Result<Guid> result = await ExecuteHandler((sut) =>
            {
                var command = new UpdateDepLocationsCommand(
                    depId.Value,
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
            LocId locId = await BaseAdding.AddLocationToBase(
                LocName.Create("loc1"),
                LocAdress.Create("city1", "street1", 1, "room1"),
                LocTimezone.Create("Europe/Moscow"),
                true);

            LocId locId2 = await BaseAdding.AddLocationToBase(
                LocName.Create("loc2"),
                LocAdress.Create("city2", "street2", 2, "room2"),
                LocTimezone.Create("Europe/Moscow"),
                true);

            DepId newDepId = DepId.NewDepId();

            List<DepartmentLocation> currentDeplocs = [
                DepartmentLocation.Create(newDepId, LocId.GetCurrent(locId.Value))];

            List<DepartmentLocation> newDeplocs = [
                DepartmentLocation.Create(newDepId, LocId.GetCurrent(locId2.Value))];

            DepId depId = await BaseAdding.AddDepartamentToBase(
                newDepId,
                DepName.Create("Director"),
                DepIdentifier.Create("dir"),
                null,
                currentDeplocs,
                [],
                true);

            CancellationToken cancellationToken = CancellationToken.None;

            // act
            Result<Guid> result = await ExecuteHandler((sut) =>
            {
                var command = new UpdateDepLocationsCommand(
                    depId.Value,
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
            LocId locId = await BaseAdding.AddLocationToBase(
                LocName.Create("loc1"),
                LocAdress.Create("city1", "street1", 1, "room1"),
                LocTimezone.Create("Europe/Moscow"),
                true);

            LocId locId2 = await BaseAdding.AddLocationToBase(
                LocName.Create("loc2"),
                LocAdress.Create("city2", "street2", 2, "room2"),
                LocTimezone.Create("Europe/Moscow"),
                true);

            DepId newDepId = DepId.NewDepId();

            List<DepartmentLocation> currentDeplocs = [
                DepartmentLocation.Create(newDepId, LocId.GetCurrent(locId.Value))];

            List<DepartmentLocation> newDeplocs = [
                DepartmentLocation.Create(newDepId, LocId.GetCurrent(locId2.Value))];

            DepId depId = await BaseAdding.AddDepartamentToBase(
                newDepId,
                DepName.Create("Director"),
                DepIdentifier.Create("dir"),
                null,
                currentDeplocs,
                [],
                true);

            CancellationToken cancellationToken = CancellationToken.None;

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