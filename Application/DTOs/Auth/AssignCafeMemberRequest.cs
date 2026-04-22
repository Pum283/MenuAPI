namespace Application.DTOs.Auth
{
    public class AssignCafeMemberRequest
    {
        public int CafeId { get; set; }
        public int UserId { get; set; }
        public bool IsBlocked { get; set; }
        public string? Reason { get; set; }
    }
}

