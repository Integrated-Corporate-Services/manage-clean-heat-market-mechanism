using Desnz.Chmm.SystemAudit.Common.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.SystemAudit.Common.Commands
{
    public class LogAuditEventCommand : IRequest<ActionResult<Guid>>
    {
        /// <summary>
        /// Gets the object that was passed as the action, denormalised.
        /// </summary>
        public object Details { get; private set; }

        /// <summary>
        /// Gets the number of milliseconds taken to execute the action this audit item refers
        /// to.
        /// </summary>
        public long MillisecondsTaken { get; private set; }

        /// <summary>
        /// Gets the context of the request.
        /// </summary>
        public string CorrelationId { get; private set; }

        /// <summary>
        /// Gets the result of the action.
        /// </summary>
        public string ResultMessage { get; private set; }

        /// <summary>
        /// Gets the username of the actioning user.
        /// </summary>
        public string Username { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the action was successful.
        /// </summary>
        public bool WasSuccessful { get; private set; }
    }
}
