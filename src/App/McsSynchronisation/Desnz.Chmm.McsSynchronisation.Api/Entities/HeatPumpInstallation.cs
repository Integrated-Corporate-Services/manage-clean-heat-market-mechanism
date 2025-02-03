using Desnz.Chmm.McsSynchronisation.Common.Dtos;

namespace Desnz.Chmm.McsSynchronisation.Api.Entities
{
    public class HeatPumpInstallation : McsInstallationDto
    {
        public Guid Id { get; set; }
        
        [Newtonsoft.Json.JsonIgnore()]
        public Guid InstallationRequestId { get; set; }

        [Newtonsoft.Json.JsonIgnore()]
        public InstallationRequest InstallationRequest { get; set; }

        public List<HeatPumpProduct> HeatPumpProducts { get; set; }

        [Newtonsoft.Json.JsonIgnore()]
        public decimal Credits { get; set; }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as HeatPumpInstallation);
        }

        public bool Equals(HeatPumpInstallation other)
        {
            return other is not null &&
                   Id == other.Id;
        }
    }
}

