using Desnz.Chmm.Common.Constants;
using Desnz.Chmm.Common.Dtos;
using Desnz.Chmm.Common.Entities;
using Newtonsoft.Json;

namespace Desnz.Chmm.Common.ValueObjects
{
    public class AuditItem : Entity
    {
        /// <summary>
        /// Default constructor for EF
        /// </summary>
        protected AuditItem() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditItem"/> class.
        /// </summary>
        /// <param name="traceId">The X-Amzn-Trace-Id of the request.</param>
        /// <param name="wasSuccessful">True if the action was successful.</param>
        /// <param name="resultMessage">The message returned, either "Success" or the exception details.</param>
        /// <param name="userId">The id of the user who actioned the event.</param>
        /// <param name="organisationId">The id of the organisation the event was run on (null if not an org event).</param>
        /// <param name="friendlyName">The friendly name of the event.</param>
        /// <param name="fullName">The full name and namespace of the event.</param>
        /// <param name="details">The object that was passed as the action.</param>
        /// <param name="millisecondsTaken">The number of milliseconds taken to execute the action this
        /// audit item refers to.</param>
        public AuditItem(
                string traceId,
                bool wasSuccessful,
                string resultMessage,
                Guid? userId,
                Guid? organisationId,
                string friendlyName,
                string fullName,
                object? details,
                long millisecondsTaken) : base()
        {
            TraceId = traceId;
            ResultMessage = resultMessage;
            UserId = userId;
            OrganisationId = organisationId;
            AuditType = AuditTypeConstants.GetAuditType(fullName);
            FriendlyName = friendlyName;
            FullName = fullName;
            Details = details == null ? string.Empty : JsonConvert.SerializeObject(details);
            WasSuccessful = wasSuccessful;
            MillisecondsTaken = millisecondsTaken;
        }

        /// <summary>
        /// Gets the object that was passed as the action, denormalised.
        /// </summary>
        public string Details { get; private set; }

        /// <summary>
        /// Gets the number of milliseconds taken to execute the action this audit item refers
        /// to.
        /// </summary>
        public long MillisecondsTaken { get; private set; }

        /// <summary>
        /// Gets the context of the request.
        /// </summary>
        public string TraceId { get; private set; }

        /// <summary>
        /// Gets the result of the action.
        /// </summary>
        public string ResultMessage { get; private set; }

        /// <summary>
        /// Gets the user Id of the actioning user.
        /// </summary>
        public Guid? UserId { get; private set; }

        /// <summary>
        /// If the event was run on an organisation, store its id
        /// </summary>
        public Guid? OrganisationId { get; private set; }

        /// <summary>
        /// The audit type (command/query)
        /// </summary>
        public string AuditType { get; private set; }

        /// <summary>
        /// A friendly name for the command/query that was executed
        /// </summary>
        public string FriendlyName { get; private set; }

        /// <summary>
        /// The full name (including namespace) of the command/query that was executed
        /// </summary>
        public string FullName { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the action was successful.
        /// </summary>
        public bool WasSuccessful { get; private set; }

        public AuditItemDto ToDto(List<ChmmUserDto> users)
        {
            return new AuditItemDto(users, this);
        }
    }
}
