using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Common.Extensions
{
    public static class ExceptionExtensions
    {
        public static ProblemDetails ToProblemDetails(this Exception exception) 
            => new ProblemDetails() { 
                Title = exception.Message, 
                Status = StatusCodes.Status400BadRequest 
            };

        public static ProblemDetails ToProblemDetails(this ValidationException exception)
        {
            var error = exception.Errors.FirstOrDefault();

            return new ProblemDetails()
            {
                Title = error?.ErrorMessage ?? "Error",
                Status = StatusCodes.Status400BadRequest
            };
        }
            
    }
}
