using DirectoryServices.Application.Abstractions;
using DirectoryServices.Application.Locations.Commands.CreateLocation;
using DirectoryServices.Application.Locations.Queries.GetLocationById;
using DirectoryServices.Contracts.Locations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shared.EndpointResult;

namespace DirectoryServices.Presenters
{
    [ApiController]
    [Route("api/[controller]")]
    public class LocationsController : ControllerBase
    {
        private readonly ILogger<LocationsController> _logger;

        public LocationsController(ILogger<LocationsController> logger)
        {
            _logger = logger;
        }

        [HttpPost("create")]
        [ProducesResponseType<Envelope<Guid>>(200)]
        [ProducesResponseType<Envelope>(400)]
        [ProducesResponseType<Envelope>(404)]
        [ProducesResponseType<Envelope>(409)]
        [ProducesResponseType<Envelope>(500)]
        public async Task<EndpointResult<Guid>> Create(
            [FromServices] ICommandHandler<Guid, CreateLocationCommand> handler,
            [FromBody] CreateLocationRequest createLocationDTO,
            CancellationToken cancellationToken)
        {
            var command = new CreateLocationCommand(createLocationDTO);
            return await handler.Handle(command, cancellationToken);
        }

        [HttpGet("{locId}")]
        [ProducesResponseType<Envelope<Guid>>(200)]
        [ProducesResponseType<Envelope>(400)]
        [ProducesResponseType<Envelope>(404)]
        [ProducesResponseType<Envelope>(409)]
        [ProducesResponseType<Envelope>(500)]
        public async Task<ActionResult<GetLocationDto>> GetById(
            [FromRoute] Guid locId,
            [FromServices] GetLocationByIdHandler handler,
            CancellationToken cancellationToken)
        {
            var location = await handler.Handle(new GetLocationByIdQuery(locId), cancellationToken);
            return Ok(location);
        }
    }
}
