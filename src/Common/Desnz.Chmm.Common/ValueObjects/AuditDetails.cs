namespace Desnz.Chmm.Common.ValueObjects
{
    /// <summary>
    /// The details regarding the request/response compiled ready for storing.
    /// </summary>
    public class AuditDetails
    {
        /// <summary>
        /// By default an audit item cannot be stored.
        /// The default constructor just sets CanAudit to false
        /// </summary>
        public AuditDetails()
        {
            CanAudit = false;
            TraceId = string.Empty;
            FriendlyName = string.Empty;
            FullName = string.Empty;
            ResponseDetails = ResponseDetails.Default;
        }

        /// <summary>
        /// Constructs the audit details object
        /// </summary>
        /// <param name="requestObject">The request that was sent to the back end</param>
        /// <param name="friendlyName">The friendly name of the command / query</param>
        /// <param name="fullName">The full name of the command / query, including namespace</param>
        /// <param name="traceId">The X-Amzn-Trace-Id of the request.</param>
        /// <param name="userId">The Id of the user making the call</param>
        /// <param name="responseDetails">Details about the response from the back end</param>
        public AuditDetails(
            object requestObject,
            string friendlyName,
            string fullName,
            string traceId,
            Guid? userId,
            Guid? organisationId,
            ResponseDetails responseDetails)
        {
            RequestObject = requestObject;
            FriendlyName = friendlyName;
            FullName = fullName;
            TraceId = traceId;
            UserId = userId;
            OrganisationId = organisationId;
            ResponseDetails = responseDetails;
            CanAudit = true;
        }

        /// <summary>
        /// Defines if the audit item should be logged or not
        /// </summary>
        public bool CanAudit { get; private set; }

        /// <summary>
        /// Details about the response from the backend
        /// </summary>
        public ResponseDetails ResponseDetails { get; private set; }

        /// <summary>
        /// The X-Amzn-Trace-Id of the request
        /// </summary>
        public string TraceId { get; private set; }

        /// <summary>
        /// The Id of the user making the call
        /// </summary>
        public Guid? UserId { get; private set; }

        /// <summary>
        /// The Id of the organisastion the query / command was run on (if any)
        /// </summary>
        public Guid? OrganisationId { get; }

        /// <summary>
        /// The friendly name of the command / query 
        /// </summary>
        public string FriendlyName { get; private set; }

        /// <summary>
        /// The full name of the command / query including namespace
        /// </summary>
        public string FullName { get; private set; }

        /// <summary>
        /// The whole object sent to the backend
        /// </summary>
        public object? RequestObject { get; private set; }
    }
}
