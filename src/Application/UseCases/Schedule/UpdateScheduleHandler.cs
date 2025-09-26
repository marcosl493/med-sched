using Application.Interfaces.Repositories;
using FluentResults;
using MediatR;

namespace Application.UseCases.Schedule;

public class UpdateScheduleHandler(IScheduleRepository repository) : IRequestHandler<UpdateScheduleCommand, Result>
{
    public async Task<Result> Handle(UpdateScheduleCommand request, CancellationToken cancellationToken)
    {
        var schedule = await repository.GetScheduleByIdAsync(request.Id, cancellationToken);
        if (schedule is null)
            return Result.Fail(new Error("Schedule not found.")
                .WithMetadata("StatusCode", 404));

        if (schedule.PhysicianId != request.PhysicianId)
            return Result.Fail(new Error("Schedule not found.")
                .WithMetadata("StatusCode", 403));

        schedule.Update(request.StartTime, request.EndTime);
        await repository.UpdateScheduleAsync(schedule, cancellationToken);
        return Result.Ok();
    }
}

public record UpdateScheduleCommand(Guid Id, Guid PhysicianId, DateTime StartTime, DateTime EndTime) : IRequest<Result>;