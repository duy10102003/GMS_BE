namespace SWP.Core.Dtos.MechanicRoleDto
{
    /// <summary>
    /// Thông tin thợ được gán vào một mechanic role
    /// </summary>
    public class MechanicRoleMechanicDto
    {
        public int MechanicRolePermissionId { get; set; }
        public int MechanicRoleId { get; set; }
        public string? MechanicRoleName { get; set; }
        public int UserId { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public int YearExp { get; set; }
    }
}
