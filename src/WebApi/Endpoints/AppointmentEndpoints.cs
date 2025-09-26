using Application.UseCases.Patient;
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
            .Produces<GetPatientResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(Policies.Patient)
            .WithDescription("Consulta agendamento de atendimento pelo Id.");

    }
    private static async Task<IResult> GetAppointmentByIdAsync([FromRoute] Guid id, [FromServices] IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetPatientByIdQuery(id), cancellationToken);
        return result.ToHttpResult();
    }
}
