using DirectoryService.IntegrationTests.Common;
using DirectoryService.IntegrationTests.Infrastructure;
using DirectoryServices.Application.Departaments.ChangeParent;
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
    public class ChangeDepParentTests : DirectoryBaseTests
    {
        protected AddToBase BaseAdding { get; private set; }

        public ChangeDepParentTests(DirectoryTestWebFactory factory)
            : base(factory)
        {
            BaseAdding = new AddToBase(factory);
        }

        [Fact]
        public async Task ChangeDepParent_with_valid_data_should_succeed()
        {
            // arrange
            CancellationToken cancellationToken = CancellationToken.None;

            Location loc1 = Location.Create(
                LocName.Create("loc1"),
                LocAdress.Create("city1", "street1", 1, "room1"),
                LocTimezone.Create("Europe/Moscow"),
                true);

            Location loc2 = Location.Create(
                LocName.Create("loc2"),
                LocAdress.Create("city2", "street2", 2, "room2"),
                LocTimezone.Create("Europe/Moscow"),
                true);

            await BaseAdding.AddLocationToBase([loc1, loc2], cancellationToken); // локации в базу

            DepId dirDepId = DepId.NewDepId();
            Departament dirDep = Departament.Create(
                dirDepId,
                DepName.Create("Director"),
                DepIdentifier.Create("dir"),
                null,
                [DepartmentLocation.Create(dirDepId, LocId.GetCurrent(loc1.Id.Value))],
                [],
                true);

            DepId hureDepId = DepId.NewDepId();
            Departament hureDep = Departament.Create(
                hureDepId,
                DepName.Create("Human Resources"),
                DepIdentifier.Create("hure"),
                dirDep,
                [DepartmentLocation.Create(hureDepId, LocId.GetCurrent(loc1.Id.Value))],
                [],
                true);

            DepId maitDepId = DepId.NewDepId();
            Departament maitDep = Departament.Create(
                maitDepId,
                DepName.Create("Main IT"),
                DepIdentifier.Create("mait"),
                dirDep,
                [DepartmentLocation.Create(maitDepId, LocId.GetCurrent(loc1.Id.Value))],
                [],
                true);

            DepId learnDepId = DepId.NewDepId();
            Departament learnDep = Departament.Create(
                learnDepId,
                DepName.Create("Learning"),
                DepIdentifier.Create("learn"),
                hureDep,
                [DepartmentLocation.Create(learnDepId, LocId.GetCurrent(loc1.Id.Value))],
                [],
                true);

            DepId deopDepId = DepId.NewDepId();
            Departament deopDep = Departament.Create(
                deopDepId,
                DepName.Create("DevOps"),
                DepIdentifier.Create("deop"),
                maitDep,
                [DepartmentLocation.Create(deopDepId, LocId.GetCurrent(loc2.Id.Value))],
                [],
                true);

            DepId testDepId = DepId.NewDepId();
            Departament testDep = Departament.Create(
                testDepId,
                DepName.Create("Testers"),
                DepIdentifier.Create("test"),
                deopDep,
                [DepartmentLocation.Create(testDepId, LocId.GetCurrent(loc2.Id.Value))],
                [],
                true);

            DepId watchDepId = DepId.NewDepId();
            Departament watchDep = Departament.Create(
                watchDepId,
                DepName.Create("Watchers"),
                DepIdentifier.Create("watch"),
                deopDep,
                [DepartmentLocation.Create(watchDepId, LocId.GetCurrent(loc2.Id.Value))],
                [],
                true);

            await BaseAdding.AddDepartamentToBase([dirDep, hureDep, maitDep, learnDep, deopDep, testDep, watchDep], cancellationToken); // депы в базу

            // act
            Result<int> result = await ExecuteHandler((sut) =>
            {
                var command = new ChangeParentCommand(
                    deopDep.Id.Value,
                    new ChangeParentDto(dirDep.Id.Value));

                return sut.Handle(command, cancellationToken);
            });

            // assert
            await ExecuteInDb(async dbContext =>
            {
                result.IsSuccess.Should().BeTrue();

                var devops = await dbContext.Departaments.Where(dl => dl.Id == deopDep.Id).FirstAsync(cancellationToken);

                devops.Path.Value.Should().Be("dir.deop");
                devops.ParentId.Value.Should().Be(dirDepId.Value);

                var testers = await dbContext.Departaments.Where(dl => dl.Id == testDep.Id).FirstAsync(cancellationToken);

                testers.Path.Value.Should().Be("dir.deop.test");

                var wathers = await dbContext.Departaments.Where(dl => dl.Id == watchDep.Id).FirstAsync(cancellationToken);

                wathers.Path.Value.Should().Be("dir.deop.watch");
            });
        }

        [Fact]
        public async Task ChangeDepParent_with_same_ids_should_failed()
        {
            // arrange
            CancellationToken cancellationToken = CancellationToken.None;

            Location loc1 = Location.Create(
                LocName.Create("loc1"),
                LocAdress.Create("city1", "street1", 1, "room1"),
                LocTimezone.Create("Europe/Moscow"),
                true);

            await BaseAdding.AddLocationToBase([loc1], cancellationToken); // локации в базу

            DepId dirDepId = DepId.NewDepId();
            Departament dirDep = Departament.Create(
                dirDepId,
                DepName.Create("Director"),
                DepIdentifier.Create("dir"),
                null,
                [DepartmentLocation.Create(dirDepId, LocId.GetCurrent(loc1.Id.Value))],
                [],
                true);

            DepId maitDepId = DepId.NewDepId();
            Departament maitDep = Departament.Create(
                maitDepId,
                DepName.Create("Main IT"),
                DepIdentifier.Create("mait"),
                dirDep,
                [DepartmentLocation.Create(maitDepId, LocId.GetCurrent(loc1.Id.Value))],
                [],
                true);

            DepId deopDepId = DepId.NewDepId();
            Departament deopDep = Departament.Create(
                deopDepId,
                DepName.Create("DevOps"),
                DepIdentifier.Create("deop"),
                maitDep,
                [DepartmentLocation.Create(deopDepId, LocId.GetCurrent(loc1.Id.Value))],
                [],
                true);

            await BaseAdding.AddDepartamentToBase([dirDep, maitDep, deopDep], cancellationToken); // депы в базу

            // act
            Result<int> result = await ExecuteHandler((sut) =>
            {
                var command = new ChangeParentCommand(
                    deopDep.Id.Value,
                    new ChangeParentDto(deopDep.Id.Value));

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
        public async Task ChangeDepParent_with_random_dep_id_should_failed()
        {
            // arrange
            CancellationToken cancellationToken = CancellationToken.None;

            Location loc1 = Location.Create(
                LocName.Create("loc1"),
                LocAdress.Create("city1", "street1", 1, "room1"),
                LocTimezone.Create("Europe/Moscow"),
                true);

            await BaseAdding.AddLocationToBase([loc1], cancellationToken); // локации в базу

            DepId dirDepId = DepId.NewDepId();
            Departament dirDep = Departament.Create(
                dirDepId,
                DepName.Create("Director"),
                DepIdentifier.Create("dir"),
                null,
                [DepartmentLocation.Create(dirDepId, LocId.GetCurrent(loc1.Id.Value))],
                [],
                true);

            DepId maitDepId = DepId.NewDepId();
            Departament maitDep = Departament.Create(
                maitDepId,
                DepName.Create("Main IT"),
                DepIdentifier.Create("mait"),
                dirDep,
                [DepartmentLocation.Create(maitDepId, LocId.GetCurrent(loc1.Id.Value))],
                [],
                true);

            await BaseAdding.AddDepartamentToBase([dirDep, maitDep], cancellationToken); // депы в базу

            // act
            Result<int> result = await ExecuteHandler((sut) =>
            {
                var command = new ChangeParentCommand(
                    Guid.NewGuid(),
                    new ChangeParentDto(maitDep.Id.Value));

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
        public async Task ChangeDepParent_with_random_parent_id_should_failed()
        {
            // arrange
            CancellationToken cancellationToken = CancellationToken.None;

            Location loc1 = Location.Create(
                LocName.Create("loc1"),
                LocAdress.Create("city1", "street1", 1, "room1"),
                LocTimezone.Create("Europe/Moscow"),
                true);

            await BaseAdding.AddLocationToBase([loc1], cancellationToken); // локации в базу

            DepId dirDepId = DepId.NewDepId();
            Departament dirDep = Departament.Create(
                dirDepId,
                DepName.Create("Director"),
                DepIdentifier.Create("dir"),
                null,
                [DepartmentLocation.Create(dirDepId, LocId.GetCurrent(loc1.Id.Value))],
                [],
                true);

            DepId maitDepId = DepId.NewDepId();
            Departament maitDep = Departament.Create(
                maitDepId,
                DepName.Create("Main IT"),
                DepIdentifier.Create("mait"),
                dirDep,
                [DepartmentLocation.Create(maitDepId, LocId.GetCurrent(loc1.Id.Value))],
                [],
                true);

            await BaseAdding.AddDepartamentToBase([dirDep, maitDep], cancellationToken); // депы в базу

            // act
            Result<int> result = await ExecuteHandler((sut) =>
            {
                var command = new ChangeParentCommand(
                    maitDep.Id.Value,
                    new ChangeParentDto(Guid.NewGuid()));

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
        public async Task ChangeDepParent_children_id_as_parent_should_failed()
        {
            // arrange
            CancellationToken cancellationToken = CancellationToken.None;

            Location loc1 = Location.Create(
                LocName.Create("loc1"),
                LocAdress.Create("city1", "street1", 1, "room1"),
                LocTimezone.Create("Europe/Moscow"),
                true);

            await BaseAdding.AddLocationToBase([loc1], cancellationToken); // локации в базу

            DepId dirDepId = DepId.NewDepId();
            Departament dirDep = Departament.Create(
                dirDepId,
                DepName.Create("Director"),
                DepIdentifier.Create("dir"),
                null,
                [DepartmentLocation.Create(dirDepId, LocId.GetCurrent(loc1.Id.Value))],
                [],
                true);

            DepId maitDepId = DepId.NewDepId();
            Departament maitDep = Departament.Create(
                maitDepId,
                DepName.Create("Main IT"),
                DepIdentifier.Create("mait"),
                dirDep,
                [DepartmentLocation.Create(maitDepId, LocId.GetCurrent(loc1.Id.Value))],
                [],
                true);

            DepId deopDepId = DepId.NewDepId();
            Departament deopDep = Departament.Create(
                deopDepId,
                DepName.Create("DevOps"),
                DepIdentifier.Create("deop"),
                maitDep,
                [DepartmentLocation.Create(deopDepId, LocId.GetCurrent(loc1.Id.Value))],
                [],
                true);

            await BaseAdding.AddDepartamentToBase([dirDep, maitDep, deopDep], cancellationToken); // депы в базу

            // act
            Result<int> result = await ExecuteHandler((sut) =>
            {
                var command = new ChangeParentCommand(
                    maitDep.Id.Value,
                    new ChangeParentDto(deopDep.Id.Value));

                return sut.Handle(command, cancellationToken);
            });

            // assert
            await ExecuteInDb(async dbContext =>
            {
                result.IsFailure.Should().BeTrue();
                result.Error.Message.Should().NotBeEmpty();
            });
        }

        private async Task<T> ExecuteHandler<T>(Func<ChangeParentHandler, Task<T>> action)
        {
            await using var scope = Services.CreateAsyncScope();

            ChangeParentHandler sut = scope.ServiceProvider.GetRequiredService<ChangeParentHandler>(); // sut - system under test

            return await action(sut);
        }
    }
}