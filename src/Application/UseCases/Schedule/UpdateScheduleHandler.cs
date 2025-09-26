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
            return Result.Fail(new Error("Recurso não encontrado.")
                .WithMetadata("StatusCode", 404));

        if (schedule.PhysicianId != request.PhysicianId)
            return Result.Fail(new Error("Recurso não autorizado")
                .WithMetadata("StatusCode", 403));

        schedule.Update(request.StartTime, request.EndTime);
        await repository.UpdateScheduleAsync(schedule, cancellationToken);
        return Result.Ok();
    }
}

public record UpdateScheduleCommand(Guid Id, Guid PhysicianId, DateTime StartTime, DateTime EndTime) : IRequest<Result>;