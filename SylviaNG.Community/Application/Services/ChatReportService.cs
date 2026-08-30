using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.ChatReports.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Domain.Enums;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;
using Task = System.Threading.Tasks.Task;

namespace SylviaNG.Community.Application.Services
{
    public class ChatReportService : IChatReportService
    {
        private const int MessagePreviewMaxLength = 120;

        private readonly IChatReportRepository _chatReportRepository;
        private readonly IChatMessageRepository _chatMessageRepository;
        private readonly IChatConversationRepository _chatConversationRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ChatReportService(
            IChatReportRepository chatReportRepository,
            IChatMessageRepository chatMessageRepository,
            IChatConversationRepository chatConversationRepository,
            IEmployeeRepository employeeRepository,
            IUnitOfWork unitOfWork)
        {
            _chatReportRepository = chatReportRepository;
            _chatMessageRepository = chatMessageRepository;
            _chatConversationRepository = chatConversationRepository;
            _employeeRepository = employeeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResult<ChatReportQueueItemResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _chatReportRepository.GetPaginatedAsync(request);

            var items = new List<ChatReportQueueItemResponse>();
            foreach (var report in pagedResult.Data)
            {
                items.Add(await EnrichAsync(report));
            }

            return new PagedResult<ChatReportQueueItemResponse>
            {
                Data = items,
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }

        private async Task<ChatReportQueueItemResponse> EnrichAsync(ChatReport report)
        {
            var conversation = await _chatConversationRepository.GetByIdAsync(report.ChatConversationId);
            var message = report.ChatMessageId.HasValue
                ? await _chatMessageRepository.GetByIdAsync(report.ChatMessageId.Value)
                : null;
            var reporter = await _employeeRepository.GetByIdAsync(report.ReportedByEmployeeId);
            var sender = message != null ? await _employeeRepository.GetByIdAsync(message.SenderEmployeeId) : null;

            var conversationTitle = conversation?.Title
                ?? (conversation?.Type == ConversationTypeEnum.Direct ? "Direct message" : $"Conversation #{report.ChatConversationId}");

            var messagePreview = report.ChatMessageId == null
                ? "[conversation reported]"
                : message == null
                    ? "[message deleted]"
                    : message.IsDeleted
                        ? "[message deleted]"
                        : Truncate(message.Body, MessagePreviewMaxLength) ?? string.Empty;

            return new ChatReportQueueItemResponse
            {
                ReportId = report.ChatReportId,
                ReportedByEmployeeId = report.ReportedByEmployeeId,
                ReporterName = reporter?.EmployeeName ?? "Unknown",
                ChatConversationId = report.ChatConversationId,
                ConversationTitle = conversationTitle,
                ConversationType = conversation?.Type.ToString() ?? string.Empty,
                ChatMessageId = report.ChatMessageId,
                MessageBodyPreview = messagePreview,
                IsMessageDeleted = message?.IsDeleted ?? false,
                SenderEmployeeId = message?.SenderEmployeeId ?? 0,
                SenderName = sender?.EmployeeName ?? "Unknown",
                Reason = report.Reason,
                Status = report.Status,
                ReviewedBy = report.ReviewedBy,
                ReviewedAt = report.ReviewedAt,
                CreatedAt = report.CreatedAt
            };
        }

        private static string? Truncate(string? value, int maxLength)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return value.Length <= maxLength ? value : value[..maxLength] + "...";
        }

        public async Task ResolveAsync(long reportId, ChatReportResolveRequest request)
        {
            var entity = await _chatReportRepository.GetByIdAsync(reportId)
                ?? throw new NotFoundException("ChatReport", reportId);

            entity.Status = request.Status;
            entity.ReviewedBy = request.ReviewedBy;
            entity.ReviewedAt = DateTime.UtcNow;

            _chatReportRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
