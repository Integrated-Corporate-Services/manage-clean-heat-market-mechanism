using Desnz.Chmm.Common.Configuration.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Notify.Client;
using Notify.Models.Responses;

namespace Desnz.Chmm.Common.Services
{
    public class NotificationService : INotificationService
    {
        private readonly NotificationClient _notificationClient;
        private readonly IOptions<GovukNotifyConfig> _govukNotifyConfig;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(IOptions<GovukNotifyConfig> govukNotifyConfig, ILogger<NotificationService> logger)
        {
            _govukNotifyConfig = govukNotifyConfig;
            _logger = logger;
            _notificationClient = new NotificationClient(govukNotifyConfig.Value.ApiKey);
        }

        private async Task<EmailNotificationResponse?> SendEmail(string email, string templateId, Dictionary<string, dynamic>? personalisation = null, string? clientReference = null, string? emailReplyToId = null)
        {
            if(_govukNotifyConfig.Value.DisableNotifications == "Disable")
                return null;

            if (!string.IsNullOrEmpty(_govukNotifyConfig.Value.OverrideEmailDestination))
                email = _govukNotifyConfig.Value.OverrideEmailDestination;

            var response = await _notificationClient.SendEmailAsync(email, templateId, personalisation, clientReference, emailReplyToId);
            return response;        
        }

        public async Task<EmailNotificationResponse?> SendNotification(string email, string templateId, Dictionary<string, dynamic>? personalisation = null, string? clientReference = null, string? emailReplyToId = null)
        {
            EmailNotificationResponse? response = null;
            try
            {
                _logger.LogInformation("Sending notification with Template Id: {templateId} to {email}, Template Personalisation: {personalisation}", templateId, email, personalisation);
                response = await SendEmail(
                    email,
                    templateId,
                    personalisation,
                    clientReference,
                    emailReplyToId
                );
                _logger.LogInformation("Notification with Template Id: {templateId} was sent and received response: {response}", templateId, JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                _logger.LogError("Error: {error} sending notification with Template Id: {templateId} to {email}, Template Personalisation: {personalisation}", JsonConvert.SerializeObject(ex), templateId, email, personalisation);
            }
            return response;
        }
    }
}
