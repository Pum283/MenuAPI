namespace Application.DTOs.Auth
{
    public class AssignCafeMemberRequest
    {
        public Guid CafeId { get; set; }
        public Guid UserId { get; set; }
        public bool IsBlocked { get; set; }
        public string? Reason { get; set; }
    }
}

