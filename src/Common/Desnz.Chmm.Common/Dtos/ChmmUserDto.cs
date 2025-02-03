
namespace Desnz.Chmm.Common.Dtos
{
    public class ChmmUserDto
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string TelephoneNumber { get; set; }
        public string Status { get; set; }
        public List<RoleDto>? ChmmRoles { get; set; }
    }
}
