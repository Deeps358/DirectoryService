using DirectoryServices.Entities;
using Shared.ResultPattern;

namespace DirectoryServices.Application.Departaments
{
    public interface IDepartamentsRepository
    {
        Task<Result<Guid>> CreateAsync(Departament departament, CancellationToken cancellationToken);

        Task<Result<Departament>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    }
}
