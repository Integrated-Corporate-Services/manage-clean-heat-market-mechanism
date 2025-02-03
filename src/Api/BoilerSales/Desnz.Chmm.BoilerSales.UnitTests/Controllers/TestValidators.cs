using Desnz.Chmm.BoilerSales.Common.Commands.Annual.SupportingEvidence;
using Desnz.Chmm.BoilerSales.Common.Commands.Annual.VerificationStatement;
using Desnz.Chmm.BoilerSales.Common.Queries.Annual.SupportingEvidence;
using Desnz.Chmm.BoilerSales.Common.Queries.Annual.VerificationStatement;
using Desnz.Chmm.BoilerSales.Common.Queries.Annual;
using Desnz.Chmm.BoilerSales.Common.Queries.Quarterly;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Desnz.Chmm.BoilerSales.UnitTests.Controllers
{
    public class TestValidators
    {    /// <summary>
         /// Validates a GetAnnualBoilerSalesQuery instance
         /// </summary>
         /// <param name="q">GetAnnualBoilerSalesQuery instance to validate</param>
         /// <param name="organisationId">Expected Organisation ID</param>
         /// <param name="schemeYearId">Expected Scheme Year ID</param>
        internal static bool ValidateGetAnnualBoilerSalesQuery(GetAnnualBoilerSalesQuery q, Guid? organisationId, Guid schemeYearId)
        {
            Assert.Equal(organisationId, q.OrganisationId);
            Assert.Equal(schemeYearId, q.SchemeYearId);
            return true;
        }

        /// <summary>
        /// Validates a GetQuarterlyBoilerSalesQuery instance
        /// </summary>
        /// <param name="q">GetQuarterlyBoilerSalesQuery instance to validate</param>
        /// <param name="organisationId">Expected Organisation ID</param>
        /// <param name="schemeYearId">Expected Scheme Year ID</param>
        internal static bool ValidateGetQuarterlyBoilerSalesQuery(GetQuarterlyBoilerSalesQuery q, Guid? organisationId, Guid schemeYearId)
        {
            Assert.Equal(organisationId, q.OrganisationId);
            Assert.Equal(schemeYearId, q.SchemeYearId);
            return true;
        }

        /// <summary>
        /// Validates a GetAnnualVerificationStatementFileNamesQuery instance
        /// </summary>
        /// <param name="q">GetAnnualVerificationStatementFileNamesQuery instance to validate</param>
        /// <param name="organisationId">Expected Organisation ID</param>
        /// <param name="schemeYearId">Expected Scheme Year ID</param>
        internal static bool ValidateGetAnnualVerificationStatementFileNamesQuery(GetAnnualVerificationStatementFileNamesQuery q, Guid organisationId, Guid schemeYearId)
        {
            Assert.Equal(organisationId, q.OrganisationId);
            Assert.Equal(schemeYearId, q.SchemeYearId);
            return true;
        }

        /// <summary>
        /// Validates a GetAnnualSupportingEvidenceFileNamesQuery instance
        /// </summary>
        /// <param name="q">GetAnnualSupportingEvidenceFileNamesQuery instance to validate</param>
        /// <param name="organisationId">Expected Organisation ID</param>
        /// <param name="schemeYearId">Expected Scheme Year ID</param>
        internal static bool ValidateGetAnnualSupportingEvidenceFileNamesQuery(GetAnnualSupportingEvidenceFileNamesQuery q, Guid organisationId, Guid schemeYearId)
        {
            Assert.Equal(organisationId, q.OrganisationId);
            Assert.Equal(schemeYearId, q.SchemeYearId);
            return true;
        }

        /// <summary>
        /// Validates a UploadAnnualVerificationStatementCommand instance
        /// </summary>
        /// <param name="q">UploadAnnualVerificationStatementCommand instance to validate</param>
        /// <param name="organisationId">Expected Organisation ID</param>
        /// <param name="schemeYearId">Expected Scheme Year ID</param>
        /// <param name="formFiles">Expected set of form files</param>
        /// <returns></returns>
        internal static bool ValidateUploadAnnualVerificationStatementCommand(UploadAnnualVerificationStatementCommand q, Guid organisationId, Guid schemeYearId, List<IFormFile> formFiles)
        {
            Assert.Equal(organisationId, q.ManufacturerId);
            Assert.Equal(schemeYearId, q.SchemeYearId);
            Assert.Equal(formFiles, q.VerificationStatement);
            return true;
        }

        /// <summary>
        /// Validates a UploadAnnualSupportingEvidenceCommand instance
        /// </summary>
        /// <param name="q">UploadAnnualSupportingEvidenceCommand instance to validate</param>
        /// <param name="organisationId">Expected Organisation ID</param>
        /// <param name="schemeYearId">Expected Scheme Year ID</param>
        /// <param name="formFiles">Expected set of form files</param>
        internal static bool ValidateUploadAnnualSupportingEvidenceCommand(UploadAnnualSupportingEvidenceCommand q, Guid organisationId, Guid schemeYearId, List<IFormFile> formFiles)
        {
            Assert.Equal(organisationId, q.OrganisationId);
            Assert.Equal(schemeYearId, q.SchemeYearId);
            Assert.Equal(formFiles, q.SupportingEvidence);
            return true;
        }

        /// <summary>
        /// Validates a DeleteAnnualVerificationStatementCommand instance
        /// </summary>
        /// <param name="q">DeleteAnnualVerificationStatementCommand instance to validate</param>
        /// <param name="organisationId">Expected Organisation ID</param>
        /// <param name="schemeYearId">Expected Scheme Year ID</param>
        /// <param name="fileName">Expected file name</param>
        internal static bool ValidateDeleteAnnualVerificationStatementCommand(DeleteAnnualVerificationStatementCommand q, Guid organisationId, Guid schemeYearId, string fileName)
        {
            Assert.Equal(organisationId, q.OrganisationId);
            Assert.Equal(schemeYearId, q.SchemeYearId);
            Assert.Equal(fileName, q.FileName);
            return true;
        }

        /// <summary>
        /// DeleteAnnualSupportingEvidenceCommand instance to validate
        /// </summary>
        /// <param name="q">DeleteAnnualSupportingEvidenceCommand instance to validate</param>
        /// <param name="organisationId">Expected Organisation ID</param>
        /// <param name="schemeYearId">Expected Scheme Year ID</param>
        /// <param name="fileName">Expected file name</param>
        internal static bool ValidateDeleteAnnualSupportingEvidenceCommand(DeleteAnnualSupportingEvidenceCommand q, Guid organisationId, Guid schemeYearId, string fileName)
        {
            Assert.Equal(organisationId, q.OrganisationId);
            Assert.Equal(schemeYearId, q.SchemeYearId);
            Assert.Equal(fileName, q.FileName);
            return true;
        }
    }
}
