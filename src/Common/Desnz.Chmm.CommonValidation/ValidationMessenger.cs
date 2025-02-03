using Desnz.Chmm.Common.Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text;
using Newtonsoft.Json;

namespace Desnz.Chmm.CommonValidation
{
    public class ValidationMessenger : IValidationMessenger
    {
        private readonly ILogger<ValidationMessenger> _logger;

        public ValidationMessenger(ILogger<ValidationMessenger> logger)
        {
            this._logger = logger;
        }

        private string faledToGet(string @object, string @for, string forIdProperty, bool hasProblem = false)
            => $"Failed to get {@object} for {@for} with Id: {{{forIdProperty}}}" + appendProblem(hasProblem);
        private string faledToGet(string @object, string forIdProperty, bool hasProblem = false)
            => $"Failed to get {@object} with Id: {{{forIdProperty}}}" + appendProblem(hasProblem);
        private string faledToGet(string @object, bool hasProblem = true)
            => $"Failed to get {@object}" + appendProblem(hasProblem);
        private string appendProblem(bool hasProblem)
        {
            var message = "";
            if (hasProblem)
            {
                message = ", problem: {problem}";
            }
            return message;
        }
        public BadRequestObjectResult CannotLoadOrganisation(Guid organisationId, Common.ValueObjects.ProblemDetails? problemDetails)
        {
            return LogErrorAndReturnBadRequest(faledToGet("Organisation", "organisationId", true), organisationId, JsonConvert.SerializeObject(problemDetails));
        }
        public BadRequestObjectResult CannotLoadSchemeYear(Guid schemeYearId, Common.ValueObjects.ProblemDetails? problemDetails)
        {
            return LogErrorAndReturnBadRequest(faledToGet("Scheme Year", "schemeYearId", true), schemeYearId, JsonConvert.SerializeObject(problemDetails));
        }
        public BadRequestObjectResult CannotLoadCurrentSchemeYear(Common.ValueObjects.ProblemDetails? problemDetails)
        {
            return LogErrorAndReturnBadRequest("Failed to get current scheme year, problem: {problem}", problemDetails);
        }

        public BadRequestObjectResult InvalidOrganisationStatus(Guid organisationId, string status)
        {
            return LogErrorAndReturnBadRequest("Organisation with Id: {organisationId} has an invalid status: {status}", organisationId, status);
        }
        public BadRequestObjectResult InvalidLoadSchemeYearQuarter(Guid schemeYearId, Common.ValueObjects.ProblemDetails? problemDetails)
        {
            return LogErrorAndReturnBadRequest(faledToGet("Scheme Year Quarter", "Scheme Year", "schemeYearId", true), schemeYearId, JsonConvert.SerializeObject(problemDetails));
        }

        public BadRequestObjectResult InvalidObligationAmendment(Guid organisationId)
        {
            return LogErrorAndReturnBadRequest("Total obligation cannot be amended to a negative balance. Organisation Id: {organisationId}", organisationId);
        }

        private BadRequestObjectResult LogErrorAndReturnBadRequest(string? message, params object?[] args)
        {
            var processedMessage = LogMessage(message, args);
            return Responses.BadRequest(string.Format(processedMessage, args));
        }

        private string LogMessage(string? message, params object?[] args)
        {
            _logger.LogError(message, args);
            message = message ?? string.Empty;
            string? processedMessage;
            if (message.IndexOf("{") > 0)
            {
                processedMessage = ReplaceWithIncrementingNumbers(message);
            }
            else
            { processedMessage = message; }
            return processedMessage;
        }

        private static string ReplaceWithIncrementingNumbers(string input)
        {
            StringBuilder result = new StringBuilder();
            int count = 0;

            int start = 0;
            int end;

            while ((end = input.IndexOf('{', start)) != -1)
            {
                result.Append(input, start, end - start);

                start = input.IndexOf('}', end) + 1;
                result.Append("{" + count++ + "}");
            }

            result.Append(input, start, input.Length - start);

            return result.ToString();
        }
    }
}
