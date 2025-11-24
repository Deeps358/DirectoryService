using DirectoryServices.Application.Abstractions;
using DirectoryServices.Application.Departaments.CreateDepartament;
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

        [HttpPost]
        [ProducesResponseType<Envelope<Guid>>(200)]
        [ProducesResponseType<Envelope>(400)]
        [ProducesResponseType<Envelope>(404)]
        [ProducesResponseType<Envelope>(409)]
        [ProducesResponseType<Envelope>(500)]
        public async Task<EndpointResult<Guid>> Create(
            [FromServices] ICommandHandler<Guid, CreateDepartamentCommand> handler,
            [FromBody] CreateDepartamentDto createDepartamentDTO,
            CancellationToken cancellationToken)
        {
            var command = new CreateDepartamentCommand(createDepartamentDTO);
            return await handler.Handle(command, cancellationToken);
        }
    }
}
