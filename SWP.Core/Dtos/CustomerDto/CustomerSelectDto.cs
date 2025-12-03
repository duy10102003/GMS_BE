namespace SWP.Core.Dtos.CustomerDto
{
    /// <summary>
    /// DTO cho Customer trong select (với search)
    /// Created by: DuyLC(03/12/2025)
    /// </summary>
    public class CustomerSelectDto
    {
        public int CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string CustomerPhone { get; set; } = string.Empty;
        public string? CustomerEmail { get; set; }
    }
}

