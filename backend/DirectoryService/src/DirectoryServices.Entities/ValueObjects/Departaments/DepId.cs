namespace DirectoryServices.Entities.ValueObjects.Departaments
{
    public record DepId
    {
        private DepId()
        {
            // ефкор не ругайся
        }

        private DepId(Guid value)
        {
            Value = value;
        }

        public Guid Value { get; } = Guid.Empty;

        public static DepId NewDepId() => new(Guid.NewGuid());
        public static DepId GetCurrent(Guid idFromDB) => new(idFromDB);
    }
}
