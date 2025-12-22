using DirectoryServices.Application.Abstractions;
using DirectoryServices.Application.Positions.Commands.CreatePosition;
using DirectoryServices.Contracts.Positions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shared.EndpointResult;

namespace DirectoryServices.Presenters
{
    [ApiController]
    [Route("api/[controller]")]
    public class PositionsController : ControllerBase
    {
        private readonly ILogger<PositionsController> _logger;

        public PositionsController(ILogger<PositionsController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType<Envelope<Guid>>(200)]
        [ProducesResponseType<Envelope>(400)]
        [ProducesResponseType<Envelope>(404)]
        [ProducesResponseType<Envelope>(409)]
        [ProducesResponseType<Envelope>(500)]
        public async Task<EndpointResult<Guid>> Create(
            [FromServices] ICommandHandler<Guid, CreatePositionCommand> handler,
            [FromBody] CreatePositionDto createPositionDTO,
            CancellationToken cancellationToken)
        {
            var command = new CreatePositionCommand(createPositionDTO);
            return await handler.Handle(command, cancellationToken);
        }
    }
}
