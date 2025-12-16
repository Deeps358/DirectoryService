using DirectoryService.IntegrationTests.Infrastructure;
using DirectoryServices.Application.Departaments.CreateDepartament;
using DirectoryServices.Contracts.Departaments;
using DirectoryServices.Entities;
using DirectoryServices.Entities.ValueObjects.Departaments;
using DirectoryServices.Entities.ValueObjects.Locations;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.IntegrationTests.DepartamentTests;

public class CreateDepartamentTests : DirectoryBaseTests
{
    public CreateDepartamentTests(DirectoryTestWebFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task CreateDepartament_with_valid_data_should_succeed()
    {
        // arrange
        LocId locId = await AddLocationToBase(
            LocName.Create("loc1"),
            LocAdress.Create("city1", "street1", 1, "room1"),
            LocTimezone.Create("Europe/Moscow"),
            true);

        var cancellationToken = CancellationToken.None;

        // act
        var result = await ExecuteHandler((sut) =>
        {
            var command = new CreateDepartamentCommand(
                new CreateDepartamentDto(
                    "dep",
                    "dep",
                    null,
                    [locId.Value],
                    true));

            return sut.Handle(command, cancellationToken);
        });

        // assert
        await ExecuteInDb(async dbContext =>
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBe(Guid.Empty);

            var departament = await dbContext.Departaments
                .FirstAsync(d => d.Id == DepId.GetCurrent(result.Value), cancellationToken);

            departament.Should().NotBeNull();
            departament.Id.Value.Should().Be(result.Value);
        });
    }

    [Fact]
    public async Task CreateDepartament_with_duplicate_name_should_fail()
    {
        // arrange
        LocId locId = await AddLocationToBase(
            LocName.Create("loc1"),
            LocAdress.Create("city1", "street1", 1, "room1"),
            LocTimezone.Create("Europe/Moscow"),
            true);

        DepId depId = await AddDepartamentToBase(
            DepId.NewDepId(),
            DepName.Create("Director"),
            DepIdentifier.Create("dir"),
            null,
            [],
            [],
            true);

        var cancellationToken = CancellationToken.None;

        // act
        var result = await ExecuteHandler((sut) =>
        {
            var command = new CreateDepartamentCommand(
                new CreateDepartamentDto(
                    "Director",
                    "dir",
                    null,
                    [locId.Value],
                    true));

            return sut.Handle(command, cancellationToken);
        });

        // assert
        await ExecuteInDb(async dbContext =>
        {
            result.IsSuccess.Should().BeFalse();
            result.Error.Message.Should().NotBeEmpty();
        });
    }

    [Fact]
    public async Task CreateDepartament_with_duplicate_identifier_should_fail()
    {
        // arrange
        LocId locId = await AddLocationToBase(
            LocName.Create("loc1"),
            LocAdress.Create("city1", "street1", 1, "room1"),
            LocTimezone.Create("Europe/Moscow"),
            true);

        DepId depId = await AddDepartamentToBase(
            DepId.NewDepId(),
            DepName.Create("Director"),
            DepIdentifier.Create("dir"),
            null,
            [],
            [],
            true);

        var cancellationToken = CancellationToken.None;

        // act
        var result = await ExecuteHandler((sut) =>
        {
            var command = new CreateDepartamentCommand(
                new CreateDepartamentDto(
                    "Main",
                    "dir",
                    null,
                    [locId.Value],
                    true));

            return sut.Handle(command, cancellationToken);
        });

        // assert
        await ExecuteInDb(async dbContext =>
        {
            result.IsSuccess.Should().BeFalse();
            result.Error.Message.Should().NotBeEmpty();
        });
    }

    private async Task<LocId> AddLocationToBase(LocName locName, LocAdress locAdress, LocTimezone locTimezone, bool isActive)
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

    private async Task<DepId> AddDepartamentToBase(
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

    private async Task<T> ExecuteHandler<T>(Func<CreateDepartamentHandler, Task<T>> action)
    {
        await using var scope = Services.CreateAsyncScope();

        CreateDepartamentHandler sut = scope.ServiceProvider.GetRequiredService<CreateDepartamentHandler>(); // sut - system under test

        return await action(sut);
    }
}
