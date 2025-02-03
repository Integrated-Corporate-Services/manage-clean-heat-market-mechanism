
namespace Desnz.Chmm.Common.Configuration;

/// <summary>
/// Configuration stored in appSettings
/// </summary>
public class AmazonSecretsManagerConfiguration
{
    /// <summary>
    /// AWS region
    /// </summary>
    public string Region { get; set; }

    /// <summary>
    /// AWS secret name
    /// </summary>
    public string Secret { get; set; }

    /// <summary>
    /// Credentials, used when running locally
    /// </summary>
    public AmazonSecretsManagerConfigurationCredentials Credentials { get; set; }
}

/// <summary>
/// Credentials configuration stored in appSettings.Development
/// </summary>
public class AmazonSecretsManagerConfigurationCredentials
{
    /// <summary>
    /// AWS access key
    /// </summary>
    public string AccessKey { get; set; }

    /// <summary>
    /// AWS secret key
    /// </summary>
    public string SecretKey { get; set; }
}