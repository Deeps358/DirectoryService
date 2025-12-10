using DirectoryServices.Application.Abstractions;
using DirectoryServices.Application.Departaments.ChangeParent;
using DirectoryServices.Application.Departaments.CreateDepartament;
using DirectoryServices.Application.Departaments.UpdateDepLocations;
using DirectoryServices.Contracts.Departaments;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shared.EndpointResult;

namespace DirectoryServices.Presenters
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartamentController : ControllerBase
    {
        private readonly ILogger<DepartamentController> _logger;

        public DepartamentController(ILogger<DepartamentController> logger)
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
            [FromServices] ICommandHandler<Guid, CreateDepartamentCommand> handler,
            [FromBody] CreateDepartamentDto createDepartamentDto,
            CancellationToken cancellationToken)
        {
            var command = new CreateDepartamentCommand(createDepartamentDto);
            return await handler.Handle(command, cancellationToken);
        }

        [HttpPatch("{departmentId}/locations")]
        [ProducesResponseType<Envelope<Guid>>(200)]
        [ProducesResponseType<Envelope>(400)]
        [ProducesResponseType<Envelope>(404)]
        [ProducesResponseType<Envelope>(409)]
        [ProducesResponseType<Envelope>(500)]
        public async Task<EndpointResult<Guid>> UpdateLocations(
            [FromRoute] Guid departmentId,
            [FromServices] ICommandHandler<Guid, UpdateDepLocationsCommand> handler,
            [FromBody] UpdateDepLocationsDto updateDepLocationsDto,
            CancellationToken cancellationToken)
        {
            var command = new UpdateDepLocationsCommand(departmentId, updateDepLocationsDto);
            return await handler.Handle(command, cancellationToken);
        }

        [HttpPut("{departmentId}/parent")]
        [ProducesResponseType<Envelope<Guid>>(200)]
        [ProducesResponseType<Envelope>(400)]
        [ProducesResponseType<Envelope>(404)]
        [ProducesResponseType<Envelope>(409)]
        [ProducesResponseType<Envelope>(500)]
        public async Task<EndpointResult<Guid>> ChangeParent(
            [FromRoute] Guid departmentId,
            [FromServices] ICommandHandler<Guid, ChangeParentCommand> handler,
            [FromBody] ChangeParentDto updateDepLocationsDto,
            CancellationToken cancellationToken)
        {
            var command = new ChangeParentCommand(departmentId, updateDepLocationsDto);
            return await handler.Handle(command, cancellationToken);
        }
    }
}
