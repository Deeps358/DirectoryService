using DirectoryServices.Entities;
using Shared.ResultPattern;

namespace DirectoryServices.Application
{
    public interface IDepartmentLocationRepository
    {
        Task<Result<Guid>> CreateAsync(DepartmentLocation deploc, CancellationToken cancellationToken);
    }
}
