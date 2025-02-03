namespace Desnz.Chmm.Identity.ApiTests;

public static class Endpoints
{
    public static class Get
    {
        public static string AllUsers = "api/identity/users";

        public static string AdminUsers = "api/identity/users/admin";

        public static string AdminById(Guid userId) => $"api/identity/users/admin/{userId}";

        public static string AdminRoles = "api/identity/roles/admin";
    }

    public static class Post
    {
        public static string InviteAdminUser = "api/identity/users/admin";
        public static string ActivateAdminUser = "api/identity/users/admin/activate";
        public static string DeactivateAdminUser = "api/identity/users/admin/deactivate";
    }
}