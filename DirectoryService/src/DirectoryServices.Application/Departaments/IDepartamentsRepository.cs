using DirectoryServices.Entities;
using DirectoryServices.Entities.ValueObjects.Departaments;
using Shared.ResultPattern;

namespace DirectoryServices.Application.Departaments
{
    public interface IDepartamentsRepository
    {
        Task<Result<Guid>> CreateAsync(Departament departament, CancellationToken cancellationToken);

        Task<Result<Departament[]>> GetByIdAsync(Guid[] ids, CancellationToken cancellationToken);

        Task<Result<Departament>> GetByIdWithLockAsync(Guid id, CancellationToken cancellationToken);

        Task<CSharpFunctionalExtensions.UnitResult<Error>> GetChildDepsWithLockAsync(DepPath depPath, CancellationToken cancellationToken);

        Task<CSharpFunctionalExtensions.UnitResult<Error>> AddDepLocationsRelationsAsync(
            List<DepartmentLocation> deplocs,
            CancellationToken cancellationToken);

        Task<CSharpFunctionalExtensions.UnitResult<Error>> DeleteLocationsByDepAsync(DepId depId, CancellationToken cancellationToken);

        Task<Result<int>> MoveDepWithChildernsAsync(DepPath depPath, DepPath curParentPath, DepPath newPath, DepId? parentId, CancellationToken cancellationToken);

        Task<CSharpFunctionalExtensions.UnitResult<Error>> SoftDeleteWithChildrensAsync(DepPath oldPath, DepPath deletedPath, CancellationToken cancellationToken);

        Task<Result<int>> DeactivateLocationsWithDepId(Guid depId, CancellationToken cancellationToken);

        Task<Result<int>> DeactivatePositionsWithDepId(Guid depId, CancellationToken cancellationToken);

        Task<Departament[]?> GetDepsChildrensById(DepId depId, CancellationToken cancellationToken);

        Task<Departament[]> GetDepsForHardDelete(CancellationToken cancellationToken);

        Task<CSharpFunctionalExtensions.UnitResult<Error>> HardDeleteDep(DepId[] ids, CancellationToken cancellationToken);
    }
}
