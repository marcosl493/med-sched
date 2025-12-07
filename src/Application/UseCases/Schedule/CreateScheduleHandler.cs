using Application.Interfaces.Repositories;
using FluentResults;
using MediatR;

namespace Application.UseCases.Schedule;

public class CreateScheduleHandler(IScheduleRepository repository) : IRequestHandler<CreateScheduleCommand, Result<CreateScheduleResponse>>
{
    public async Task<Result<CreateScheduleResponse>> Handle(CreateScheduleCommand request, CancellationToken cancellationToken)
    {
        var startTimeUtc = request.StartTime.ToUniversalTime();
        var endTimeUtc = request.EndTime.ToUniversalTime();
        if (request.EndTime <= request.StartTime)
        {
            return Result.Fail<CreateScheduleResponse>("EndTime must be greater than StartTime");
        }
        if (await repository.IsAvailableScheduleByPhysicianIdAsync(request.PhysicianId, startTimeUtc, endTimeUtc, cancellationToken))
        {
            return Result.Fail<CreateScheduleResponse>("There is already a schedule for this physician in the given time range.");
        }
        var schedule = new Domain.Entities.Schedule
        (
           request.PhysicianId,
           startTimeUtc,
           endTimeUtc
        );
        await repository.CreateScheduleAsync(schedule, cancellationToken);
        return Result.Ok(new CreateScheduleResponse(
            schedule.Id,
            schedule.PhysicianId,
            schedule.StartTime,
            schedule.EndTime))
            .WithSuccess("Created");
    }
}

public record CreateScheduleResponse(Guid Id, Guid PhysicianId,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime);

public record CreateScheduleCommand(
    Guid PhysicianId,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime) : IRequest<Result<CreateScheduleResponse>>;