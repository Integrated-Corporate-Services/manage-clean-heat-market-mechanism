namespace Desnz.Chmm.BoilerSales.Common.Dtos.Annual
{
    public class AnnualBoilerSalesDto : SalesNumbersDto
    {
        #region Properties

        /// <summary>
        /// Scheme year boiler sales are reported for
        /// </summary>
        public Guid SchemeYearId { get; set; }

        /// <summary>
        /// Organisation boiler sales are reported for
        /// </summary>
        public Guid OrganisationId { get; set; }

        /// <summary>
        /// Admin review <see cref="BoilerSalesConstants.Status">status</see>
        /// </summary>
        public string? Status { get; set; }

        /// <summary>
        /// Files to support annual boiler sales
        /// </summary>
        public List<AnnualBoilerSalesFileDto>? Files { get; set; }

        /// <summary>
        /// History of changes
        /// </summary>
        public List<AnnualBoilerSalesChangeDto>? Changes { get; set; }

        #endregion
    }
}
