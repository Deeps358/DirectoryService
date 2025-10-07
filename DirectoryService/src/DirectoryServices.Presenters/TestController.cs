using CSharpFunctionalExtensions;
using DirectoryServices.Entities;
using DirectoryServices.Entities.ValueObjects.Departaments;
using DirectoryServices.Entities.ValueObjects.Locations;
using DirectoryServices.Entities.ValueObjects.Positions;
using Microsoft.AspNetCore.Mvc;
using static DirectoryServices.Entities.Departament;
using static DirectoryServices.Entities.Location;
using static DirectoryServices.Entities.Position;

namespace DirectoryServices.Presenters
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        [HttpPost("CreateDep")]
        public IActionResult CreateDep(
            string name,
            string identifier,
            Guid? parent,
            [FromRoute] Guid[] locations,
            [FromRoute] Guid[] positions,
            bool isActive)
        {
            var newDepName = DepName.Create(name);
            if(newDepName.IsFailure)
            {
                return BadRequest(newDepName.Error);
            }

            var newDepIdentifier = DepIdentifier.Create(identifier);
            if(newDepIdentifier.IsFailure)
            {
                return BadRequest(newDepIdentifier.Error);
            }

            var newDepPath = DepPath.Create(null, newDepIdentifier.Value);
            if(newDepPath.IsFailure)
            {
                return BadRequest(newDepPath.Error);
            }

            // тут будет взятие отдела-родителя по id
            // тут будет взятие списка локаций по id
            // тут будет взятие списка позиций по id

            Result<Departament> depResult = Create(
                newDepName.Value,
                newDepIdentifier.Value,
                newDepPath.Value,
                null,
                null,
                null,
                isActive);

            if (depResult.IsFailure)
            {
                return BadRequest(depResult.Error);
            }

            return Ok(depResult.Value);
        }

        [HttpPost("CreateLocation")]
        public IActionResult CreateLocation(
            string name,
            string city,
            string street,
            string timezone,
            bool isActive)
        {
            var newLocName = LocName.Create(name);
            if (newLocName.IsFailure)
            {
                return BadRequest(newLocName.Error);
            }

            var newLocAdress = LocAdress.Create(city, street);
            if (newLocAdress.IsFailure)
            {
                return BadRequest(newLocAdress.Error);
            }

            var newTimeZone = LocTimezone.Create(timezone);
            if (newTimeZone.IsFailure)
            {
                return BadRequest(newTimeZone.Error);
            }

            Result<Location> locResult = Create(
                newLocName.Value,
                newLocAdress.Value,
                newTimeZone.Value,
                isActive);

            if (locResult.IsFailure)
            {
                return BadRequest(locResult.Error);
            }

            return Ok(locResult.Value);
        }

        [HttpPost("CreatePosition")]
        public IActionResult CreatePosition(
            string name,
            string? description,
            bool isActive)
        {
            var newPosName = PosName.Create(name);
            if (newPosName.IsFailure)
            {
                return BadRequest(newPosName.Error);
            }

            var newPosDescr = PosDescription.Create(description);
            if (newPosDescr.IsFailure)
            {
                return BadRequest(newPosDescr.Error);
            }

            Result<Position> posResult = Create(
                newPosName.Value,
                newPosDescr.Value,
                isActive);

            if (posResult.IsFailure)
            {
                return BadRequest(posResult.Error);
            }

            return Ok(posResult.Value);
        }
    }
}
