using Application.UseCases.Schedule;
using Domain.Entities;
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
            .ProducesValidationProblem()
            .RequireAuthorization(nameof(UserRole.PATIENT), nameof(UserRole.PHYSICIAN))   
            .WithDescription("Consulta pelo Id um horário de atendimento cadastrado.");

        group.MapGet("/", GetAllAvaliableScheduleAsync)
            .WithName(nameof(GetAllAvaliableScheduleAsync))
            .Produces<IEnumerable<GetScheduleResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .RequireAuthorization(nameof(UserRole.PATIENT))
            .WithDescription("Consulta todos os horários disponíveis para agendamento, com seus respectivos médicos e especialidade.");
    }
    private static async Task<IResult> GetScheduleByIdAsync([FromRoute] Guid id, [FromServices] IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetScheduleCommand(id), cancellationToken);
        return result.ToHttpResult();
    }
    private static async Task<IResult> GetAllAvaliableScheduleAsync
        ([FromQuery] Guid? physicianId,
         [FromQuery] int? skip,
         [FromServices] IMediator mediator,
         CancellationToken cancellationToken,
         [FromQuery] DateTime? startTime = default,
         [FromQuery] int top = 50)
    {
        var result = await mediator.Send(new GetAllAvaliableScheduleQuery(physicianId, startTime, skip, top), cancellationToken);
        return result.ToHttpResult();
    }
}
