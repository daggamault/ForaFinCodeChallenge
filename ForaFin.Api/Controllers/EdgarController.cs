using ForaFin.Api.CQRS.Commands;
using ForaFin.Api.CQRS.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ForaFin.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EdgarController(
    ISender sender,
    ILogger<EdgarController> logger
)
    : ControllerBase
{
    [HttpPost("refresh")]
    public async Task<ActionResult> RefreshEdgarCompanies(
        CancellationToken cancellationToken
    )
    {
        try
        {
            await sender
                .Send(new RefreshEdgarCompaniesCommand(), cancellationToken);
            return Ok();
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(500);
        }
    }

    [HttpGet]
    public async Task<ActionResult> FindCompanies(
        [FromQuery] string? name,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var response = await sender
                .Send(new FindCompaniesQuery(name), cancellationToken);
            return Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(500);
        }
    }
}