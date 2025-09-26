using Application.UseCases.Patient;
using Application.UseCases.Schedule;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
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
            .RequireAuthorization(nameof(UserRole.PHYSICIAN))
            .WithDescription("Cria um horário disponível para um médico.");
    }
    private static async Task<IResult> CreateSchedule([FromRoute]Guid id, [FromBody]CreateScheduleDto request, [FromServices] IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateScheduleCommand(id, request.StartTime, request.EndTime), cancellationToken);
        return result.ToCreatedAtRouteResult(
        routeName: "GetScheduleByIdAsync",
        routeValuesFunc: p => new { id = p.Id }
        );
    }
}
