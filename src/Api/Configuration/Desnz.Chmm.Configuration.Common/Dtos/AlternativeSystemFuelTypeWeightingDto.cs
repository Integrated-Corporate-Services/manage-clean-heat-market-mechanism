namespace Desnz.Chmm.Configuration.Common.Dtos
{
    public class AlternativeSystemFuelTypeWeightingDto
    {
        public Guid? Id { get; set; }
        public AlternativeSystemFuelTypeWeightingValueDto AlternativeSystemFuelTypeWeightingValue { get; set; }
        public string Code { get; set; }
    }
    public class AlternativeSystemFuelTypeWeightingValueDto
    {
        public Guid? Id { get; set; }
        public decimal Value { get; set; }
    }
}
