namespace DirectoryServices.Entities.ValueObjects.Locations
{
    public record LocId
    {
        private LocId()
        {
            // ефкор не ругайся
        }

        private LocId(Guid value)
        {
            Value = value;
        }

        public Guid Value { get; } = Guid.Empty;

        public static LocId NewLocId() => new(Guid.NewGuid());
        public static LocId GetCurrent(Guid idFromDB) => new(idFromDB);
    }
}
