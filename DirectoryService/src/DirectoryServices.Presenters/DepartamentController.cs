using DirectoryServices.Application.Abstractions;
using DirectoryServices.Application.Departaments.Commands.ChangeParent;
using DirectoryServices.Application.Departaments.Commands.CreateDepartament;
using DirectoryServices.Application.Departaments.Commands.SoftDelete;
using DirectoryServices.Application.Departaments.Commands.UpdateDepLocations;
using DirectoryServices.Application.Departaments.Queries.GetRoots;
using DirectoryServices.Application.Departaments.Queries.GetRoots.GetChildrensById;
using DirectoryServices.Application.Departaments.Queries.GetTopFiveByPositions;
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
        public async Task<EndpointResult<int>> ChangeParent(
            [FromRoute] Guid departmentId,
            [FromServices] ICommandHandler<int, ChangeParentCommand> handler,
            [FromBody] ChangeParentDto updateDepLocationsDto,
            CancellationToken cancellationToken)
        {
            var command = new ChangeParentCommand(departmentId, updateDepLocationsDto);
            return await handler.Handle(command, cancellationToken);
        }

        [HttpGet("top-positions")]
        [ProducesResponseType<Envelope<GetTopFiveByPositionsDto>>(200)]
        [ProducesResponseType<Envelope>(400)]
        [ProducesResponseType<Envelope>(404)]
        [ProducesResponseType<Envelope>(409)]
        [ProducesResponseType<Envelope>(500)]
        public async Task<ActionResult<GetTopFiveByPositionsDto>> GetTopFive(
            [FromServices] GetTopFiveByPositionsHandler handler,
            CancellationToken cancellationToken)
        {
            var deps = await handler.Handle(new GetTopFiveByPositionsQuery(), cancellationToken);
            return Ok(deps);
        }

        [HttpGet("roots")]
        [ProducesResponseType<Envelope<GetTopFiveByPositionsDto>>(200)]
        [ProducesResponseType<Envelope>(400)]
        [ProducesResponseType<Envelope>(404)]
        [ProducesResponseType<Envelope>(409)]
        [ProducesResponseType<Envelope>(500)]
        public async Task<ActionResult<GetRootsWithChildrensDto>> GetRootsWithChildrens(
            [FromQuery] GetRootsWithChildrensRequest request,
            [FromServices] GetRootsWithChildrensHandler handler,
            CancellationToken cancellationToken)
        {
            var deps = await handler.Handle(new GetRootsWithChildrensQuery(request), cancellationToken);
            return Ok(deps);
        }

        [HttpGet("{parentId}/childrens")]
        [ProducesResponseType<Envelope<GetTopFiveByPositionsDto>>(200)]
        [ProducesResponseType<Envelope>(400)]
        [ProducesResponseType<Envelope>(404)]
        [ProducesResponseType<Envelope>(409)]
        [ProducesResponseType<Envelope>(500)]
        public async Task<ActionResult<DepartamentDto[]>> GetChildrensById(
            [FromRoute] Guid parentId,
            [FromQuery] GetChildrensByIdRequest request,
            [FromServices] GetChildrensByIdHandler handler,
            CancellationToken cancellationToken)
        {
            var deps = await handler.Handle(new GetChildrensByIdQuery(request, parentId), cancellationToken);
            return Ok(deps);
        }

        [HttpDelete("{depId}")]
        [ProducesResponseType<Envelope<GetTopFiveByPositionsDto>>(200)]
        [ProducesResponseType<Envelope>(400)]
        [ProducesResponseType<Envelope>(404)]
        [ProducesResponseType<Envelope>(409)]
        [ProducesResponseType<Envelope>(500)]
        public async Task<ActionResult<Guid>> SoftDelete(
            [FromRoute] Guid depId,
            [FromServices] SoftDeleteHandler handler,
            CancellationToken cancellationToken)
        {
            var deletedDep = await handler.Handle(new SoftDeleteCommand(depId), cancellationToken);
            return Ok(deletedDep);
        }
    }
}
