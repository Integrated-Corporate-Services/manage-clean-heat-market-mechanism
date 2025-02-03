namespace Desnz.Chmm.Common.Constants
{
    public static class EnvironmentVariablesConstants
    {
        public const string AwsRegion = "AWS_REGION";
        public const string SecretName = "SECRET_NAME";

        public static string GetErrorMessage(string envVarName) => $"Unable to get the {envVarName} from the environment variables. Missing variable: {envVarName}";

        public static void Validate(params string[] envVarNames)
        {
            foreach (var envVarName in envVarNames)
            {
                if (Environment.GetEnvironmentVariable(envVarName) == null)
                {
                    throw new ArgumentNullException(GetErrorMessage(envVarName));
                }
            }
        }
    }
}
