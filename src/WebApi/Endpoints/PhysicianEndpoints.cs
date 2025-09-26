using Application.UseCases.Schedule;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Authorization;
using WebApi.Dtos;
using WebApi.Extensions;

namespace WebApi.Endpoints;

public static class PhysicianEndpoints
{
    public static void MapPhysicianEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/physicians").WithTags("Physicians");
        group.MapPost("/{id:guid}/schedules", CreateSchedule)
            .WithName("CreateSchedule")
            .Accepts<CreateScheduleCommand>("application/json")
            .Produces<CreateScheduleResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .RequireAuthorization(Policies.PhysicianSameUser)
            .WithDescription("Cria um horário disponível para um médico.");

        group.MapPut("/{id:guid}/schedules/{scheduleId:guid}", UpdateSchedule)
            .WithName(nameof(UpdateSchedule))
            .Accepts<UpdateScheduleDto>("application/json")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesValidationProblem()
            .RequireAuthorization(Policies.PhysicianSameUser)
            .WithDescription("Atualiza um horário disponível para um médico.");

    }
    private static async Task<IResult> CreateSchedule([FromRoute] Guid id,
        [FromBody] CreateScheduleDto request,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator
            .Send(new CreateScheduleCommand(id, request.StartTime, request.EndTime), cancellationToken);
        return result.ToCreatedAtRouteResult(
        routeName: "GetScheduleByIdAsync",
        routeValuesFunc: p => new { id = p.Id }
        );
    }
    private static async Task<IResult> UpdateSchedule([FromRoute] Guid id,
        [FromRoute] Guid scheduleId,
        [FromBody] UpdateScheduleDto request,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator
            .Send(new UpdateScheduleCommand(scheduleId, id, request.StartTime, request.EndTime), cancellationToken);
        return result.ToHttpResult();
    }
}
