using SylviaNG.Community.Application.Features.Tasks.Models;
using SylviaNG.Community.Domain.Entities;
using TaskEntity = SylviaNG.Community.Domain.Entities.Task;

namespace SylviaNG.Community.Application.Mappings
{
    public static class TaskMapper
    {
        public static TaskEntity ToEntity(this TaskCreateRequest request)
        {
            return new TaskEntity
            {
                TeamId = request.TeamId,
                AssignedBy = request.AssignedBy,
                AssignedTo = request.AssignedTo,
                RecurringTaskId = request.RecurringTaskId,
                Title = request.Title,
                Description = request.Description,
                Priority = request.Priority,
                TaskStatus = request.Status,
                DueDate = request.DueDate,
                // US-7.9: reminder window defaults to 2 days when the caller doesn't set one.
                ReminderDays = request.ReminderDays ?? 2
            };
        }

        public static void ApplyUpdate(this TaskEntity entity, TaskUpdateRequest request)
        {
            if (request.TeamId.HasValue) entity.TeamId = request.TeamId.Value;
            if (request.AssignedTo.HasValue) entity.AssignedTo = request.AssignedTo.Value;
            if (request.RecurringTaskId.HasValue) entity.RecurringTaskId = request.RecurringTaskId;
            if (request.Title != null) entity.Title = request.Title;
            if (request.Description != null) entity.Description = request.Description;
            if (request.Priority != null) entity.Priority = request.Priority;
            if (request.Status != null) entity.TaskStatus = request.Status;
            if (request.DueDate.HasValue) entity.DueDate = request.DueDate;
            if (request.ReminderDays.HasValue) entity.ReminderDays = request.ReminderDays;
        }

        public static TaskResponse ToResponse(this TaskEntity entity)
        {
            return new TaskResponse
            {
                TaskId = entity.TaskId,
                TeamId = entity.TeamId,
                AssignedBy = entity.AssignedBy,
                AssignedTo = entity.AssignedTo,
                RecurringTaskId = entity.RecurringTaskId,
                Title = entity.Title,
                Description = entity.Description,
                Priority = entity.Priority,
                Status = entity.TaskStatus,
                DueDate = entity.DueDate,
                ReminderDays = entity.ReminderDays,
                CreatedBy = entity.CreatedBy,
                CreatedAt = entity.CreatedAt,
                DerivedStatus = ComputeDerivedStatus(entity)
            };
        }

        /// <summary>US-7.9: Due Soon is distinct from Overdue - within the reminder window but not yet
        /// past the due date. No due date means neither can apply.</summary>
        private static string ComputeDerivedStatus(TaskEntity entity)
        {
            if (string.Equals(entity.TaskStatus, "Completed", StringComparison.OrdinalIgnoreCase))
                return "Completed";

            if (!entity.DueDate.HasValue)
                return "OnTrack";

            var now = DateTime.UtcNow;
            if (entity.DueDate.Value < now)
                return "Overdue";

            var reminderDays = entity.ReminderDays ?? 2;
            if ((entity.DueDate.Value - now).TotalDays <= reminderDays)
                return "DueSoon";

            return "OnTrack";
        }

        public static TaskComment ToEntity(this TaskCommentAddRequest request, long taskId)
        {
            return new TaskComment
            {
                TaskId = taskId,
                EmployeeId = request.EmployeeId,
                Comment = request.Comment
            };
        }

        public static TaskCommentResponse ToResponse(this TaskComment entity)
        {
            return new TaskCommentResponse
            {
                CommentId = entity.CommentId,
                TaskId = entity.TaskId,
                EmployeeId = entity.EmployeeId,
                Comment = entity.Comment,
                CreatedAt = entity.CreatedAt
            };
        }

        public static TaskAttachment ToEntity(this TaskAttachmentAddRequest request, long taskId)
        {
            return new TaskAttachment
            {
                TaskId = taskId,
                FileName = request.FileName,
                FileType = request.FileType,
                FilePath = request.FilePath,
                FileSize = request.FileSize,
                UploadedBy = request.UploadedBy
            };
        }

        public static TaskAttachmentResponse ToResponse(this TaskAttachment entity)
        {
            return new TaskAttachmentResponse
            {
                AttachmentId = entity.AttachmentId,
                TaskId = entity.TaskId,
                FileName = entity.FileName,
                FileType = entity.FileType,
                FilePath = entity.FilePath,
                FileSize = entity.FileSize,
                UploadedBy = entity.UploadedBy,
                CreatedAt = entity.CreatedAt
            };
        }

        public static TaskHistoryResponse ToResponse(this TaskHistory entity)
        {
            return new TaskHistoryResponse
            {
                HistoryId = entity.HistoryId,
                TaskId = entity.TaskId,
                Action = entity.Action,
                OldValue = entity.OldValue,
                NewValue = entity.NewValue,
                ChangedBy = entity.ChangedBy,
                CreatedAt = entity.CreatedAt
            };
        }
    }
}
