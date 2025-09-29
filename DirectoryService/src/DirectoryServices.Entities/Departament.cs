using CSharpFunctionalExtensions;
using System.Text.RegularExpressions;

namespace DirectoryServices.Entities
{

    public partial class Departament
    {
        private readonly List<Departament> _childrens = [];
        private readonly List<Location> _locations = [];
        private readonly List<Position> _positions = [];

        private Departament(
            DepName name,
            DepIdentifier identifier,
            DepPath path,
            Departament? parent,
            short depth,
            IEnumerable<Location> locations,
            IEnumerable<Position> positions,
            bool isActive)
        {
            Id = Guid.NewGuid();
            Name = name;
            Identifier = identifier;
            Path = path;
            Parent = parent;
            Depth = depth;
            _locations = locations.ToList();
            _positions = positions.ToList();
            IsActive = isActive;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public Guid Id { get; private set; }

        public DepName Name { get; private set; }

        public DepIdentifier Identifier { get; private set; }

        public Departament? Parent { get; private set; }

        public IReadOnlyList<Departament> Childrens => _childrens;

        public DepPath Path { get; private set; }

        public short Depth { get; private set; }

        public IReadOnlyList<Location> Locations => _locations;

        public IReadOnlyList<Position> Positions => _positions;

        public bool IsActive { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public DateTime UpdatedAt { get; private set; }

        public static Result<Departament> Create(
            DepName name,
            DepIdentifier identifier,
            DepPath path,
            Departament? parent,
            IEnumerable<Location> locations,
            IEnumerable<Position> positions,
            bool isActive)
        {
            // логика записи глубины отдела
            short depth = Convert.ToInt16(parent?.Depth + 1 ?? 1);

            // передача в конструктор
            var departament = new Departament(name, identifier, path, parent, depth, locations, positions, isActive);

            return Result.Success(departament);
        }

        public Result<Departament> Rename(DepName name)
        {
            if (string.IsNullOrWhiteSpace(name.Value) || name.Value.Length < 3 || name.Value.Length > 150)
            {
                return Result.Failure<Departament>("Название отдела должно быть 3-150 символов!");
            }

            Name = name;
            UpdatedAt = DateTime.UtcNow;

            return Result.Success(this);
        }

        public Result<Departament> ChangeIdentidier(DepIdentifier identifier)
        {
            // валидация идентификатора
            if (string.IsNullOrWhiteSpace(identifier.Value) || identifier.Value.Length < 2 || identifier.Value.Length > 10)
            {
                return Result.Failure<Departament>("Идентификатор отдела должно быть 2-10 символов!");
            }

            if (!Regex.IsMatch(identifier.Value, @"^[a-z\-]+$"))
            {
                return Result.Failure<Departament>("В идентификаторе допускаются только латиница в нижнем регистре и дефисы");
            }

            Identifier = identifier;
            UpdatedAt = DateTime.UtcNow;

            return Result.Success(this);
        }

        public Result<Departament> AddLocations(IEnumerable<Location> locations)
        {
            // проверка списка локаций
            if (locations == null || !locations.Any())
            {
                return Result.Failure<Departament>("Список локаций не должен быть пустым!");
            }

            _locations.Concat(locations);
            UpdatedAt = DateTime.UtcNow;

            return Result.Success(this);
        }

        public Result<Departament> AddPositions(IEnumerable<Position> positions)
        {
            // проверка списка позиций
            if (positions == null || !positions.Any())
            {
                return Result.Failure<Departament>("Список позиций не должен быть пустым!");
            }

            _positions.Concat(positions);
            UpdatedAt = DateTime.UtcNow;

            return Result.Success(this);
        }
    }
}
