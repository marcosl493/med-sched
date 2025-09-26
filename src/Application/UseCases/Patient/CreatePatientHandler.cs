using Application.Interfaces.Repositories;
using FluentResults;
using MediatR;
namespace Application.UseCases.Patient;

public class CreatePatientHandler(IPatientRepository patientRepository) : IRequestHandler<CreatePatientCommand, Result<CreatePatientResult>>
{
    public async Task<Result<CreatePatientResult>> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
    {
        var patient = new Domain.Entities.Patient(request.DateOfBirth, request.Name, request.Email, request.Password);
        await patientRepository.CreatePatientAsync(patient, cancellationToken);

        return Result.Ok(new CreatePatientResult(patient.Id))
                    .WithSuccess("Created");
    }
}

public record CreatePatientCommand(string Name, string Email, string Password, DateTime DateOfBirth) : IRequest<Result<CreatePatientResult>>;

public record CreatePatientResult(Guid Id);