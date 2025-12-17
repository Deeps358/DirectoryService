using DirectoryService.IntegrationTests.Common;
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
    protected AddToBase BaseAdding { get; private set; }

    public CreateDepartamentTests(DirectoryTestWebFactory factory)
        : base(factory)
    {
        BaseAdding = new AddToBase(factory);
    }

    [Fact]
    public async Task CreateDepartament_with_valid_data_should_succeed()
    {
        // arrange
        Location loc = Location.Create(
            LocName.Create("loc1"),
            LocAdress.Create("city1", "street1", 1, "room1"),
            LocTimezone.Create("Europe/Moscow"),
            true);

        var cancellationToken = CancellationToken.None;

        await BaseAdding.AddLocationToBase([loc], cancellationToken); // локации в базу

        // act
        var result = await ExecuteHandler((sut) =>
        {
            var command = new CreateDepartamentCommand(
                new CreateDepartamentDto(
                    "dep",
                    "dep",
                    null,
                    [loc.Id.Value],
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
    public async Task CreateDepartament_with_valid_parent_id_should_succeed()
    {
        // arrange
        var cancellationToken = CancellationToken.None;

        Location loc = Location.Create(
            LocName.Create("loc1"),
            LocAdress.Create("city1", "street1", 1, "room1"),
            LocTimezone.Create("Europe/Moscow"),
            true);

        await BaseAdding.AddLocationToBase([loc], cancellationToken); // локации в базу

        DepId newDepId = DepId.NewDepId();

        Departament dep = Departament.Create(
            newDepId,
            DepName.Create("Director"),
            DepIdentifier.Create("dir"),
            null,
            [DepartmentLocation.Create(newDepId, loc.Id).Value],
            [],
            true);

        await BaseAdding.AddDepartamentToBase([dep], cancellationToken); // депы в базу

        // act
        var result = await ExecuteHandler((sut) =>
        {
            var command = new CreateDepartamentCommand(
                new CreateDepartamentDto(
                    "Human Resources",
                    "hure",
                    dep.Id.Value,
                    [loc.Id.Value],
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
            departament.ParentId.Value.Should().Be(dep.Id.Value);
        });
    }

    [Fact]
    public async Task CreateDepartament_with_duplicate_name_should_fail()
    {
        // arrange
        var cancellationToken = CancellationToken.None;

        Location loc = Location.Create(
            LocName.Create("loc1"),
            LocAdress.Create("city1", "street1", 1, "room1"),
            LocTimezone.Create("Europe/Moscow"),
            true);

        await BaseAdding.AddLocationToBase([loc], cancellationToken); // локации в базу

        DepId newDepId = DepId.NewDepId();

        Departament dep = Departament.Create(
            newDepId,
            DepName.Create("Director"),
            DepIdentifier.Create("dir"),
            null,
            [DepartmentLocation.Create(newDepId, loc.Id).Value],
            [],
            true);

        await BaseAdding.AddDepartamentToBase([dep], cancellationToken); // депы в базу

        // act
        var result = await ExecuteHandler((sut) =>
        {
            var command = new CreateDepartamentCommand(
                new CreateDepartamentDto(
                    "Director",
                    "direc",
                    null,
                    [loc.Id.Value],
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
        var cancellationToken = CancellationToken.None;

        Location loc = Location.Create(
            LocName.Create("loc1"),
            LocAdress.Create("city1", "street1", 1, "room1"),
            LocTimezone.Create("Europe/Moscow"),
            true);

        await BaseAdding.AddLocationToBase([loc], cancellationToken); // локации в базу

        DepId newDepId = DepId.NewDepId();

        Departament dep = Departament.Create(
            newDepId,
            DepName.Create("Director"),
            DepIdentifier.Create("dir"),
            null,
            [DepartmentLocation.Create(newDepId, loc.Id).Value],
            [],
            true);

        await BaseAdding.AddDepartamentToBase([dep], cancellationToken); // депы в базу

        // act
        var result = await ExecuteHandler((sut) =>
        {
            var command = new CreateDepartamentCommand(
                new CreateDepartamentDto(
                    "Main",
                    "dir",
                    null,
                    [loc.Id.Value],
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
    public async Task CreateDepartament_with_incorrect_parent_id_should_fail()
    {
        // arrange
        var cancellationToken = CancellationToken.None;

        Location loc = Location.Create(
            LocName.Create("loc1"),
            LocAdress.Create("city1", "street1", 1, "room1"),
            LocTimezone.Create("Europe/Moscow"),
            true);

        await BaseAdding.AddLocationToBase([loc], cancellationToken); // локации в базу

        DepId newDepId = DepId.NewDepId();

        Departament dep = Departament.Create(
            newDepId,
            DepName.Create("Director"),
            DepIdentifier.Create("dir"),
            null,
            [DepartmentLocation.Create(newDepId, loc.Id).Value],
            [],
            true);

        await BaseAdding.AddDepartamentToBase([dep], cancellationToken); // депы в базу

        // act
        var result = await ExecuteHandler((sut) =>
        {
            var command = new CreateDepartamentCommand(
                new CreateDepartamentDto(
                    "Human Resources",
                    "hure",
                    Guid.NewGuid(),
                    [loc.Id.Value],
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

    private async Task<T> ExecuteHandler<T>(Func<CreateDepartamentHandler, Task<T>> action)
    {
        await using var scope = Services.CreateAsyncScope();

        CreateDepartamentHandler sut = scope.ServiceProvider.GetRequiredService<CreateDepartamentHandler>(); // sut - system under test

        return await action(sut);
    }
}
