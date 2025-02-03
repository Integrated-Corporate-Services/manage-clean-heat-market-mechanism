using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.Common.Configuration;
using Desnz.Chmm.Common.Configuration.Settings;
using Desnz.Chmm.Common.Constants;
using Desnz.Chmm.Identity.Api.Entities;
using Desnz.Chmm.Identity.Api.Exceptions;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Desnz.Chmm.Identity.Api.Infrastructure.Setup;
using Desnz.Chmm.Identity.Api.Services;
using Desnz.Chmm.Identity.Common.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace Desnz.Chmm.Identity.Api.Handlers.Commands;

public class GetJwtTokenCommandHandler : IRequestHandler<GetJwtTokenCommand, ActionResult<string>>
{
    private readonly ITokenService _tokenService;
    private readonly IUsersRepository _usersRepository;
    private readonly IOneLoginService _oneLoginService;
    private readonly IOptions<EnvironmentConfig> _environmentConfig;
    private readonly ApiKeyPolicyConfig _mcsSynchronisationPolicyConfig;
    private readonly ILogger<GetJwtTokenCommandHandler> _logger;

    public GetJwtTokenCommandHandler(
        ITokenService tokenService,
        IUsersRepository usersRepository,
        IOneLoginService oneLoginService,
        IOptions<EnvironmentConfig> environmentConfig,
        IOptions<ApiKeyPolicyConfig> mcsSynchronisationPolicyConfig,
        ILogger<GetJwtTokenCommandHandler> logger)
    {
        _tokenService = tokenService;
        _usersRepository = usersRepository;
        _oneLoginService = oneLoginService;
        _environmentConfig = environmentConfig;
        _mcsSynchronisationPolicyConfig = mcsSynchronisationPolicyConfig.Value
                ?? throw new InvalidOperationException(ConfigurationValueConstants.GetErrorMessage(ConfigurationValueConstants.ApiKeyPolicy));
        _oneLoginService = oneLoginService;
        _logger = logger;
    }

    public async Task<ActionResult<string>> Handle(GetJwtTokenCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Attempting to validate GOV.UK One Login ID token: {idToken}", command.IdToken);

        var subject = string.Empty;
        var email = string.Empty;

        if (command.ApiKey != null && command.ApiKey == _mcsSynchronisationPolicyConfig.ApiKey)
        {
            _logger.LogInformation("Generating jwt token for API key: {apiKey}", command.ApiKey);
            return _tokenService.GenerateJwtToken(email, null, command.ApiKey);
        }

        if (!string.IsNullOrEmpty(command.Email) && (_environmentConfig.Value.EnvironmentName == "dev" || _environmentConfig.Value.EnvironmentName == "test"))
        {
            email = command.Email;
        }
        else
        {
            await _tokenService.ValidateTokenAsync(command.IdToken, cancellationToken);
            var response = await _oneLoginService.GetUserInfo(command.AccessToken);
            if (!response.IsSuccessStatusCode)
            {
                response.Problem.ThrowException();
            }

            subject = response.Result?.Sub;
            if (string.IsNullOrWhiteSpace(subject))
            {
                throw new ChmmIdentityException("GOV.UK One Login returned an empty subject for user");
            }

            email = response.Result?.Email;
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ChmmIdentityException("GOV.UK One Login returned an empty email for user");
            }
        }

        email = email.ToLower();

        var subjectLoginAttempt = await TryLoginBySubject(email, subject);
        if (subjectLoginAttempt.Success) return subjectLoginAttempt.Token!;

        var emailLoginAttempt = await TryLoginByEmail(email, subject);
        if (emailLoginAttempt.Success) return emailLoginAttempt.Token!;

        _logger.LogInformation("Generating default user token without claims for subject: {subject} with email: {email}", subject, email);
        return _tokenService.GenerateJwtToken(email, null);
    }

    private record LoginAttempt(bool Success, string? Token);

    private async Task<LoginAttempt> TryLoginBySubject(string email, string subject)
    {
        if (string.IsNullOrWhiteSpace(subject)) return new LoginAttempt(false, null);

        _logger.LogInformation("Attempting to retrieve CHMM user by its GOV.UK One Login subject: {subject}", subject);
        var userBySubject = await _usersRepository.GetBySubject(subject, true, true);
        if (userBySubject == null) return new LoginAttempt(false, null);

        _logger.LogInformation("Generating JWT token for user with subject: {subject}", subject);
        var token = _tokenService.GenerateJwtToken(email, userBySubject);
        return new LoginAttempt(true, token);
    }

    private async Task<LoginAttempt> TryLoginByEmail(string email, string subject)
    {
        _logger.LogInformation("Attempting to retrieve CHMM user by its GOV.UK One Login email: {email}", email);
        var userByEmail = await _usersRepository.GetByEmail(email, true, true);
        if (userByEmail == null) return new LoginAttempt(false, null);

        // Fail if user found by email address already has a subject.
        // This likely means there's something malicious occurring.
        if (!string.IsNullOrWhiteSpace(userByEmail.Subject))
        {
            _logger.LogInformation("CHMM user with email: {email} was found, but has an existing subject: {userByEmailSubject}, which differs from GOV.UK One Login subject: {oneLoginSubject}", email, userByEmail.Subject, subject);
            return new LoginAttempt(false, null);
        }

        // This will only be called the first time a user logs in.
        // It will bind their user account to a GOV.UK One Login subject.
        // Next time they log in, it will identify them by subject rather than email.
        await SetSubject(userByEmail, subject);

        _logger.LogInformation("Generating JWT token for user with email: {email}", email);
        var token = _tokenService.GenerateJwtToken(email, userByEmail);
        return new LoginAttempt(true, token);
    }

    private async Task SetSubject(ChmmUser user, string subject)
    {
        if (string.IsNullOrWhiteSpace(subject)) return;

        _logger.LogInformation("Setting subject: {subject} to CHMM user with email: {email}", subject, user.Email);
        user.SetSubject(subject);
        await _usersRepository.UnitOfWork.SaveChangesAsync();
    }
}
