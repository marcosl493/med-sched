using Application.Interfaces.Repositories;
using FluentResults;
using MediatR;

namespace Application.UseCases.Schedule;

internal class GetScheduleHandler(IScheduleRepository repository) : IRequestHandler<GetScheduleCommand, Result<GetScheduleResponse>>
{
    public async Task<Result<GetScheduleResponse>> Handle(GetScheduleCommand request, CancellationToken cancellationToken)
    {
        var schedule = await repository.GetScheduleByIdAsync(request.Id, cancellationToken);
        if (schedule is null)
            return Result.Ok();

        var physicianDto = new PhysicianDto
            (
                schedule.Physician.Id,
                schedule.Physician.User.Name,
                schedule.Physician.Specialty
            );
        var response = new GetScheduleResponse(
            schedule.Id,
            schedule.IsAvaliableSchedule(),
            schedule.StartTime,
            schedule.EndTime,
            physicianDto
        );
        return Result.Ok(response);
    }
}

public record GetScheduleResponse(
    Guid Id,
    bool IsAvaliable,
    DateTime StartTime,
    DateTime EndTime,
    PhysicianDto Physician
);
public record PhysicianDto(
    Guid Id,
    string Name,
    string Specialty
);
public record GetScheduleCommand(Guid Id) : IRequest<Result<GetScheduleResponse>>;