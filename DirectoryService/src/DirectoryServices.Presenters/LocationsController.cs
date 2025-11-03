using DirectoryServices.Application.Abstractions;
using DirectoryServices.Application.Locations;
using DirectoryServices.Application.Locations.CreateLocation;
using DirectoryServices.Contracts.Locations;
using DirectoryServices.Entities;
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

        [HttpPost]
        [ProducesResponseType<Envelope<Location>>(200)]
        [ProducesResponseType<Envelope>(400)]
        [ProducesResponseType<Envelope>(404)]
        [ProducesResponseType<Envelope>(409)]
        [ProducesResponseType<Envelope>(500)]
        public async Task<EndpointResult<Location>> Create(
            [FromServices] ICommandHandler<Location, CreateLocationCommand> handler,
            [FromBody] CreateLocationDto createLocationDTO,
            CancellationToken cancellationToken)
        {
            var command = new CreateLocationCommand(createLocationDTO);
            return await handler.Handle(command, cancellationToken);
        }
    }
}
