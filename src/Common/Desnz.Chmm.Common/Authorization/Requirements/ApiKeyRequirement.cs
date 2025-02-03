using Microsoft.AspNetCore.Authorization;

namespace Desnz.Chmm.Common.Authorization.Requirements
{
    public class ApiKeyRequirement : IAuthorizationRequirement
    {
        public string HeaderName { get; private set; }
        public string ApiKey { get; private set; }

        public ApiKeyRequirement(string headerName, string apiKey)
        {
            HeaderName = headerName;
            ApiKey = apiKey;
        }
    }
}
