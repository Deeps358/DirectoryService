namespace DirectoryServices.Entities.ValueObjects.Positions
{
    public record PosId
    {
        private PosId()
        {
            // ефкор не ругайся
        }

        private PosId(Guid value)
        {
            Value = value;
        }

        public Guid Value { get; } = Guid.Empty;

        public static PosId NewPosId() => new(Guid.NewGuid());
        public static PosId GetCurrent(Guid idFromDB) => new(idFromDB);
    }
}
