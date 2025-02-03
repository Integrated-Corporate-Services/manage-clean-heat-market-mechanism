using Desnz.Chmm.Common.ValueObjects;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net;

namespace Desnz.Chmm.ApiClients.Http
{
    public static class HttpObjectResponseFactory
    {
        public async static Task<HttpObjectResponse<T>> DetermineSuccess<T>(HttpResponseMessage httpResponseMessage) where T : class
        {
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                return TryRetrieveProblem<T>(httpResponseMessage);
            }

            var result = default(T);
            var json = await httpResponseMessage.Content.ReadAsStringAsync();
            if (!string.IsNullOrWhiteSpace(json))
            {
                if (typeof(T) == typeof(string))
                {
                    result = (T)Convert.ChangeType(json, typeof(T));
                }
                else
                {
                    result = JsonConvert.DeserializeObject<T>(json);
                }
            }

            return new HttpObjectResponse<T>(httpResponseMessage, result);
        }

        private static HttpObjectResponse<T> TryRetrieveProblem<T>(HttpResponseMessage httpResponseMessage)
        {
            var unknownProblem = Problem<T>(new ProblemDetails(StatusCodes.Status500InternalServerError, "Unknown error occurred"));

            try
            {
                using var stream = httpResponseMessage.Content.ReadAsStream();
                using var streamReader = new StreamReader(stream);
                var json = streamReader.ReadToEnd();

                // If response body is not JSON, do not attempt to deserialize
                if (!(json?.Contains('{') ?? false)) return unknownProblem;

                var problem = JsonConvert.DeserializeObject<ProblemDetails>(json);
                return Problem<T>(problem);
            }
            catch
            {
                return unknownProblem;
            }
        }

        public static HttpObjectResponse<T> Problem<T>(ProblemDetails problem)
        {
            var statusCode = (HttpStatusCode)problem.Status;

            var json = JsonConvert.SerializeObject(problem);
            var httpResponseMessage = new HttpResponseMessage(statusCode)
            {
                StatusCode = statusCode,
                Content = new StringContent(json)
            };

            return new HttpObjectResponse<T>(httpResponseMessage, default, problem);
        }
    }

    public class HttpObjectResponse<T>
    {
        public virtual HttpResponseMessage Message { get; private set; }

        public virtual T? Result { get; private set; }

        public virtual ProblemDetails? Problem { get; private set; }

        public virtual HttpStatusCode StatusCode => Message.StatusCode;

        public virtual bool IsSuccessStatusCode => Message.IsSuccessStatusCode;

        public HttpObjectResponse(HttpResponseMessage httpResponseMessage, T? result, ProblemDetails? problem = null)
        {
            Message = httpResponseMessage;
            Result = result;
            Problem = problem;
        }
    }

    public class CustomHttpObjectResponse<T> : HttpObjectResponse<T>
    {
        public override HttpResponseMessage Message => throw new NotImplementedException();
        public override ProblemDetails? Problem => throw new NotImplementedException();
        public override HttpStatusCode StatusCode => HttpStatusCode.OK;
        public override bool IsSuccessStatusCode => true;

        public CustomHttpObjectResponse(T result) : base(null, result)
        {
        }
    }
}
