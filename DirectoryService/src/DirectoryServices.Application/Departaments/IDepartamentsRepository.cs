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

        Task<Result<Departament[]>> GetChildDepsWithLockAsync(DepPath depPath, CancellationToken cancellationToken);

        Task<CSharpFunctionalExtensions.UnitResult<Error>> AddDepLocationsRelationsAsync(
            List<DepartmentLocation> deplocs,
            CancellationToken cancellationToken);

        Task<CSharpFunctionalExtensions.UnitResult<Error>> DeleteLocationsByDepAsync(DepId depId, CancellationToken cancellationToken);

        Task<Result<int>> ChangeParent(string depPath, string curParentPath, string newPath, Guid? parentId, CancellationToken cancellationToken);
    }
}
