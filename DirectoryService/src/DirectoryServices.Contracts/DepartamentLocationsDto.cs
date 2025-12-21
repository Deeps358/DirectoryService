using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DirectoryServices.Contracts
{
    public record DepartamentLocationsDto
    {
        public Guid DepartamentId { get; init; }

        public Guid LocationId { get; init; }

        public DateTime CreatedAt { get; init; }

        public DateTime UpdatedAt { get; init; }
    }
}