
using Notify.Models.Responses;

namespace Desnz.Chmm.Common.Services
{
    public interface INotificationService
    {
        Task<EmailNotificationResponse?> SendNotification(string email, string templateId, Dictionary<string, dynamic>? personalisation = null, string? clientReference = null, string? emailReplyToId = null);
    }
}
