using Application.Interfaces.Repositories;
using FluentResults;
using MediatR;

namespace Application.UseCases.Schedule;

internal class GetAllAvaliableScheduleHandler(IScheduleRepository repository) : IRequestHandler<GetAllAvaliableScheduleQuery, Result<IEnumerable<GetScheduleResponse>>>
{
    public async Task<Result<IEnumerable<GetScheduleResponse>>> Handle(GetAllAvaliableScheduleQuery request, CancellationToken cancellationToken)
    {
        var result = await repository.GetAllAvaliableScheduleAsync(request.PhysicianId, request.StartTime, request.Top.GetValueOrDefault(), request.Skip, cancellationToken);
        var response = result
            .Select(sched => 
            new GetScheduleResponse
            (
                sched.Id,
                sched.StartTime,
                sched.EndTime,
                new PhysicianDto
                (
                    sched.Physician.Id,
                    sched.Physician.User.Name,
                    sched.Physician.Specialty))
            );
        return Result.Ok(response);
    }
}



public record GetAllAvaliableScheduleQuery(
    Guid? PhysicianId,
    DateTime? StartTime,
    int? Skip,
    int? Top = 50
) : IRequest<Result<IEnumerable<GetScheduleResponse>>>;
