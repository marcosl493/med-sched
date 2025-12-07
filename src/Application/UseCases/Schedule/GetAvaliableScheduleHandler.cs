using Application.Interfaces.Repositories;
using Domain.Entities;
using FluentResults;
using MediatR;
using System;

namespace Application.UseCases.Schedule;

internal class GetAvaliableScheduleHandler(IScheduleRepository repository) : IRequestHandler<GetAllScheduleQuery, Result<IEnumerable<GetScheduleResponse>>>
{
    public async Task<Result<IEnumerable<GetScheduleResponse>>> Handle(GetAllScheduleQuery request, CancellationToken cancellationToken)
    {
        var result = await repository.GetAllScheduleAsync(request.PhysicianId, request.StartTime?.ToUniversalTime(), request.OnlyAvaliable, request.Top.GetValueOrDefault(), request.Skip, cancellationToken);
        var response = result
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
        return Result.Ok(response);
    }
}



public record GetAllScheduleQuery(
    Guid? PhysicianId,
    DateTimeOffset? StartTime,
    int? Skip,
    bool? OnlyAvaliable,
    int? Top = 50
) : IRequest<Result<IEnumerable<GetScheduleResponse>>>;
