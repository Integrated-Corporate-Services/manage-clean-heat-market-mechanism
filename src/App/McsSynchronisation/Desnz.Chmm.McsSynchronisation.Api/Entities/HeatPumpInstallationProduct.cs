using Desnz.Chmm.McsSynchronisation.Common.Dtos;

namespace Desnz.Chmm.McsSynchronisation.Api.Entities
{
    public class HeatPumpInstallationProduct
    {
        public int Id { get; set; }
        public Guid InstallationId { get; set; }
        public int ProductId { get; set; }


        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as HeatPumpInstallationProduct);
        }

        public bool Equals(HeatPumpInstallationProduct other)
        {
            return other is not null &&
                   Id == other.Id;
        }
    }
}

