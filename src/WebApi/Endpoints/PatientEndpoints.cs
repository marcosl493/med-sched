using Application.UseCases.Appointment;
using Application.UseCases.Patient;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Authorization;
using WebApi.Dtos;
using WebApi.Extensions;

namespace WebApi.Endpoints;

public static class PatientEndpoints
{

    public static void MapPatientEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/patients").WithTags("Patients");
        group.MapGet("/{id:guid}", GetPatientByIdAsync)
            .WithName(nameof(GetPatientByIdAsync))
            .Produces<GetPatientResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(Policies.PatientSameUser)
            .CacheOutput(OutputCacheExtensions.GetByIdPolicyName)
            .WithDescription("Consulta pelo Id os dados do paciente.");

        group.MapPost("/", CreatePatient)
            .WithName("CreatePatient")
            .Accepts<CreatePatientCommand>("application/json")
            .Produces<CreatePatientResult>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .WithDescription("Cria um paciente com seus dados, e usuário de login.");

        group.MapPost("/{id:guid}/appointments/{scheduleId:guid}", CreateAppointmentAsync)
            .WithName(nameof(CreateAppointmentAsync))
            .Accepts<CreateAppointmentCommand>("application/json")
            .Produces<CreateAppointmentResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .RequireAuthorization(Policies.PatientSameUser)
            .WithDescription("Cria o agendamento de uma consulta para o paciente.");

    }
    private static async Task<IResult> CreatePatient([FromBody] CreatePatientCommand request, [FromServices] IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        return result.ToCreatedAtRouteResult(
        routeName: nameof(GetPatientByIdAsync),
        routeValuesFunc: p => new { id = p.Id }
        );
    }
    private static async Task<IResult> GetPatientByIdAsync([FromRoute] Guid id, [FromServices] IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetPatientByIdQuery(id), cancellationToken);
        return result.ToHttpResult();
    }
    private static async Task<IResult> CreateAppointmentAsync([FromRoute] Guid id,
        [FromRoute] Guid scheduleId,
        [FromBody] CreateAppointmentDto dto,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateAppointmentCommand(id, scheduleId, dto.Reason), cancellationToken);
        return result.ToCreatedAtRouteResult(
       routeName: "GetAppointmentByIdAsync",
       routeValuesFunc: p => new { id = p.Id }
       );
    }
}
