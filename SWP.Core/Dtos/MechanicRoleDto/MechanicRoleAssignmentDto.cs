namespace SWP.Core.Dtos.MechanicRoleDto
{
    public class MechanicRoleAssignmentDto
    {
        public int MechanicRolePermissionId { get; set; }
        public int MechanicRoleId { get; set; }
        public string? MechanicRoleName { get; set; }
        public int UserId { get; set; }
        public int YearExp { get; set; }
    }
}
