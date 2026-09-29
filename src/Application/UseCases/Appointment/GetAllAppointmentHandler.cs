using Application.Interfaces.Repositories;
using Application.UseCases.Patient;
using Domain.Entities;
using FluentResults;
using MediatR;

namespace Application.UseCases.Appointment;

internal class GetAllAppointmentHandler(IAppointmentRepository repository) : IRequestHandler<GetAllAppointmentQuery, Result<GetAllAppointmentResponse>>
{
    public async Task<Result<GetAllAppointmentResponse>> Handle(GetAllAppointmentQuery request, CancellationToken cancellationToken)
    {
        var appointments = await repository.GetAllAppointmentsAsync(
            status: request.Status,
            physicianId: request.PhysicianId,
            patientId: request.PatientId,
            skip: request.Skip,
            top: request.Top,
            cancellationToken: cancellationToken);

        var response = new GetAllAppointmentResponse(
            Appointments: appointments.Item1.Select(appointment => new GetAppointmentResponse(
                Id: appointment.Id,
                PhysicianId: appointment.Schedule.PhysicianId,
                Patient: new GetPatientResult
                    (
                        appointment.Patient.Id,
                        appointment.Patient.User.Name,
                        appointment.Patient.User.Email,
                        appointment.Patient.DateOfBirth
                    ),
                StartTime: appointment.Schedule.StartTime,
                EndTime: appointment.Schedule.EndTime,
                Status: appointment.Status,
                CreatedAt: appointment.CreatedAt
            )),
            Count: appointments.Count
        );
        return Result.Ok(response);
    }
}
public record GetAllAppointmentResponse
    (
        IEnumerable<GetAppointmentResponse> Appointments,
        int Count
    );

public record GetAllAppointmentQuery(int Top, Guid? PhysicianId, AppointmentStatus? Status, Guid? PatientId, int? Skip)
    : IRequest<Result<GetAllAppointmentResponse>>;