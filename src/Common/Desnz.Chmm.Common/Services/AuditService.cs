using Desnz.Chmm.Common.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using Microsoft.AspNetCore.Http;
using Desnz.Chmm.Common.Extensions;
using Desnz.Chmm.Common.ValueObjects;
using System.Reflection;
using System.Security.Claims;

namespace Desnz.Chmm.Common.Services
{
    /// <summary>
    /// Handles auditing Request/Responses for the Audit Behavoir
    /// </summary>
    public class AuditService : IAuditService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuditItemRepository _auditItemRepository;

        private readonly List<HttpStatusCode> _acceptableCodes = new List<HttpStatusCode> {
                    HttpStatusCode.OK,
                    HttpStatusCode.Continue,
                    HttpStatusCode.NoContent,
                    HttpStatusCode.Created,
                    HttpStatusCode.Found
                };

        public AuditService(
        IHttpContextAccessor httpContextAccessor,
        IAuditItemRepository auditItemRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _auditItemRepository = auditItemRepository;
        }

        public AuditDetails GenerateAuditDetails<TRequest>(TRequest request, ResponseDetails responseDetails)
        {
            // Don't continue if we explicitly exlude a command or query
            var requestType = request.GetType();

            bool hasAttribute = requestType.HasAttribute<ExcludeFromAuditAttribute>();
            if (hasAttribute) { return new AuditDetails(); }

            // Get the names of the command/query
            var fullName = requestType.FullName;
            var friendlyName = requestType.GetFriendlyName("Command", "Query");

            // Get the user's Id
            var user = _httpContextAccessor.HttpContext!.User;
            var userId = user.GetUserId();
            Guid? organisationId = ExtractOrganisationId(request, requestType, user);

            // Replace files with filesnames
            var requestDetails = request.CopyObjectWithFileName();

            var headers = _httpContextAccessor.HttpContext.Request.Headers["X-Amzn-Trace-Id"];
            var traceId = "unknown"; // This should only stay as "unknown" in the local environment.
            if (headers.Any())
            {
                traceId = string.Join(", ", headers);
            }

            return new(requestDetails, friendlyName, fullName, traceId, userId, organisationId, responseDetails);
        }

        private static Guid? ExtractOrganisationId<TRequest>(TRequest request, Type requestType, ClaimsPrincipal user)
        {
            // If the user is a manufacturer, then we'll just have the Org Id.
            var organisationId = user.GetOrganisationId();
            if (organisationId != null)
                return organisationId;

            // Most commands and queries have an OrganisationId property we can use if the user isn't logged in.
            PropertyInfo propertyInfo = requestType.GetProperty("OrganisationId");
            if (propertyInfo != null)
            {
                var value = propertyInfo.GetValue(request);
                if (value != null)
                {
                    // Only return Organisation Id if it can successfully be turned into a Guid
                    Guid.TryParse(value.ToString(), out var orgId);
                    return orgId;
                }
            }

            return organisationId;
        }

        /// <summary>
        /// Store the details of the audit item in the database.
        /// </summary>
        /// <typeparam name="TResponse">The Response Type from the IPipelineBehavior</typeparam>
        /// <param name="request">The request details from the client</param>
        /// <param name="details">The response details, including success/failure and the response message</param>
        /// <param name="duration">How long did the transaction take</param>
        /// <returns></returns>
        public async Task LogAuditItem(AuditDetails details, long duration)
        {
            if (details.CanAudit)
            {
                await _auditItemRepository.Create(new AuditItem(
                                            details.TraceId,
                                            details.ResponseDetails.Success,
                                            details.ResponseDetails.Message,
                                            details.UserId,
                                            details.OrganisationId,
                                            details.FriendlyName,
                                            details.FullName,
                                            details.RequestObject,
                                            duration));
            }
        }

        /// <summary>
        /// Process the response, extracting any failures and messages
        /// </summary>
        /// <typeparam name="TResponse">The Response Type from the IPipelineBehavior</typeparam>
        /// <param name="response">The response message from behavour</param>
        /// <returns>Success or failure details</returns>
        public ResponseDetails ProcessResponse<TResponse>(TResponse response)
        {
            var completedSuccessfully = true;
            string? message;

            // Check if the response is an ActionResult or ActionResult<T>
            if (response != null)
            {
                (var responseCode, var responseObj) = extractDetailsFromActionResult(response);
                var responseValue = JsonConvert.SerializeObject(responseObj);

                if (responseCode.HasValue && !_acceptableCodes.Contains(responseCode.Value))
                {
                    completedSuccessfully = false;
                    message = $"({(int?)responseCode} {responseCode}) - {responseValue}";
                }
                else
                {
                    completedSuccessfully = true;
                    message = $"({(int?)responseCode} {responseCode}) - Success";
                }
            }
            else
            {
                message = "Success - Null Response";
            }

            return new(completedSuccessfully, message);
        }

        /// <summary>
        /// Extract details from the exception and mark as unsuccessful
        /// </summary>
        /// <param name="ex">The exception that was thrown</param>
        /// <returns>A ResponseDetails with an exception message and failure marked</returns>
        public ResponseDetails ProcessException(Exception ex)
        {
            if (ex == null)
                return new(false, "Null exception passed");

            var message = ex.Message + Environment.NewLine + ex.StackTrace;
            return new(false, message);
        }

        private (HttpStatusCode? statusCode, object? value) extractDetailsFromActionResult(object actionResult)
        {
            var resultProperty = actionResult.GetType().GetProperty("Result");
            object? value;
            int? statusCodeInt;
            if (resultProperty != null)
            {
                value = getPropertyValue<object>(resultProperty.GetValue(actionResult), "Value");

                statusCodeInt = getPropertyValue<int?>(resultProperty.GetValue(actionResult), "StatusCode");
            }
            else
            {
                value = getPropertyValue<object>(actionResult, "Value");
                statusCodeInt = getPropertyValue<int?>(actionResult, "StatusCode");
            }

            if (value == null && statusCodeInt == null)
            {
                var valueProperty = actionResult.GetType().GetProperty("Value");
                value = valueProperty?.GetValue(actionResult);
                statusCodeInt = 200;
            }

            HttpStatusCode statusCode = (HttpStatusCode)Enum.ToObject(typeof(HttpStatusCode), statusCodeInt ?? 306);

            // Handle the information as needed
            return (statusCode, value);
        }

        private T? getPropertyValue<T>(object? obj, string propertyName)
        {
            if (obj == null)
            {
                return default(T);
            }
            var propertyInfo = obj.GetType().GetProperty(propertyName);
            return propertyInfo != null ? (T?)propertyInfo.GetValue(obj) : default(T);
        }
    }
}
