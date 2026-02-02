using System.Data;
using CSharpFunctionalExtensions;
using DirectoryServices.Application.Database;
using Microsoft.Extensions.Logging;
using Shared.ResultPattern;

namespace DirectoryServices.Infrastructure.Postgres.Database;

public class TransactionScope : ITransactionScope
{
    private readonly IDbTransaction _transaction;
    private readonly ILogger<TransactionScope> _logger;

    public TransactionScope(IDbTransaction transaction, ILogger<TransactionScope> logger)
    {
        _transaction = transaction;
        _logger = logger;
    }

    public UnitResult<Error> Commit()
    {
        try
        {
            _transaction.Commit();
            return UnitResult.Success<Error>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Не удалось сделать commit в транзакции");
            return Error.Failure("database.transaction.commit", ["Не удалось сделать commit в транзакции"]);
        }
    }

    public UnitResult<Error> Rollback()
    {
        try
        {
            _transaction.Rollback();
            return UnitResult.Success<Error>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Не удалось сделать rollback в транзакции");
            return Error.Failure("database.transaction.rollback", ["Не удалось сделать rollback в транзакции"]);
        }
    }

    public void Dispose()
    {
        _transaction.Dispose();
    }
}
