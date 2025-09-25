using Application.UseCases.Schedule;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Extensions;

namespace WebApi.Endpoints;

public static class ScheduleEndpoints
{
    public static void MapScheduleEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/schedules").WithTags("Schedules");
        group.MapGet("/{id:guid}", GetScheduleByIdAsync)
            .WithName(nameof(GetScheduleByIdAsync))
            .Produces<GetScheduleResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesValidationProblem();
    }
    private static async Task<IResult> GetScheduleByIdAsync([FromRoute] Guid id, [FromServices] IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetScheduleCommand(id), cancellationToken);
        return result.ToHttpResult();
    }
}
