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
                ReminderDays = request.ReminderDays
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
                CreatedAt = entity.CreatedAt
            };
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
