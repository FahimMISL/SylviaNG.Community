using SylviaNG.Community.Application.Features.RecurringTasks.Models;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Application.Mappings
{
    public static class RecurringTaskMapper
    {
        public static RecurringTask ToEntity(this RecurringTaskCreateRequest request)
        {
            return new RecurringTask
            {
                Frequency = request.Frequency,
                IntervalValue = request.IntervalValue,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                IsActive = true
            };
        }

        public static void ApplyUpdate(this RecurringTask entity, RecurringTaskUpdateRequest request)
        {
            if (request.Frequency != null) entity.Frequency = request.Frequency;
            if (request.IntervalValue.HasValue) entity.IntervalValue = request.IntervalValue.Value;
            if (request.StartDate.HasValue) entity.StartDate = request.StartDate.Value;
            if (request.EndDate.HasValue) entity.EndDate = request.EndDate;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static RecurringTaskResponse ToResponse(this RecurringTask entity)
        {
            return new RecurringTaskResponse
            {
                RecurringTaskId = entity.RecurringTaskId,
                Frequency = entity.Frequency,
                IntervalValue = entity.IntervalValue,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                IsActive = entity.IsActive
            };
        }
    }
}
