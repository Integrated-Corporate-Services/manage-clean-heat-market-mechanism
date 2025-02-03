using Desnz.Chmm.Common.Constants;
using Desnz.Chmm.Common.Entities;
using Desnz.Chmm.Identity.Api.Constants;
using Desnz.Chmm.Identity.Common.Dtos.ManufacturerUser;
using static Desnz.Chmm.Common.Constants.IdentityConstants;

namespace Desnz.Chmm.Identity.Api.Entities
{
    public class ChmmUser : Entity
    {
        #region Properties

        public string? Subject { get; private set; }
        public string Name { get; private set; }
        public string Email { get => _email; private set => _email = value.ToLower(); }
        public string? JobTitle { get; private set; }
        public string? TelephoneNumber { get; private set; }
        public string? ResponsibleOfficerOrganisationName { get; private set; }
        public string Status { get; private set; }
        public Guid? OrganisationId { get; private set; }

        public IReadOnlyCollection<ChmmRole> ChmmRoles => _chmmRoles;
        public Organisation? Organisation { get; private set; }
        public IReadOnlyCollection<OrganisationDecisionComment> Comments => _comments;

        #endregion

        #region Private fields

        private List<ChmmRole> _chmmRoles;
        private List<OrganisationDecisionComment> _comments;
        private string _email;

        #endregion

        #region Constructors

        protected ChmmUser() : base() { }

        public ChmmUser(string name, string email, List<ChmmRole> roles, string? createdBy = null) : base(createdBy)
        {
            Name = name;
            Email = email;
            Status = UsersConstants.Status.Active;
            if (roles.Any(r => r.Name == IdentityConstants.Roles.Manufacturer))
            {
                throw new ArgumentException("Admin roles only expected");
            };
            _chmmRoles = roles;
        }

        public ChmmUser(CreateManufacturerUserDto user, List<ChmmRole> roles, bool isActive = false, string? createdBy = null) : base(createdBy)
        {
            SetManufacturerUserDetails(user);
            Status = isActive ? UsersConstants.Status.Active : UsersConstants.Status.Inactive;
            if (roles.FirstOrDefault(r => r.Name == IdentityConstants.Roles.Manufacturer) == default)
            {
                throw new ArgumentException("Manufacturer role expected");
            };
            _chmmRoles = roles;
        }

        public ChmmUser(CreateManufacturerUserDto user, List<ChmmRole> roles, Guid? organisationId, bool isActive = false, string? createdBy = null)
            : this(user, roles, isActive, createdBy)
        {
            OrganisationId = organisationId;
        }

        #endregion

        #region Behaviours

        public void SetSubject(string subject)
        {
            Subject = subject;
        }

        public void SetName(string name)
        {
            Name = name;
        }

        public void SetStatus(string status)
        {
            Status = status;
        }

        internal void UpdateDetails(string name, string jobTitle, string telephoneNumber)
        {
            Name = name;
            JobTitle = jobTitle;
            TelephoneNumber = telephoneNumber;
        }

        public void SetManufacturerUserDetails(CreateManufacturerUserDto user)
        {
            Name = user.Name;
            Email = user.Email;
            JobTitle = user.JobTitle;
            TelephoneNumber = user.TelephoneNumber;
            ResponsibleOfficerOrganisationName = user.ResponsibleOfficerOrganisationName;
        }

        public void Activate()
        {
            Status = UsersConstants.Status.Active;
        }

        public void Deactivate()
        {
            Status = UsersConstants.Status.Inactive;
        }

        public void AddRole(ChmmRole role)
        {
            _chmmRoles.Add(role);
        }

        public void UpdateRoles(List<ChmmRole> roles)
        {
            var newRoles = ChmmRoles.ToList();

            newRoles.RemoveAll(oldRole => roles.All(newRole => oldRole.Id != newRole.Id));
            newRoles.AddRange(roles.Where(newRole => newRoles.All(oldRole => oldRole.Id != newRole.Id)));

            _chmmRoles = newRoles;
        }

        public void RemoveRole(ChmmRole role)
        {
            _chmmRoles.Remove(role);
        }

        public bool IsAdmin()
        {
            return ChmmRoles.Any(r => Roles.AdminsList.Contains(r.Name));
        }

        internal void AddToOrganisaton(Organisation organisation)
        {
            Organisation = organisation;
        }

        #endregion

        public override string ToString()
        {
            return $"{Id}-{Name}-{Status}";
        }
    }
}
