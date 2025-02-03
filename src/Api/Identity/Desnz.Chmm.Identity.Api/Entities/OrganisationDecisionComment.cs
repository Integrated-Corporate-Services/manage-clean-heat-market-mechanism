using Desnz.Chmm.Common.Entities;

namespace Desnz.Chmm.Identity.Api.Entities
{
    public class OrganisationDecisionComment : Entity
    {
        public string Comment { get; private set; }
        public string Decision { get; private set; }
        public Guid ChmmUserId { get; private set; }
        public Guid OrganisationId { get; private set; }

        public ChmmUser ChmmUser { get; private set; }
        public Organisation Organisation { get; private set; }

        protected OrganisationDecisionComment() : base()
        { 
        }

        public OrganisationDecisionComment(string comment, Guid userId, Guid orgId, string decision) : base()
        {
            Comment = comment;
            ChmmUserId = userId;
            OrganisationId = orgId;
            Decision = decision;
        }
    }
}
