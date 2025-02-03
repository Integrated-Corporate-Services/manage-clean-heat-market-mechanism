using Desnz.Chmm.Common.Entities;
using Newtonsoft.Json;

namespace Desnz.Chmm.Identity.Api.Entities
{
    public class ChmmRole : Entity
    {
        public string Name { get; private set; }

        private List<ChmmUser> _chmmUsers;

        [JsonIgnore]
        public IReadOnlyCollection<ChmmUser> ChmmUsers => _chmmUsers;

        protected ChmmRole(): base() { }

        public ChmmRole(string name, string? createdBy = null) : base(createdBy)
        {
            Name = name;
        }
    }
}
