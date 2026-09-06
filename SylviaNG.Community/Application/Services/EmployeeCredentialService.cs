using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.EmployeeCredentials.Models;
using SylviaNG.Community.Application.Interfaces.Externals;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Application.Mappings;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Application.Services
{
    public class EmployeeCredentialService : IEmployeeCredentialService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IEmployeeKeycloakAccountRepository _employeeKeycloakAccountRepository;
        private readonly IKeycloakAdminClient _keycloakAdminClient;
        private readonly IEmailService _emailService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmployeeCredentialService> _logger;

        public EmployeeCredentialService(
            IEmployeeRepository employeeRepository,
            IEmployeeKeycloakAccountRepository employeeKeycloakAccountRepository,
            IKeycloakAdminClient keycloakAdminClient,
            IEmailService emailService,
            IUnitOfWork unitOfWork,
            IConfiguration configuration,
            ILogger<EmployeeCredentialService> logger)
        {
            _employeeRepository = employeeRepository;
            _employeeKeycloakAccountRepository = employeeKeycloakAccountRepository;
            _keycloakAdminClient = keycloakAdminClient;
            _emailService = emailService;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<EmployeeCredentialResponse> CreateAsync(EmployeeCredentialCreateRequest request)
        {
            var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId)
                ?? throw new NotFoundException("Employee", request.EmployeeId);

            if (!employee.IsActive)
                throw new ForbiddenException("Cannot create login credentials for a deactivated employee.");

            var alreadyProvisioned = await _employeeKeycloakAccountRepository.ExistsByEmployeeIdAsync(request.EmployeeId);
            if (alreadyProvisioned)
                throw new DuplicateException("EmployeeKeycloakAccount", "EmployeeId", request.EmployeeId.ToString());

            var role = string.IsNullOrWhiteSpace(request.Role)
                ? _configuration["Keycloak:DefaultRealmRole"] ?? "Employee"
                : request.Role;

            var (firstName, lastName) = SplitName(employee.EmployeeName);

            var keycloakUserId = await _keycloakAdminClient.CreateUserAsync(
                request.Username, employee.Email, firstName, lastName, request.TemporaryPassword, request.EmployeeId);

            await _keycloakAdminClient.AssignRealmRoleAsync(keycloakUserId, role);

            var account = new EmployeeKeycloakAccount
            {
                EmployeeId = request.EmployeeId,
                KeycloakUserId = keycloakUserId,
                Username = request.Username,
                AssignedRole = role,
                IsActive = true
            };
            await _employeeKeycloakAccountRepository.AddAsync(account);
            await _unitOfWork.SaveChangesAsync();

            await SendWelcomeEmailBestEffortAsync(employee, request.Username, request.TemporaryPassword);

            return account.ToResponse();
        }

        /// <summary>
        /// The Keycloak account and EmployeeKeycloakAccount row are already committed by this
        /// point, so a failure here (bad SMTP config, unreachable mail server, etc.) must never
        /// fail the Grant Access call - it's only logged. HR can always communicate credentials
        /// out-of-band if the email doesn't arrive.
        /// </summary>
        private async System.Threading.Tasks.Task SendWelcomeEmailBestEffortAsync(Employee employee, string username, string temporaryPassword)
        {
            if (string.IsNullOrWhiteSpace(employee.Email))
            {
                _logger.LogWarning("Skipping Grant Access welcome email for employee {EmployeeId}: no email on file.", employee.EmployeeId);
                return;
            }

            try
            {
                await _emailService.SendWelcomeEmailAsync(employee.Email, employee.EmployeeName, username, temporaryPassword);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to send Grant Access welcome email to employee {EmployeeId}.", employee.EmployeeId);
            }
        }

        public async System.Threading.Tasks.Task ResetPasswordAsync(long employeeId, string newTemporaryPassword)
        {
            var account = await _employeeKeycloakAccountRepository.GetByEmployeeIdAsync(employeeId)
                ?? throw new NotFoundException("EmployeeKeycloakAccount", employeeId);

            await _keycloakAdminClient.ResetPasswordAsync(account.KeycloakUserId, newTemporaryPassword);
        }

        /// <summary>
        /// Employee has a single free-text EmployeeName (no separate first/last name fields), so this
        /// splits naively on the first space to satisfy Keycloak's firstName/lastName fields.
        /// </summary>
        private static (string FirstName, string LastName) SplitName(string? employeeName)
        {
            if (string.IsNullOrWhiteSpace(employeeName))
                return (string.Empty, string.Empty);

            var parts = employeeName.Trim().Split(' ', 2);
            return parts.Length == 2 ? (parts[0], parts[1]) : (parts[0], string.Empty);
        }
    }
}
