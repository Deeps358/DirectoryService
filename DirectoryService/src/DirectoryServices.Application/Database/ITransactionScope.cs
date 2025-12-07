using CSharpFunctionalExtensions;
using Shared.ResultPattern;

namespace DirectoryServices.Application.Database;

public interface ITransactionScope : IDisposable
{
    UnitResult<Error> Commit();

    UnitResult<Error> Rollback();
}
