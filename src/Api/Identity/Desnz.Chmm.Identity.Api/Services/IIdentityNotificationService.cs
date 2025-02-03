using Desnz.Chmm.Identity.Api.Entities;

namespace Desnz.Chmm.Identity.Api.Services
{
    public interface IIdentityNotificationService
    {
        Task NotifyUserActivated(ChmmUser user);
        Task NotifyAdminUserEdited(ChmmUser user);
        Task NotifyManufacturerUserEdited(ChmmUser user);
        Task NotifyUserDeactivated(ChmmUser user);
        Task NotifyAdminUserInvited(ChmmUser user);
        Task NotifyManufacturerUserInvited(ChmmUser user, string invitingUserName);
        Task NotifyManufacturerApproved(Organisation organisation);
        Task NotifyManufacturerOnboarded(Organisation organisation);
        Task NotifyOrganisationEdited(Organisation organisation);
        Task NotifyManufacturerRejected(Organisation organisation);
    }
}