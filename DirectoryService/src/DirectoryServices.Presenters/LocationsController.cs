using DirectoryServices.Application.Locations;
using DirectoryServices.Contracts.Locations;
using DirectoryServices.Presenters.ResponseExtensions;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> CreateLocation([FromBody] CreateLocationDto createLocationDTO, CancellationToken cancellationToken)
        {
            var location = await _locationsService.Create(createLocationDTO, cancellationToken);

            if(location.IsFailure)
            {
                return location.Error.ToErrorResponse();
            }

            return Ok(location);
        }
    }
}
