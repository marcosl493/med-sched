using Application.UseCases.Patient;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Extensions;

namespace WebApi.Endpoints;

public static class PatientEndpoints
{

    public static void MapPatientEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/patients").WithTags("Patients");
        group.MapGet("/{id:guid}", GetByIdAsync)
            .WithName(nameof(GetByIdAsync))
            .Produces<GetPatientResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(nameof(UserRole.PATIENT))
            .CacheOutput(OutputCacheExtensions.GetByIdPolicyName)
            .WithDescription("Consulta pelo Id os dados do paciente.");

        group.MapPost("/", CreatePatient)
            .WithName("CreatePatient")
            .Accepts<CreatePatientCommand>("application/json")
            .Produces<CreatePatientResult>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .WithDescription("Cria um paciente com seus dados, e usuário de login.");
    }
    private static async Task<IResult> CreatePatient([FromBody] CreatePatientCommand request, [FromServices] IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        return result.ToCreatedAtRouteResult(
        routeName: nameof(GetByIdAsync),
        routeValuesFunc: p => new { id = p.Id }
        );
    }
    private static async Task<IResult> GetByIdAsync([FromRoute] Guid id, [FromServices] IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetPatientByIdQuery(id), cancellationToken);
        return result.ToHttpResult();
    }
}
