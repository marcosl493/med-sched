using Application.Interfaces.Repositories;
using Domain.Entities;
using FluentResults;
using MediatR;

namespace Application.UseCases.Appointment;

internal class GetAppointmentHandler(IAppointmentRepository repository) : IRequestHandler<GetAppointmentQuery, Result<GetAppointmentResponse>>
{
    public async Task<Result<GetAppointmentResponse>> Handle(GetAppointmentQuery request, CancellationToken cancellationToken)
    {
        var result = await repository.GetAppointmentByIdAsync(request.Id, cancellationToken);
        if (result == null)
            return Result.Ok();

        var response = new GetAppointmentResponse
            (
                result.Id,
                result.PatientId,
                result.Schedule.PhysicianId,
                result.Status,
                result.Schedule.StartTime,
                result.Schedule.EndTime,
                result.CreatedAt
            );
        return Result.Ok(response);
    }
}

public record GetAppointmentResponse
    (
        Guid Id,
        Guid PatientId,
        Guid PhysicianId,
        AppointmentStatus Status,
        DateTime StartTime,
        DateTime EndTime,
        DateTime CreatedAt);

public record GetAppointmentQuery(Guid Id) : IRequest<Result<GetAppointmentResponse>>;