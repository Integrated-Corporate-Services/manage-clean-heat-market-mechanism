using System.ComponentModel.DataAnnotations.Schema;

namespace Desnz.Chmm.Common.Entities
{
    public class Entity
    {
        public Guid Id { get; private set; }

        public DateTime CreationDate { get; private set; }

        public string CreatedBy { get; private set; }

        protected Entity() : this(null) { }

        public Entity(string? createdBy)
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
            CreatedBy = createdBy ?? "System";
        }

        internal void SetCreatedBy(Guid? createdBy)
        {
            if(createdBy != null)
            {
                CreatedBy = createdBy.ToString();
            }
        }
    }
}
