using Desnz.Chmm.Common.Entities;
using Desnz.Chmm.Common.Extensions;
using Desnz.Chmm.Common.Services;
using Microsoft.EntityFrameworkCore;

namespace Desnz.Chmm.Common.Infrastructure
{
    public class EntityDbContext : DbContext
    {
        private readonly ICurrentUserService _userService;

        public EntityDbContext(DbContextOptions options, ICurrentUserService userService) : base(options)
        {
            _userService = userService;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SetCreatedByProperty();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void SetCreatedByProperty()
        {
            var entries = ChangeTracker.Entries();

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added && entry.Entity is Entity)
                {
                    // Ensure the user exists before we try to use them to set the Id
                    var currentUser = _userService.CurrentUser;
                    if (currentUser != null)
                    {
                        var userId = currentUser.GetUserId();
                        ((Entity)entry.Entity).SetCreatedBy(userId);
                    }
                }
            }
        }
    }
}
