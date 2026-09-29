using Application.Interfaces.Repositories;
using FluentResults;
using MediatR;

namespace Application.UseCases.Schedule;

public class DeleteScheduleHandler(IScheduleRepository repository) : IRequestHandler<DeleteScheduleRequest, Result>
{
    private readonly IScheduleRepository _repository = repository;
    public async Task<Result> Handle(DeleteScheduleRequest request, CancellationToken cancellationToken)
    {
        var isExistings = await _repository.IsAvaliableToDeleteAsync(request.ScheduleId, cancellationToken);
        if (!isExistings)
            return Result.Fail(new Error("Schedule not found.")
                .WithMetadata("StatusCode", 404));

        await _repository.DeleteAsync(request.ScheduleId, cancellationToken);
        return Result.Ok();
    }
}
public record DeleteScheduleRequest(Guid PhysicianId, Guid ScheduleId)
    : IRequest<Result>;
