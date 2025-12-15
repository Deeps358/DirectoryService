using DirectoryService.IntegrationTests.Common;
using DirectoryServices.Application.Departaments.CreateDepartament;
using DirectoryServices.Contracts.Departaments;
using DirectoryServices.Entities;
using DirectoryServices.Entities.ValueObjects.Departaments;
using DirectoryServices.Entities.ValueObjects.Locations;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.IntegrationTests.Departament;

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
        LocId locId = await CreateLocation();
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

            Console.WriteLine(result.Value);

            var departament = await dbContext.Departaments
                .FirstAsync(d => d.Id == DepId.GetCurrent(result.Value), cancellationToken);

            departament.Should().NotBeNull();
            departament.Id.Value.Should().Be(result.Value);

            Console.WriteLine(departament.Name);
        });
    }

    private async Task<LocId> CreateLocation()
    {
        await using AsyncServiceScope initializerScope = Services.CreateAsyncScope();

        return await ExecuteInDb(async dbContext =>
        {
            Location location = Location.Create(
            LocName.Create("loc"),
            LocAdress.Create("city", "street", 1, "room"),
            LocTimezone.Create("Europe/Moscow"),
            true);

            dbContext.Locations.Add(location);
            await dbContext.SaveChangesAsync();

            return location.Id;
        });
    }

    private async Task<T> ExecuteHandler<T>(Func<CreateDepartamentHandler, Task<T>> action)
    {
        await using var scope = Services.CreateAsyncScope();

        CreateDepartamentHandler sut = scope.ServiceProvider.GetRequiredService<CreateDepartamentHandler>(); // sut - system under test

        return await action(sut);
    }
}
