namespace DirectoryServices.Application.Abstractions
{
    public interface IQuery;

    public interface IQueryHandler<TResponce, in TQuery>
        where TQuery : IQuery
    {
        Task<TResponce?> Handle(TQuery query, CancellationToken cancellationToken);
    }
}