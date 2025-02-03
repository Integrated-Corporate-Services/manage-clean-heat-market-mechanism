using Amazon;
using Amazon.Runtime;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;

namespace Desnz.Chmm.Common.Configuration;

/// <summary>
/// Load secrets from AWS
/// </summary>
public class AmazonSecretsManagerLoader
{
    private const string VersionStage = "AWSCURRENT";

    private readonly AmazonSecretsManagerConfiguration _config;

    public AmazonSecretsManagerLoader(AmazonSecretsManagerConfiguration config)
    {
        _config = config;
    }

    /// <summary>
    /// Get secret (returns JSON)
    /// </summary>
    /// <returns></returns>
    public string GetSecret()
    {
        using var client = CreateClient();

        var request = new GetSecretValueRequest
        {
            SecretId = _config.Secret,
            VersionStage = VersionStage
        };

        var response = client.GetSecretValueAsync(request).Result;
        return response.SecretString;
    }

    /// <summary>
    /// Create AWS client, with or without credentials
    /// </summary>
    /// <returns></returns>
    private AmazonSecretsManagerClient CreateClient()
    {
        var region = RegionEndpoint.GetBySystemName(_config.Region);

        // Client without credentials, when running on AWS infrastructure
        if (_config.Credentials == null) return new AmazonSecretsManagerClient(region);

        // Client with credentials, when running locally
        var credentials = new BasicAWSCredentials(_config.Credentials.AccessKey, _config.Credentials.SecretKey);
        return new AmazonSecretsManagerClient(credentials, region);
    }
}
