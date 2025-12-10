namespace SWP.Core.Dtos.UserDto
{
    /// <summary>
    /// DTO cho Technical Staff trong select (với trạng thái rảnh/không rảnh)
    /// Created by: DuyLC(03/12/2025)
    /// </summary>
    public class TechnicalStaffSelectDto
    {
        public int UserId { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public int? RoleId { get; set; }
        public string? RoleName { get; set; }
        public bool IsAvailable { get; set; } // true = rảnh, false = không rảnh
        public int? CurrentTaskCount { get; set; } // Số task đang làm hoặc đang được assign
    }
}


