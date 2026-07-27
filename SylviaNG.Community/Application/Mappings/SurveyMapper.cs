using SylviaNG.Community.Application.Features.Surveys.Models;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Application.Mappings
{
    public static class SurveyMapper
    {
        public static Survey ToEntity(this SurveyCreateRequest request)
        {
            return new Survey
            {
                Title = request.Title,
                Description = request.Description,
                SurveyType = request.SurveyType,
                Status = "Draft",
                IsAnonymous = request.IsAnonymous,
                IsMandatory = request.IsMandatory
            };
        }

        public static void ApplyUpdate(this Survey entity, SurveyUpdateRequest request)
        {
            if (request.Title != null) entity.Title = request.Title;
            if (request.Description != null) entity.Description = request.Description;
            if (request.SurveyType != null) entity.SurveyType = request.SurveyType;
            if (request.IsAnonymous.HasValue) entity.IsAnonymous = request.IsAnonymous.Value;
            if (request.IsMandatory.HasValue) entity.IsMandatory = request.IsMandatory.Value;
        }

        public static SurveyDetailResponse ToResponse(this Survey entity)
        {
            return new SurveyDetailResponse
            {
                SurveyId = entity.SurveyId,
                Title = entity.Title,
                Description = entity.Description,
                SurveyType = entity.SurveyType,
                Status = entity.Status,
                IsAnonymous = entity.IsAnonymous,
                IsMandatory = entity.IsMandatory,
                CreatedBy = entity.CreatedBy,
                PublishedAt = entity.PublishedAt,
                ClosedAt = entity.ClosedAt
            };
        }

        public static SurveyQuestion ToEntity(this SurveyQuestionCreateRequest request, long surveyId)
        {
            return new SurveyQuestion
            {
                SurveyId = surveyId,
                QuestionText = request.QuestionText,
                QuestionType = request.QuestionType,
                DisplayOrder = request.DisplayOrder,
                IsRequired = request.IsRequired
            };
        }

        public static void ApplyUpdate(this SurveyQuestion entity, SurveyQuestionUpdateRequest request)
        {
            if (request.QuestionText != null) entity.QuestionText = request.QuestionText;
            if (request.QuestionType != null) entity.QuestionType = request.QuestionType;
            if (request.DisplayOrder.HasValue) entity.DisplayOrder = request.DisplayOrder.Value;
            if (request.IsRequired.HasValue) entity.IsRequired = request.IsRequired.Value;
        }

        public static SurveyOption ToEntity(this SurveyOptionCreateRequest request, long questionId)
        {
            return new SurveyOption
            {
                QuestionId = questionId,
                OptionText = request.OptionText,
                DisplayOrder = request.DisplayOrder
            };
        }

        public static SurveyOptionResponse ToResponse(this SurveyOption entity)
        {
            return new SurveyOptionResponse
            {
                OptionId = entity.OptionId,
                QuestionId = entity.QuestionId,
                OptionText = entity.OptionText,
                DisplayOrder = entity.DisplayOrder
            };
        }

        public static SurveyQuestionResponse ToResponse(this SurveyQuestion entity, List<SurveyOption>? options = null)
        {
            return new SurveyQuestionResponse
            {
                QuestionId = entity.QuestionId,
                SurveyId = entity.SurveyId,
                QuestionText = entity.QuestionText,
                QuestionType = entity.QuestionType,
                DisplayOrder = entity.DisplayOrder,
                IsRequired = entity.IsRequired,
                Options = (options ?? new List<SurveyOption>()).Select(o => o.ToResponse()).ToList()
            };
        }

        public static SurveyAudience ToEntity(this SurveyAudienceCreateRequest request, long surveyId)
        {
            return new SurveyAudience
            {
                SurveyId = surveyId,
                AudienceType = request.AudienceType,
                DepartmentId = request.DepartmentId,
                BranchId = request.BranchId
            };
        }

        public static SurveyAudienceResponse ToResponse(this SurveyAudience entity)
        {
            return new SurveyAudienceResponse
            {
                AudienceId = entity.AudienceId,
                SurveyId = entity.SurveyId,
                AudienceType = entity.AudienceType,
                DepartmentId = entity.DepartmentId,
                BranchId = entity.BranchId
            };
        }

        public static SurveyResponse ToEntity(this SurveySubmissionRequest request, long surveyId)
        {
            return new SurveyResponse
            {
                SurveyId = surveyId,
                EmployeeId = request.EmployeeId,
                SubmittedAt = DateTime.UtcNow,
                CompletionStatus = "Completed"
            };
        }

        public static SurveyAnswer ToEntity(this SurveyAnswerSubmitRequest request, long responseId)
        {
            return new SurveyAnswer
            {
                ResponseId = responseId,
                QuestionId = request.QuestionId,
                OptionId = request.OptionId,
                AnswerText = request.AnswerText
            };
        }

        public static SurveyAnswerResponse ToResponse(this SurveyAnswer entity)
        {
            return new SurveyAnswerResponse
            {
                AnswerId = entity.AnswerId,
                ResponseId = entity.ResponseId,
                QuestionId = entity.QuestionId,
                OptionId = entity.OptionId,
                AnswerText = entity.AnswerText
            };
        }

        public static SurveySubmissionResponse ToResponse(this SurveyResponse entity, List<SurveyAnswer>? answers = null)
        {
            return new SurveySubmissionResponse
            {
                ResponseId = entity.ResponseId,
                SurveyId = entity.SurveyId,
                EmployeeId = entity.EmployeeId,
                SubmittedAt = entity.SubmittedAt,
                CompletionStatus = entity.CompletionStatus,
                Answers = (answers ?? new List<SurveyAnswer>()).Select(a => a.ToResponse()).ToList()
            };
        }
    }
}
