using DirectoryServices.Entities;
using DirectoryServices.Entities.ValueObjects.Departaments;
using Shared.ResultPattern;

namespace DirectoryServices.Application.Departaments
{
    public interface IDepartamentsRepository
    {
        Task<Result<Guid>> CreateAsync(Departament departament, CancellationToken cancellationToken);

        Task<Result<Departament[]>> GetByIdAsync(Guid[] ids, CancellationToken cancellationToken);
    }
}
