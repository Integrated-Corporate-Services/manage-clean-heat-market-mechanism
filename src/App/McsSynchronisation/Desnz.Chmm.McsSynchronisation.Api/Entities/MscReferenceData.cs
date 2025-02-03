namespace Desnz.Chmm.McsSynchronisation.Api.Entities
{
    public class MscReferenceData
    {
        public int Id { get; set; }

        public string Description { get; set; }
    }

    public class AirTypeTechnology : MscReferenceData { }
    public class AlternativeSystemFuelType : MscReferenceData { }
    public class AlternativeSystemType : MscReferenceData { }
    public class InstallationAge : MscReferenceData { }
    public class Manufacturer : MscReferenceData { }
    public class NewBuildOption : MscReferenceData { }
    public class RenewableSystemDesign : MscReferenceData { }
    public class TechnologyType : MscReferenceData { }
}
