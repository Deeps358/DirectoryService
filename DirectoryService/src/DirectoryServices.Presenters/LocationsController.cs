using DirectoryServices.Application.Locations;
using DirectoryServices.Contracts.Locations;
using DirectoryServices.Entities;
using Microsoft.AspNetCore.Mvc;
using Shared.EndpointResult;

namespace DirectoryServices.Presenters
{
    [ApiController]
    [Route("api/[controller]")]
    public class LocationsController : ControllerBase
    {
        private readonly ILocationsService _locationsService;

        public LocationsController(ILocationsService locationsService)
        {
            _locationsService = locationsService;
        }

        [HttpPost]
        [ProducesResponseType<Envelope<Location>>(200)]
        [ProducesResponseType<Envelope>(400)]
        [ProducesResponseType<Envelope>(404)]
        [ProducesResponseType<Envelope>(409)]
        [ProducesResponseType<Envelope>(500)]
        public async Task<EndpointResult<Location>> CreateLocation([FromBody] CreateLocationDto createLocationDTO, CancellationToken cancellationToken)
        {
            return await _locationsService.Create(createLocationDTO, cancellationToken);
        }
    }
}
