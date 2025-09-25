using Application.Interfaces.Repositories;
using FluentResults;
using MediatR;

namespace Application.UseCases.Patient;

internal class GetPatientHandler(IPatientRepository repository) : IRequestHandler<GetPatientByIdQuery, Result<GetPatientResult>>
{
    public async Task<Result<GetPatientResult>> Handle(GetPatientByIdQuery request, CancellationToken cancellationToken)
    {
        var patient = await repository.GetPatientByIdAsync(request.Id, cancellationToken);

        if (patient is null)
            return Result.Ok();

        return Result.Ok(new GetPatientResult(patient.Id, patient.User.Name, patient.User.Email, patient.DateOfBirth))
            .WithSuccess("Found");
    }
}

public record GetPatientByIdQuery(Guid Id) : IRequest<Result<GetPatientResult>>;

public record GetPatientResult(Guid Id, string Name, string Email, DateTime DateOfBirth);