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
        LocId locId = await BaseAdding.AddLocationToBase(
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
    public async Task CreateDepartament_with_valid_parent_id_should_succeed()
    {
        // arrange
        LocId locId = await BaseAdding.AddLocationToBase(
            LocName.Create("loc1"),
            LocAdress.Create("city1", "street1", 1, "room1"),
            LocTimezone.Create("Europe/Moscow"),
            true);

        DepId newDepId = DepId.NewDepId();

        DepId depId = await BaseAdding.AddDepartamentToBase(
            newDepId,
            DepName.Create("Director"),
            DepIdentifier.Create("dir"),
            null,
            [DepartmentLocation.Create(newDepId, locId).Value],
            [],
            true);

        var cancellationToken = CancellationToken.None;

        // act
        var result = await ExecuteHandler((sut) =>
        {
            var command = new CreateDepartamentCommand(
                new CreateDepartamentDto(
                    "Human Resources",
                    "hure",
                    depId.Value,
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
            departament.ParentId.Value.Should().Be(depId.Value);
        });
    }

    [Fact]
    public async Task CreateDepartament_with_duplicate_name_should_fail()
    {
        // arrange
        LocId locId = await BaseAdding.AddLocationToBase(
            LocName.Create("loc1"),
            LocAdress.Create("city1", "street1", 1, "room1"),
            LocTimezone.Create("Europe/Moscow"),
            true);

        DepId newDepId = DepId.NewDepId();

        DepId depId = await BaseAdding.AddDepartamentToBase(
            newDepId,
            DepName.Create("Director"),
            DepIdentifier.Create("dir"),
            null,
            [DepartmentLocation.Create(newDepId, locId).Value],
            [],
            true);

        var cancellationToken = CancellationToken.None;

        // act
        var result = await ExecuteHandler((sut) =>
        {
            var command = new CreateDepartamentCommand(
                new CreateDepartamentDto(
                    "Director",
                    "direc",
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
        LocId locId = await BaseAdding.AddLocationToBase(
            LocName.Create("loc1"),
            LocAdress.Create("city1", "street1", 1, "room1"),
            LocTimezone.Create("Europe/Moscow"),
            true);

        DepId newDepId = DepId.NewDepId();

        DepId depId = await BaseAdding.AddDepartamentToBase(
            newDepId,
            DepName.Create("Director"),
            DepIdentifier.Create("dir"),
            null,
            [DepartmentLocation.Create(newDepId, locId).Value],
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

    [Fact]
    public async Task CreateDepartament_with_incorrect_parent_id_should_fail()
    {
        // arrange
        LocId locId = await BaseAdding.AddLocationToBase(
            LocName.Create("loc1"),
            LocAdress.Create("city1", "street1", 1, "room1"),
            LocTimezone.Create("Europe/Moscow"),
            true);

        DepId newDepId = DepId.NewDepId();

        DepId depId = await BaseAdding.AddDepartamentToBase(
            newDepId,
            DepName.Create("Director"),
            DepIdentifier.Create("dir"),
            null,
            [DepartmentLocation.Create(newDepId, locId).Value],
            [],
            true);

        var cancellationToken = CancellationToken.None;

        // act
        var result = await ExecuteHandler((sut) =>
        {
            var command = new CreateDepartamentCommand(
                new CreateDepartamentDto(
                    "Human Resources",
                    "hure",
                    Guid.NewGuid(),
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

    private async Task<T> ExecuteHandler<T>(Func<CreateDepartamentHandler, Task<T>> action)
    {
        await using var scope = Services.CreateAsyncScope();

        CreateDepartamentHandler sut = scope.ServiceProvider.GetRequiredService<CreateDepartamentHandler>(); // sut - system under test

        return await action(sut);
    }
}
