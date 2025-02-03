using Desnz.Chmm.Identity.Common.Commands;
using Desnz.Chmm.ApiClients.Http;
using Microsoft.AspNetCore.Http;
using Desnz.Chmm.Common.Dtos;

namespace Desnz.Chmm.ApiClients;

public interface IUserClient
{
    Task<HttpObjectResponse<List<ChmmUserDto>>> GetAdminUsers();
    Task<HttpObjectResponse<ChmmUserDto>> GetAdminUser(Guid userId);
    Task<HttpObjectResponse<object>> InviteAdminUser(InviteAdminUserCommand command);
    Task<HttpObjectResponse<object>> ActivateAdminUser(ActivateAdminUserCommand command);
    Task<HttpObjectResponse<object>> DeactivateAdminUser(DeactivateAdminUserCommand command);
}

public class UserClient : HttpServiceClient, IUserClient
{
    private const string BaseUrl = "api/identity/users";

    public UserClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor) : base(httpClient, httpContextAccessor) {}

    //async Task<HttpObjectResponse<GetUsersResponse>> IUserClient.GetAllUsers()
    //=> await HttpGetAsync<GetUsersResponse>($"{BaseUrl}/all");

    public async Task<HttpObjectResponse<List<ChmmUserDto>>> GetAdminUsers()
        => await HttpGetAsync<List<ChmmUserDto>>($"{BaseUrl}/admins");

    public async Task<HttpObjectResponse<ChmmUserDto>> GetAdminUser(Guid userId)
        => await HttpGetAsync<ChmmUserDto>($"{BaseUrl}/{userId:guid}");

    public async Task<HttpObjectResponse<object>> InviteAdminUser(InviteAdminUserCommand command)
        => await HttpPostAsync<object>($"{BaseUrl}/admin", command);

    public async Task<HttpObjectResponse<object>> ActivateAdminUser(ActivateAdminUserCommand command)
        => await HttpPostAsync<object>($"{BaseUrl}/admin/activate", command);

    public async Task<HttpObjectResponse<object>> DeactivateAdminUser(DeactivateAdminUserCommand command)
        => await HttpPostAsync<object>($"{BaseUrl}/admin/activate", command);
}