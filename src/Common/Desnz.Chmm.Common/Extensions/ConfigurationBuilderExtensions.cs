using Desnz.Chmm.Common.Configuration;
using Desnz.Chmm.Common.Constants;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace Desnz.Chmm.Common.Extensions;

/// <summary>
/// Configuration builder extensions
/// </summary>
public static class ConfigurationBuilderExtensions
{
    private const string AwsConfigSection = "AWS";

    private static readonly Encoding SecretEncoding = Encoding.UTF8;

    /// <summary>
    /// Get configuration section
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="configurationBuilder"></param>
    /// <param name="section"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static T Get<T>(this ConfigurationManager configurationBuilder, string section) => configurationBuilder.GetSection(section).Get<T>() ??
        throw new ArgumentException($"Invalid configuration section: {section}");

    /// <summary>
    /// Get configuration section
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="configurationBuilder"></param>
    /// <param name="section"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static T Get<T>(this IConfiguration configurationBuilder, string section) => configurationBuilder.GetSection(section).Get<T>() ??
        throw new ArgumentException($"Invalid configuration section: {section}");

    /// <summary>
    /// Add Amazon Secrets Manager to lambda function using environment variables
    /// </summary>
    /// <param name="configurationBuilder"></param>
    /// <param name="region"></param>
    /// <param name="secretName"></param>
    public static void AddAmazonSecretsManager(this IConfigurationBuilder configurationBuilder, string region, string secretName)
    {
        var config = new AmazonSecretsManagerConfiguration { Region = region, Secret = secretName };
        Load(configurationBuilder, config);
    }

    /// <summary>
    /// Add Amazon Secrets Manager to locally running API
    /// </summary>
    /// <param name="configurationBuilder"></param>
    public static void AddAmazonSecretsManager(this ConfigurationManager configurationBuilder)
    {
        // TODO: This will need to be revised
#if DEBUG
        var config = configurationBuilder.Get<AmazonSecretsManagerConfiguration>(AwsConfigSection);
#else
        EnvironmentVariablesConstants.Validate(EnvironmentVariablesConstants.AwsRegion, EnvironmentVariablesConstants.SecretName);
        var config = new AmazonSecretsManagerConfiguration
        {
            Region = Environment.GetEnvironmentVariable(EnvironmentVariablesConstants.AwsRegion)!,
            Secret = Environment.GetEnvironmentVariable(EnvironmentVariablesConstants.SecretName)!
        };
#endif
        Load(configurationBuilder, config);
    }

    private static void Load(IConfigurationBuilder configurationBuilder, AmazonSecretsManagerConfiguration config)
    {
        var loader = new AmazonSecretsManagerLoader(config);
        var secret = loader.GetSecret();
        var stream = new MemoryStream(SecretEncoding.GetBytes(secret ?? string.Empty));
        configurationBuilder.AddJsonStream(stream);
    }
}
