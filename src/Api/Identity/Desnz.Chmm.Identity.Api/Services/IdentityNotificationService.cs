using Desnz.Chmm.Common.Configuration.Settings;
using Desnz.Chmm.Common.Constants;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.Identity.Api.Entities;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Microsoft.Extensions.Options;

namespace Desnz.Chmm.Identity.Api.Services
{
    public class IdentityNotificationService : IIdentityNotificationService
    {
        private readonly ILogger<IdentityNotificationService> _logger;
        private readonly INotificationService _notificationService;
        private readonly IUsersRepository _usersRepository;
        private readonly EnvironmentConfig _environmentConfig;

        public IdentityNotificationService(ILogger<IdentityNotificationService> logger,
                                           INotificationService notificationService,
                                           IUsersRepository usersRepository,
                                           IOptions<GovukNotifyConfig> govukNotifyConfig,
                                           IOptions<EnvironmentConfig> environmentConfig)
        {
            _logger = logger;
            _notificationService = notificationService;
            _usersRepository = usersRepository;
            _environmentConfig = environmentConfig.Value ?? throw new InvalidOperationException(ConfigurationValueConstants.GetErrorMessage(ConfigurationValueConstants.EnvironmentConfig));
        }

        public async Task NotifyUserActivated(ChmmUser user)
        {
            _logger.LogInformation($"Notify user {user.Name} activated");

            await _notificationService.SendNotification(
                user.Email,
                GovukNotifyTemplateConstants.AccountActivatedTemplateId,
                new Dictionary<string, dynamic>
                {
                    { "url", _environmentConfig.BaseUrl },
                    { "email", user.Email }
                }
            );
        }

        public async Task NotifyUserDeactivated(ChmmUser user)
        {
            _logger.LogInformation($"Notify user {user.Name} deactivated");

            await _notificationService.SendNotification(
                user.Email,
                GovukNotifyTemplateConstants.AccountDeactivatedTemplateId,
                new Dictionary<string, dynamic>
                {
                    { "url", _environmentConfig.BaseUrl },
                    { "email", user.Email }
                }
            );
        }

        public async Task NotifyAdminUserInvited(ChmmUser user)
        {
            _logger.LogInformation($"Notify admin user {user.Name} invited");

            await _notificationService.SendNotification(
                user.Email,
                GovukNotifyTemplateConstants.AdminAccountCreatedTemplateId,
                new Dictionary<string, dynamic>
                {
                    { "url", _environmentConfig.BaseUrl },
                    { "email", user.Email },
                    { "name", user.Name },
                    { "status", user.Status },
                    { "roles", string.Join(", ", user.ChmmRoles.Select(r => r.Name).ToArray()) }
                }
            );
        }

        public async Task NotifyManufacturerUserInvited(ChmmUser user, string invitingUserName)
        {
            _logger.LogInformation($"Notify manufacturer user {user.Name} invited");

            await _notificationService.SendNotification(
                user.Email,
                GovukNotifyTemplateConstants.ManufacturerAccountCreatedTemplateId,
                new Dictionary<string, dynamic>
                {
                    { "url", _environmentConfig.BaseUrl },
                    { "name", user.Name },
                    { "user-that-did-inviting", invitingUserName }
                }
            );
        }

        public async Task NotifyAdminUserEdited(ChmmUser user)
        {
            _logger.LogInformation($"Notify admin user {user.Name} edited");

            await _notificationService.SendNotification(
                user.Email,
                GovukNotifyTemplateConstants.AccountUpdatedTemplateId,
                new Dictionary<string, dynamic> {
                    { "url", _environmentConfig.BaseUrl },
                    { "email", user.Email },
                    { "name", user.Name },
                    { "status", user.Status },
                    { "roles", string.Join(", ", user.ChmmRoles.Select(r => r.Name).ToArray()) }
                }
            );
        }

        public async Task NotifyManufacturerUserEdited(ChmmUser user)
        {
            _logger.LogInformation($"Notify manufacturer user {user.Name} edited");

            await _notificationService.SendNotification(
                user.Email,
                GovukNotifyTemplateConstants.AccountUpdatedTemplateId,
                new Dictionary<string, dynamic> {
                    { "url", _environmentConfig.BaseUrl },
                    { "email", user.Email },
                    { "name", user.Name },
                    { "jobTitle", user.JobTitle },
                    { "telephoneNumber", user.TelephoneNumber }
                }
            );
        }

        public async Task NotifyOrganisationEdited(Organisation organisation)
        {
            _logger.LogInformation($"Notify organisation {organisation.Name} users organisation edited");

            var usersToNotify = await _usersRepository.GetAdmins();
            usersToNotify.AddRange(organisation.ChmmUsers);

            Parallel.ForEach(usersToNotify, async (user, token) =>
            {
                await _notificationService.SendNotification(
                    user.Email,
                    GovukNotifyTemplateConstants.ManufacturerUpdatedTemplateId,
                    new Dictionary<string, dynamic>
                    {
                        { "userName", user.Name },
                        { "organisationName", organisation.Name }
                    });
            });
        }

        public async Task NotifyManufacturerOnboarded(Organisation organisation)
        {
            _logger.LogInformation($"Notify {organisation.Name} users manufacturer onboarded");

            await Notify(organisation, GovukNotifyTemplateConstants.ManufacturerSubmitApplicationTemplateId);
        }

        public async Task NotifyManufacturerApproved(Organisation organisation)
        {
            _logger.LogInformation($"Notify {organisation.Name} users manufacturer approved");

            await Notify(organisation, GovukNotifyTemplateConstants.ManufacturerApplicationApprovedTemplateId);
        }

        public async Task NotifyManufacturerRejected(Organisation organisation)
        {
            _logger.LogInformation($"Notify {organisation.Name} users manufacturer rejected");

            await Notify(organisation, GovukNotifyTemplateConstants.ManufacturerApplicationRejectedTemplateId);
        }

        private async Task Notify(Organisation organisation, string templateId)
        {
            var usersToNotify = await _usersRepository.GetAdmins();
            usersToNotify.AddRange(organisation.ChmmUsers);

            Parallel.ForEach(usersToNotify, async (user, token) =>
            {
                await _notificationService.SendNotification(
                    user.Email,
                    templateId,
                    new Dictionary<string, dynamic>
                    {
                        { "organisationName", organisation.Name },
                        { "userName", user.Name },
                        { "applicantName", organisation.ChmmUsers.Single(u => u.Id == organisation.ApplicantId).Name },
                        { "responsibleOfficerName", organisation.ChmmUsers.Single(u => u.Id == organisation.ResponsibleOfficerId).Name },
                        { "date", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss") },
                        { "url", _environmentConfig.BaseUrl },
                    });
            });
        }
    }
}