using Application.Interfaces.Repositories;
using FluentResults;
using MediatR;

namespace Application.UseCases.Schedule;

internal class GetAllScheduleHandler(IScheduleRepository repository)
    : IRequestHandler<GetAllScheduleQuery, Result<GetAllScheduleResponse>>
{
    public async Task<Result<GetAllScheduleResponse>> Handle(GetAllScheduleQuery request, CancellationToken cancellationToken)
    {
        var result = await repository.GetAllScheduleAsync(request.PhysicianId, request.StartTime?.ToUniversalTime(), request.OnlyAvaliable, request.Top.GetValueOrDefault(), request.Skip, cancellationToken);
        var schedulesResult = result
            .Schedules
            .Select(sched =>
            new GetScheduleResponse
            (
                sched.Id,
                sched.IsAvaliableSchedule(),
                sched.StartTime,
                sched.EndTime,
                new PhysicianDto
                (
                    sched.Physician.Id,
                    sched.Physician.User.Name,
                    sched.Physician.Specialty))
            );
        var response = new GetAllScheduleResponse(schedulesResult, result.Count);
        return Result.Ok(response);
    }
}



public record GetAllScheduleQuery(
    Guid? PhysicianId,
    DateTimeOffset? StartTime,
    int? Skip,
    bool? OnlyAvaliable,
    int? Top = 50
) : IRequest<Result<GetAllScheduleResponse>>;
public record GetAllScheduleResponse(
    IEnumerable<GetScheduleResponse> Schedules,
    int Count
);
