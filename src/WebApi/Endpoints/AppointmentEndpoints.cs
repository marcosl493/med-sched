using Application.UseCases.Appointment;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Authorization;
using WebApi.Extensions;

namespace WebApi.Endpoints;

public static class AppointmentEndpoints
{
    public static void MapAppointmentEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/appointments").WithTags("Appointments");
        group.MapGet("/{id:guid}", GetAppointmentByIdAsync)
            .WithName(nameof(GetAppointmentByIdAsync))
            .Produces<GetAppointmentResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(Policies.Patient, Policies.Physician)
            .WithDescription("Consulta agendamento de atendimento pelo Id.");
        group.MapGet("/", GetAppointmentsAsync)
            .WithName(nameof(GetAppointmentsAsync))
            .Produces<GetAllAppointmentResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent)
            .WithDescription("Consulta agendamento de atendimento pelo Id.");

    }
    private static async Task<IResult> GetAppointmentsAsync
        (
        [FromQuery] Guid? physicianId,
        [FromQuery] Guid? patientId,
        [FromQuery] int? skip,
        [FromQuery] AppointmentStatus? status,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken,
        [FromQuery] int top = 50)
    {
        var result = await mediator.Send(new GetAllAppointmentQuery(top, physicianId, status, patientId, skip), cancellationToken);
        return result.ToHttpResult();
    }
    private static async Task<IResult> GetAppointmentByIdAsync([FromRoute] Guid id, [FromServices] IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetAppointmentQuery(id), cancellationToken);
        return result.ToHttpResult();
    }
}
